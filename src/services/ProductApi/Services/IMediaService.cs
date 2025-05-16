using ProductApi.Models.Dtos;
using ProductApi.Models.Entities;

namespace ProductApi.Services
{
    public interface IMediaService
    {
        // تولید URL امضا شده برای آپلود مستقیم فایل توسط کلاینت به S3
        Task<PresignedUrlResponse> GeneratePresignedUploadUrlAsync(string productId, string fileName, string contentType);
        // ثبت موفقیت آمیز آپلود و افزودن اطلاعات به محصول در MongoDB
        Task<bool> ConfirmMediaUploadAsync(string productId, string s3Key, string mediaType, string altText, int order);
        // حذف فایل از S3 و اطلاعات آن از محصول در MongoDB
        Task<bool> DeleteMediaAsync(string productId, string mediaId);
        // تبدیل S3 Key ها به URL های CDN برای نمایش در DTO
        //List<MediaInfoDto> GetMediaUrls(List<MediaInfo> mediaInfos);
    }

    public class PresignedUrlResponse
    {
        public string UploadUrl { get; set; } // URL برای PUT request به S3
        public string S3Key { get; set; } // کلید فایل در S3 که بعدا باید ثبت شود
    }
}
