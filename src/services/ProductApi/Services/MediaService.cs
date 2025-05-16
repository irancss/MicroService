using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using ProductApi.Infrastructure.Settings;
using ProductApi.Models.Dtos;
using ProductApi.Models.Entities;
using ProductApi.Repositories;

namespace ProductApi.Services;

public class MediaService : IMediaService
{
    private readonly IAmazonS3 _s3Client; // همچنان از کلاینت S3 استفاده می‌کنیم
    private readonly MinioSettings _minioSettings; // تنظیمات MinIO
    private readonly CdnSettings _cdnSettings;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<MediaService> _logger;

    public MediaService(
        IAmazonS3 s3Client, // این کلاینت با تنظیمات MinIO ساخته شده است
        IOptions<MinioSettings> minioOptions,
        IOptions<CdnSettings> cdnOptions,
        IProductRepository productRepository,
        ILogger<MediaService> logger)
    {
        _s3Client = s3Client;
        _minioSettings = minioOptions.Value;
        _cdnSettings = cdnOptions.Value;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PresignedUrlResponse> GeneratePresignedUploadUrlAsync(string productId, string fileName, string contentType)
    {
        // ایجاد کلید در مسیر موقت (pending)
        var pendingS3Key = $"{_minioSettings.PendingUploadPrefix.TrimEnd('/')}/{Guid.NewGuid()}-{Path.GetFileName(fileName)}";

        _logger.LogInformation("Generating presigned URL for Pending Key: {PendingS3Key}", pendingS3Key);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _minioSettings.BucketName,
            Key = pendingS3Key, // کلید موقت
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(_minioSettings.PresignedUrlExpirationMinutes),
            ContentType = contentType
        };

        try
        {
            // GetPreSignedURL سنکرون است، اما می‌توان در Task.Run اجرا کرد
            var url = await Task.Run(() => _s3Client.GetPreSignedURL(request));
            return new PresignedUrlResponse { UploadUrl = url, S3Key = pendingS3Key }; // کلید موقت را برمی‌گردانیم
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, "S3 Error generating presigned URL for key {pendingS3Key}", pendingS3Key);
            throw; // یا مدیریت خطا به شکل دیگر
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error generating presigned URL for key {pendingS3Key}", pendingS3Key);
            throw;
        }
    }

    public async Task<bool> ConfirmMediaUploadAsync(string productId, string s3Key, string mediaType, string altText, int order)
    {
        _logger.LogInformation("Confirming upload for Product: {ProductId}, PendingS3Key: {PendingS3Key}", productId, pendingS3Key);

        // 1. تعیین کلید نهایی فایل در MinIO
        var finalS3Key = $"products/{productId}/{Path.GetFileName(pendingS3Key)}"; // حذف بخش Guid و پیشوند pending

        // 2. انتقال (کپی و سپس حذف) فایل از مکان موقت به مکان نهایی در MinIO
        bool movedInStorage = await MoveObjectInStorageAsync(pendingS3Key, finalS3Key);
        if (!movedInStorage)
        {
            _logger.LogError("Failed to move object from {PendingS3Key} to {FinalS3Key} in MinIO.", pendingS3Key, finalS3Key);
            // فایل در مکان موقت باقی می‌ماند و توسط TTL پاک خواهد شد
            return false;
        }
        _logger.LogInformation("Object moved successfully to {FinalS3Key}", finalS3Key);


        // 3. ایجاد آبجکت MediaInfo با کلید نهایی
        var mediaInfo = new MediaInfo
        {
            Id = Guid.NewGuid().ToString(),
            S3Key = finalS3Key, // کلید نهایی
            MediaType = mediaType,
            AltText = altText,
            Order = order,
            Url = $"{_cdnSettings.BaseUrl.TrimEnd('/')}/{finalS3Key}" // URL نهایی
        };

        // 4. افزودن اطلاعات مدیا به سند محصول در MongoDB
        var dbSuccess = await _productRepository.AddMediaAsync(productId, mediaInfo);

        if (!dbSuccess)
        {
            _logger.LogWarning("Failed to add media info to Product {ProductId} in DB for FinalS3Key {FinalS3Key}. Storage object might need manual cleanup.", productId, finalS3Key);
            // اینجا باید فایل نهایی را از MinIO پاک کرد (Rollback)
            await DeleteS3ObjectAsync(finalS3Key); // استفاده از متد کمکی حذف
            return false;
        }

        _logger.LogInformation("Media info added successfully to Product {ProductId} for FinalS3Key {FinalS3Key}", productId, finalS3Key);
        return true;
    }

    // متد کمکی برای انتقال فایل در MinIO (Copy + Delete)
    private async Task<bool> MoveObjectInStorageAsync(string sourceKey, string destinationKey)
    {
        try
        {
            // کپی کردن آبجکت
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = _minioSettings.BucketName,
                SourceKey = sourceKey,
                DestinationBucket = _minioSettings.BucketName,
                DestinationKey = destinationKey,
                // MetadataDirective = S3MetadataDirective.COPY // کپی کردن متادیتا (اختیاری)
            };
            _logger.LogDebug("Copying object from {SourceKey} to {DestinationKey}", sourceKey, destinationKey);
            await _s3Client.CopyObjectAsync(copyRequest);

            // حذف آبجکت مبدا
            _logger.LogDebug("Deleting source object {SourceKey} after copy", sourceKey);
            await DeleteS3ObjectAsync(sourceKey); // استفاده از متد کمکی حذف

            return true;
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, "S3 Error moving object from {SourceKey} to {DestinationKey}", sourceKey, destinationKey);
            // اگر کپی موفق بود ولی حذف ناموفق، فایل مبدا باقی می‌ماند و با TTL پاک می‌شود
            // اگر کپی ناموفق بود، هیچ اتفاقی در مقصد نیفتاده است
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error moving object from {SourceKey} to {DestinationKey}", sourceKey, destinationKey);
            return false;
        }
    }


    public async Task<bool> DeleteMediaAsync(string productId, string mediaId)
    {
        _logger.LogInformation("Attempting to delete media with ID: {MediaId} for Product: {ProductId}", mediaId, productId);

        // 1. یافتن محصول و اطلاعات مدیا برای گرفتن S3 Key
        var product = await _productRepository.GetByIdAsync(productId); // نیاز به متد GetById بدون فیلتر IsActive
        var mediaToRemove = product?.Media.FirstOrDefault(m => m.Id == mediaId);
        if (mediaToRemove == null || string.IsNullOrEmpty(mediaToRemove.S3Key)) return false;



        if (mediaToRemove == null || string.IsNullOrEmpty(mediaToRemove.S3Key))
        {
            _logger.LogWarning("Media not found or S3Key is missing for MediaId: {MediaId}, ProductId: {ProductId}", mediaId, productId);
            return false; // یا throw KeyNotFoundException
        }


        // ... (حذف فایل نهایی از MinIO) ...
        var s3Success = await DeleteS3ObjectAsync(mediaToRemove.S3Key);
        if (!s3Success)
        {
            _logger.LogWarning("Failed to delete object from S3 with Key: {S3Key}. DB entry was removed.", mediaToRemove.S3Key);
            // اینجا نیاز به مکانیزم هشدار یا بازیابی است
        }

        // 3. حذف اطلاعات مدیا از MongoDB
        var dbSuccess = await _productRepository.RemoveMediaAsync(productId, mediaId);

        if (!dbSuccess)
        {
            _logger.LogError("Failed to remove media info from DB for MediaId: {MediaId}, ProductId: {ProductId}", mediaId, productId);
            // ادامه ندهید چون فایل در S3 باقی می‌ماند
            return false;
        }


        _logger.LogInformation("Media deleted successfully (DB and S3) for MediaId: {MediaId}, ProductId: {ProductId}", mediaId, productId);
        return true; // چون از دیتابیس حذف شده، موفقیت آمیز تلقی می‌شود حتی اگر S3 خطا دهد (با لاگ هشدار)
    }

    // متد کمکی برای حذف از S3
    private async Task<bool> DeleteS3ObjectAsync(string s3Key)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _minioSettings.BucketName,
                Key = s3Key
            };
            _logger.LogInformation("Deleting object from S3 with Key: {S3Key}", s3Key);
            await _s3Client.DeleteObjectAsync(deleteRequest);
            return true;
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, "S3 Error deleting object with Key: {S3Key}", s3Key);
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting object with Key: {S3Key}", s3Key);
            return false;
        }
    }

    // متد GetMediaUrls مانند قبل باقی می‌ماند
    public List<MediaInfoDto> GetMediaUrls(List<MediaInfo> mediaInfos)
    {
        // ... (پیاده سازی مانند قبل با ترکیب CDN BaseUrl و S3Key) ...
        if (mediaInfos == null) return new List<MediaInfoDto>();
        // ... (Select و OrderBy مانند قبل) ...
    }

}