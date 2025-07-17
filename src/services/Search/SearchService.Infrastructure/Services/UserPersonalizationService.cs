using Microsoft.Extensions.Logging;
using SearchService.Domain.Entities;
using SearchService.Domain.Interfaces;
using System.Text.Json;

namespace SearchService.Infrastructure.Services;

/// <summary>
/// HTTP-based implementation of user personalization service
/// In a real scenario, this would call a dedicated UserProfile or Marketing service
/// </summary>
public class UserPersonalizationService : IUserPersonalizationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserPersonalizationService> _logger;

    public UserPersonalizationService(HttpClient httpClient, ILogger<UserPersonalizationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserPersonalizationData?> GetUserPersonalizationAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching personalization data for user: {UserId}", userId);

            // In a real implementation, this would call an external service
            // For now, we'll return some mock data or implement a simple cache
            var response = await _httpClient.GetAsync($"/api/users/{userId}/personalization", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var personalizationData = JsonSerializer.Deserialize<UserPersonalizationData>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Successfully retrieved personalization data for user: {UserId}", userId);
                return personalizationData;
            }

            _logger.LogWarning("Failed to retrieve personalization data for user: {UserId}, Status: {StatusCode}", 
                userId, response.StatusCode);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "HTTP error while fetching personalization data for user: {UserId}", userId);
            return GetFallbackPersonalizationData(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching personalization data for user: {UserId}", userId);
            return null;
        }
    }

    public async Task UpdateUserPersonalizationAsync(string userId, UserPersonalizationData data, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating personalization data for user: {UserId}", userId);

            var content = JsonSerializer.Serialize(data);
            var httpContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/users/{userId}/personalization", httpContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully updated personalization data for user: {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Failed to update personalization data for user: {UserId}, Status: {StatusCode}", 
                    userId, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating personalization data for user: {UserId}", userId);
        }
    }

    private UserPersonalizationData? GetFallbackPersonalizationData(string userId)
    {
        // Return some basic personalization data as fallback
        // In a real scenario, this might come from a local cache or be computed from recent search history
        _logger.LogInformation("Using fallback personalization data for user: {UserId}", userId);

        return new UserPersonalizationData
        {
            UserId = userId,
            PreferredCategories = new Dictionary<string, double>
            {
                { "Electronics", 1.2 },
                { "Clothing", 1.1 }
            },
            PreferredBrands = new Dictionary<string, double>
            {
                { "Samsung", 1.3 },
                { "Nike", 1.2 }
            },
            LastUpdated = DateTime.UtcNow.AddDays(-1)
        };
    }
}
