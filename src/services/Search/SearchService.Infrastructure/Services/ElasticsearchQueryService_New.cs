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
        var searchRequest = BuildSearchRequest(query, personalizationData);

        try
        {
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
        var searchRequest = BuildSuggestionRequest(query);

        try
        {
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
                    .Add("spell_check", su => su
                        .Text(query)
                        .Term(t => t
                            .Field(f => f.Name)
                            .SuggestMode(SuggestMode.Missing)
                            .Size(3))));

            var response = await _client.SearchAsync<ProductDocument>(searchRequest, cancellationToken);

            if (response.IsValidResponse && response.Suggest != null)
            {
                if (response.Suggest.TryGetValue("spell_check", out var suggest))
                {
                    foreach (var option in suggest.SelectMany(s => s.Options ?? Enumerable.Empty<SuggestOption>()))
                    {
                        suggestions.Add(option.Text);
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
                    .Add("auto_complete", su => su
                        .Prefix(prefix)
                        .Completion(c => c
                            .Field(f => f.NameSuggest)
                            .Size(size))));

            var response = await _client.SearchAsync<ProductDocument>(searchRequest, cancellationToken);

            if (response.IsValidResponse && response.Suggest != null)
            {
                if (response.Suggest.TryGetValue("auto_complete", out var suggest))
                {
                    foreach (var option in suggest.SelectMany(s => s.Options ?? Enumerable.Empty<SuggestOption>()))
                    {
                        suggestions.Add(option.Text);
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

    private SearchRequestDescriptor<ProductDocument> BuildSearchRequest(
        ProductSearchQuery query, 
        UserPersonalizationData? personalizationData)
    {
        var searchDescriptor = new SearchRequestDescriptor<ProductDocument>()
            .Index(ProductIndexName)
            .From(query.Pagination.PageNumber * query.Pagination.PageSize)
            .Size(query.Pagination.PageSize)
            .TrackTotalHits(Elastic.Clients.Elasticsearch.TrackHits.Bool(true));

        // Build query
        var queries = new List<Query>();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var multiMatchQuery = new MultiMatchQuery
            {
                Query = query.SearchTerm,
                Fields = new Field[]
                {
                    "name^3",
                    "description^2",
                    "brand",
                    "category",
                    "tags"
                },
                Type = TextQueryType.BestFields,
                Fuzziness = Fuzziness.Auto
            };
            queries.Add(multiMatchQuery);
        }

        // Add filters
        var filters = new List<Query>();

        if (query.Filters.Category.Any())
        {
            filters.Add(new TermsQuery
            {
                Field = "category.keyword",
                Terms = TermsQueryField.String(query.Filters.Category.ToArray())
            });
        }

        if (query.Filters.Brand.Any())
        {
            filters.Add(new TermsQuery
            {
                Field = "brand.keyword", 
                Terms = TermsQueryField.String(query.Filters.Brand.ToArray())
            });
        }

        if (query.Filters.MinPrice.HasValue || query.Filters.MaxPrice.HasValue)
        {
            var rangeQuery = new RangeQuery(new Field("price"))
            {
                Gte = query.Filters.MinPrice.HasValue ? FieldValue.Double(query.Filters.MinPrice.Value) : null,
                Lte = query.Filters.MaxPrice.HasValue ? FieldValue.Double(query.Filters.MaxPrice.Value) : null
            };
            filters.Add(rangeQuery);
        }

        if (query.Filters.MinRating.HasValue)
        {
            filters.Add(new RangeQuery(new Field("rating"))
            {
                Gte = FieldValue.Double(query.Filters.MinRating.Value)
            });
        }

        if (query.Filters.InStock)
        {
            filters.Add(new TermQuery("inStock") { Value = true });
        }

        // Combine queries and filters
        Query? mainQuery = null;

        if (queries.Any() && filters.Any())
        {
            mainQuery = new BoolQuery
            {
                Must = queries,
                Filter = filters
            };
        }
        else if (queries.Any())
        {
            mainQuery = queries.Count == 1 ? queries[0] : new BoolQuery { Must = queries };
        }
        else if (filters.Any())
        {
            mainQuery = new BoolQuery { Filter = filters };
        }
        else
        {
            mainQuery = new MatchAllQuery();
        }

        searchDescriptor.Query(mainQuery);

        // Add sorting
        ApplySorting(searchDescriptor, query.SortBy, query.SortDirection);

        // Add aggregations for facets
        searchDescriptor.Aggregations(a => a
            .Add("categories", agg => agg
                .Terms(t => t.Field("category.keyword").Size(10)))
            .Add("brands", agg => agg  
                .Terms(t => t.Field("brand.keyword").Size(10)))
            .Add("price_ranges", agg => agg
                .Range(r => r
                    .Field("price")
                    .Ranges(
                        ran => ran.To(50),
                        ran => ran.From(50).To(100),
                        ran => ran.From(100).To(200),
                        ran => ran.From(200))))
            .Add("avg_rating", agg => agg
                .Average(avg => avg.Field("rating"))));

        return searchDescriptor;
    }

    private void ApplySorting(SearchRequestDescriptor<ProductDocument> searchDescriptor, ProductSortBy sortBy, SortDirection direction)
    {
        var sortOrder = direction == SortDirection.Ascending ? SortOrder.Asc : SortOrder.Desc;

        searchDescriptor.Sort(s =>
        {
            switch (sortBy)
            {
                case ProductSortBy.Relevance:
                    s.Score(new ScoreSort { Order = sortOrder });
                    break;
                case ProductSortBy.Price:
                    s.Field("price", new FieldSort { Order = sortOrder });
                    break;
                case ProductSortBy.Rating:
                    s.Field("rating", new FieldSort { Order = sortOrder });
                    break;
                case ProductSortBy.CreatedDate:
                    s.Field("createdAt", new FieldSort { Order = sortOrder });
                    break;
                case ProductSortBy.Name:
                    s.Field("name.keyword", new FieldSort { Order = sortOrder });
                    break;
                case ProductSortBy.Popularity:
                    s.Field("popularity", new FieldSort { Order = sortOrder });
                    break;
                default:
                    s.Score(new ScoreSort { Order = SortOrder.Desc });
                    break;
            }
            return s;
        });
    }

    private SearchRequestDescriptor<ProductDocument> BuildSuggestionRequest(GetSearchSuggestionsQuery query)
    {
        return new SearchRequestDescriptor<ProductDocument>()
            .Index(ProductIndexName)
            .Size(0)
            .Suggest(s => s
                .Add("term_suggestions", su => su
                    .Text(query.Query)
                    .Term(t => t
                        .Field(f => f.Name)
                        .Size(query.MaxSuggestions)
                        .SuggestMode(SuggestMode.Popular)))
                .Add("completion_suggestions", su => su
                    .Prefix(query.Query)
                    .Completion(c => c
                        .Field(f => f.NameSuggest)
                        .Size(query.MaxSuggestions))));
    }

    private ProductSearchResponse MapSearchResponse(SearchResponse<ProductDocument> response, ProductSearchQuery query)
    {
        var products = response.Documents?.ToList() ?? new List<ProductDocument>();
        var totalHits = response.HitsMetadata?.Total?.Value ?? 0;

        var facets = MapAggregationsToFacets(response.Aggregations);

        return new ProductSearchResponse
        {
            Products = products,
            TotalHits = (int)totalHits,
            Pagination = new PaginationMetadata
            {
                PageNumber = query.Pagination.PageNumber,
                PageSize = query.Pagination.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalHits / query.Pagination.PageSize),
                HasNextPage = (query.Pagination.PageNumber + 1) * query.Pagination.PageSize < totalHits,
                HasPreviousPage = query.Pagination.PageNumber > 0
            },
            Facets = facets,
            SearchMetadata = new SearchMetadata
            {
                SearchTerm = query.SearchTerm,
                ProcessingTimeMs = (int)(response.Took ?? 0),
                SuggestedCorrections = new List<string>()
            }
        };
    }

    private GetSearchSuggestionsResponse MapSuggestionResponse(SearchResponse<ProductDocument> response, GetSearchSuggestionsQuery query)
    {
        var suggestions = new List<string>();

        if (response.Suggest != null)
        {
            // Process term suggestions
            if (response.Suggest.TryGetValue("term_suggestions", out var termSuggestions))
            {
                foreach (var suggestion in termSuggestions)
                {
                    foreach (var option in suggestion.Options ?? Enumerable.Empty<SuggestOption>())
                    {
                        suggestions.Add(option.Text);
                    }
                }
            }

            // Process completion suggestions
            if (response.Suggest.TryGetValue("completion_suggestions", out var completionSuggestions))
            {
                foreach (var suggestion in completionSuggestions)
                {
                    foreach (var option in suggestion.Options ?? Enumerable.Empty<SuggestOption>())
                    {
                        suggestions.Add(option.Text);
                    }
                }
            }
        }

        return new GetSearchSuggestionsResponse
        {
            Suggestions = suggestions.Distinct().Take(query.MaxSuggestions).ToList(),
            Query = query.Query
        };
    }

    private List<SearchFacet> MapAggregationsToFacets(IReadOnlyDictionary<string, IAggregate>? aggregations)
    {
        var facets = new List<SearchFacet>();

        if (aggregations == null) return facets;

        // Categories facet
        if (aggregations.TryGetValue("categories", out var categoriesAgg) && categoriesAgg is StringTermsAggregate categoriesTerms)
        {
            var categoryFacet = new SearchFacet
            {
                Name = "Category",
                Type = FacetType.Terms,
                Values = categoriesTerms.Buckets.Select(b => new FacetValue
                {
                    Value = b.Key.ToString(),
                    Count = (int)(b.DocCount ?? 0),
                    DisplayName = FormatCategoryDisplay(b.Key.ToString())
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
                Type = FacetType.Terms,
                Values = brandsTerms.Buckets.Select(b => new FacetValue
                {
                    Value = b.Key.ToString(),
                    Count = (int)(b.DocCount ?? 0),
                    DisplayName = b.Key.ToString()
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
                Type = FacetType.Range,
                Values = priceRanges.Buckets.Select(b => new FacetValue
                {
                    Value = b.Key?.ToString() ?? "",
                    Count = (int)(b.DocCount ?? 0),
                    DisplayName = FormatRangeDisplay(b.Key?.ToString() ?? "", b.From, b.To)
                }).ToList()
            };
            facets.Add(priceFacet);
        }

        return facets;
    }

    private string FormatCategoryDisplay(string category)
    {
        return category.Replace("-", " ").Replace("_", " ");
    }

    private string FormatRangeDisplay(string key, double? from, double? to)
    {
        if (from.HasValue && to.HasValue)
            return $"${from:F0} - ${to:F0}";
        if (from.HasValue)
            return $"${from:F0}+";
        if (to.HasValue)
            return $"Under ${to:F0}";
        return key;
    }
}
