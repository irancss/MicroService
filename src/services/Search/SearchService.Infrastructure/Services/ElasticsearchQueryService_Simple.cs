using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using SearchService.Application.Queries.ProductSearch;
using SearchService.Application.Queries.SearchSuggestions;
using SearchService.Application.Services;
using SearchService.Domain.Entities;
using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Infrastructure.Services;

public class ElasticsearchQueryService : IElasticsearchQueryService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchQueryService> _logger;
    private const string ProductIndexName = "products";

    public ElasticsearchQueryService(ElasticsearchClient client, ILogger<ElasticsearchQueryService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ProductSearchResponse> SearchProductsAsync(
        ProductSearchQuery query, 
        UserPersonalizationData? personalizationData, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var searchRequest = new SearchRequestDescriptor<ProductDocument>()
                .Index(ProductIndexName)
                .From((query.Page - 1) * query.Size)
                .Size(query.Size);

            // Build main query
            if (!string.IsNullOrWhiteSpace(query.Query))
            {
                searchRequest.Query(q => q
                    .MultiMatch(mm => mm
                        .Query(query.Query)
                        .Fields(new[] { "name^3", "description^2", "brand", "category", "tags" })
                        .Type(TextQueryType.BestFields)
                        .Fuzziness(Fuzziness.One)
                    )
                );
            }
            else
            {
                searchRequest.Query(q => q.MatchAll());
            }

            // Add aggregations if facets are requested
            if (query.IncludeFacets)
            {
                searchRequest.Aggregations(aggs => aggs
                    .Terms("categories", t => t.Field("category.keyword").Size(10))
                    .Terms("brands", t => t.Field("brand.keyword").Size(10))
                    .Range("price_ranges", r => r
                        .Field("price")
                        .Ranges(
                            range => range.To(50),
                            range => range.From(50).To(100),
                            range => range.From(100).To(200),
                            range => range.From(200)
                        )
                    )
                );
            }

            var response = await _client.SearchAsync<ProductDocument>(searchRequest, cancellationToken);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Search request failed: {Error}", response.ElasticsearchServerError?.Error?.Reason);
                return new ProductSearchResponse();
            }

            return MapSearchResponse(response, query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during product search");
            return new ProductSearchResponse();
        }
    }

    public async Task<GetSearchSuggestionsResponse> GetSuggestionsAsync(
        GetSearchSuggestionsQuery query, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var searchRequest = new SearchRequestDescriptor<ProductDocument>()
                .Index(ProductIndexName)
                .Size(0)
                .Suggest(s => s
                    .Term("spell_suggestions", term => term
                        .Text(query.Query)
                        .Field("name")
                        .Size(query.MaxSuggestions)
                    )
                );

            var response = await _client.SearchAsync<ProductDocument>(searchRequest, cancellationToken);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Suggestion request failed: {Error}", response.ElasticsearchServerError?.Error?.Reason);
                return new GetSearchSuggestionsResponse();
            }

            return MapSuggestionResponse(response, query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during suggestion search");
            return new GetSearchSuggestionsResponse();
        }
    }

    public async Task<List<string>> GetSpellCorrectionSuggestionsAsync(string query, CancellationToken cancellationToken = default)
    {
        var suggestions = new List<string>();

        try
        {
            var searchRequest = new SearchRequestDescriptor<ProductDocument>()
                .Index(ProductIndexName)
                .Size(0)
                .Suggest(s => s
                    .Term("spell_check", term => term
                        .Text(query)
                        .Field("name")
                        .SuggestMode(SuggestMode.Missing)
                        .Size(3)
                    )
                );

            var response = await _client.SearchAsync<ProductDocument>(searchRequest, cancellationToken);

            if (response.IsValidResponse && response.Suggest != null)
            {
                if (response.Suggest.TryGetValue("spell_check", out var suggest))
                {
                    foreach (var suggestion in suggest)
                    {
                        if (suggestion.Options != null)
                        {
                            foreach (var option in suggestion.Options)
                            {
                                suggestions.Add(option.Text);
                            }
                        }
                    }
                }
            }

            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during spell correction");
            return suggestions;
        }
    }

    public async Task<List<string>> GetAutoCompleteSuggestionsAsync(string prefix, int size = 10, CancellationToken cancellationToken = default)
    {
        var suggestions = new List<string>();

        try
        {
            var searchRequest = new SearchRequestDescriptor<ProductDocument>()
                .Index(ProductIndexName)
                .Size(0)
                .Suggest(s => s
                    .Completion("auto_complete", completion => completion
                        .Prefix(prefix)
                        .Field("nameSuggest")
                        .Size(size)
                    )
                );

            var response = await _client.SearchAsync<ProductDocument>(searchRequest, cancellationToken);

            if (response.IsValidResponse && response.Suggest != null)
            {
                if (response.Suggest.TryGetValue("auto_complete", out var suggest))
                {
                    foreach (var suggestion in suggest)
                    {
                        if (suggestion.Options != null)
                        {
                            foreach (var option in suggestion.Options)
                            {
                                suggestions.Add(option.Text);
                            }
                        }
                    }
                }
            }

            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during auto-complete");
            return suggestions;
        }
    }

    private ProductSearchResponse MapSearchResponse(SearchResponse<ProductDocument> response, ProductSearchQuery query)
    {
        var products = response.Documents?.ToList() ?? new List<ProductDocument>();
        var totalHits = response.HitsMetadata?.Total?.Value ?? 0;

        var facets = new List<SearchFacet>();
        if (query.IncludeFacets && response.Aggregations != null)
        {
            facets = MapAggregationsToFacets(response.Aggregations);
        }

        return new ProductSearchResponse
        {
            Products = products,
            TotalResults = totalHits,
            Pagination = new PaginationInfo
            {
                CurrentPage = query.Page,
                PageSize = query.Size,
                TotalPages = (int)Math.Ceiling((double)totalHits / query.Size),
                TotalItems = (int)totalHits
            },
            Facets = facets,
            Suggestions = new List<SearchSuggestion>(),
            QueryTimeMs = response.Took ?? 0,
            HasPersonalization = false
        };
    }

    private GetSearchSuggestionsResponse MapSuggestionResponse(SearchResponse<ProductDocument> response, GetSearchSuggestionsQuery query)
    {
        var suggestions = new List<SearchSuggestion>();

        if (response.Suggest != null)
        {
            if (response.Suggest.TryGetValue("spell_suggestions", out var spellSuggestions))
            {
                foreach (var suggestion in spellSuggestions)
                {
                    if (suggestion.Options != null)
                    {
                        foreach (var option in suggestion.Options)
                        {
                            suggestions.Add(new SearchSuggestion
                            {
                                Text = option.Text,
                                Score = option.Score ?? 0,
                                Frequency = (int)(option.Freq ?? 0)
                            });
                        }
                    }
                }
            }
        }

        return new GetSearchSuggestionsResponse
        {
            Suggestions = suggestions.Take(query.MaxSuggestions).ToList()
        };
    }

    private List<SearchFacet> MapAggregationsToFacets(IReadOnlyDictionary<string, IAggregate> aggregations)
    {
        var facets = new List<SearchFacet>();

        // Categories facet
        if (aggregations.TryGetValue("categories", out var categoriesAgg) && categoriesAgg is StringTermsAggregate categoriesTerms)
        {
            var categoryFacet = new SearchFacet
            {
                Name = "Category",
                Items = categoriesTerms.Buckets.Select(b => new FacetItem
                {
                    Name = b.Key.ToString(),
                    Count = (int)(b.DocCount ?? 0)
                }).ToList()
            };
            facets.Add(categoryFacet);
        }

        // Brands facet
        if (aggregations.TryGetValue("brands", out var brandsAgg) && brandsAgg is StringTermsAggregate brandsTerms)
        {
            var brandFacet = new SearchFacet
            {
                Name = "Brand",
                Items = brandsTerms.Buckets.Select(b => new FacetItem
                {
                    Name = b.Key.ToString(),
                    Count = (int)(b.DocCount ?? 0)
                }).ToList()
            };
            facets.Add(brandFacet);
        }

        // Price ranges facet
        if (aggregations.TryGetValue("price_ranges", out var priceAgg) && priceAgg is RangeAggregate priceRanges)
        {
            var priceFacet = new SearchFacet
            {
                Name = "Price Range",
                Items = priceRanges.Buckets.Select(b => new FacetItem
                {
                    Name = FormatPriceRange(b.From, b.To),
                    Count = (int)(b.DocCount ?? 0)
                }).ToList()
            };
            facets.Add(priceFacet);
        }

        return facets;
    }

    private string FormatPriceRange(double? from, double? to)
    {
        if (from.HasValue && to.HasValue)
            return $"${from:F0} - ${to:F0}";
        if (from.HasValue)
            return $"${from:F0}+";
        if (to.HasValue)
            return $"Under ${to:F0}";
        return "All Prices";
    }
}
