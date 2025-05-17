using Microsoft.Extensions.Options;
using ProductApi.Infrastructure.Settings;
using ProductApi.Models.Dtos;
using ProductApi.Models.Entities;
using ProductApi.Repositories;

namespace ProductApi.Services;

public class MediaService : IMediaService
{
    private readonly MinioSettings _minioSettings;
    private readonly CdnSettings _cdnSettings;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<MediaService> _logger;

    public MediaService(
        IOptions<MinioSettings> minioOptions,
        IOptions<CdnSettings> cdnOptions,
        IProductRepository productRepository,
        ILogger<MediaService> logger)
    {
        _minioSettings = minioOptions.Value;
        _cdnSettings = cdnOptions.Value;
        _productRepository = productRepository;
        _logger = logger;
    }

    public Task<PresignedUrlResponse> GeneratePresignedUploadUrlAsync(string productId, string fileName, string contentType)
    {
        throw new NotImplementedException("S3/Amazon functionality removed. Implement MinIO SDK or HTTP API here.");
    }

    public Task<bool> ConfirmMediaUploadAsync(string productId, string s3Key, string mediaType, string altText, int order)
    {
        throw new NotImplementedException("S3/Amazon functionality removed. Implement MinIO SDK or HTTP API here.");
    }

    public Task<bool> DeleteMediaAsync(string productId, string mediaId)
    {
        throw new NotImplementedException("S3/Amazon functionality removed. Implement MinIO SDK or HTTP API here.");
    }

    public List<MediaInfoDto> GetMediaUrls(List<MediaInfo> mediaInfos)
    {
        if (mediaInfos == null) return new List<MediaInfoDto>();
        // Implement as before, combining CDN BaseUrl and S3Key
        return mediaInfos
            .OrderBy(m => m.Order)
            .Select(m => new MediaInfoDto
            {
                Id = m.Id,
                Url = $"{_cdnSettings.BaseUrl.TrimEnd('/')}/",
                MediaType = m.MediaType,
                AltText = m.AltText,
                Order = m.Order
            })
            .ToList();
    }
}
