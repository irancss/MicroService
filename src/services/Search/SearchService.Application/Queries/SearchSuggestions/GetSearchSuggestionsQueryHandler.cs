using MediatR;
using Microsoft.Extensions.Logging;
using SearchService.Application.Services;
using System.Diagnostics;

namespace SearchService.Application.Queries.SearchSuggestions;

public class GetSearchSuggestionsQueryHandler : IRequestHandler<GetSearchSuggestionsQuery, GetSearchSuggestionsResponse>
{
    private readonly IElasticsearchQueryService _elasticsearchQueryService;
    private readonly ILogger<GetSearchSuggestionsQueryHandler> _logger;

    public GetSearchSuggestionsQueryHandler(
        IElasticsearchQueryService elasticsearchQueryService,
        ILogger<GetSearchSuggestionsQueryHandler> logger)
    {
        _elasticsearchQueryService = elasticsearchQueryService;
        _logger = logger;
    }

    public async Task<GetSearchSuggestionsResponse> Handle(GetSearchSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Getting search suggestions for query: {Query}", request.Query);

            var response = await _elasticsearchQueryService.GetSuggestionsAsync(request, cancellationToken);

            stopwatch.Stop();
            response.QueryTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            _logger.LogInformation("Suggestions retrieved in {ElapsedMs}ms, found {Count} suggestions", 
                response.QueryTimeMs, response.Suggestions.Count);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error retrieving search suggestions for query: {Query}", request.Query);
            
            return new GetSearchSuggestionsResponse
            {
                Suggestions = new List<Domain.ValueObjects.SearchSuggestion>(),
                QueryTimeMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
    }
}
