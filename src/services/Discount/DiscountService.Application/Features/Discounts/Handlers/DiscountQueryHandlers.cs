using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using DiscountService.Application.DTOs;
using DiscountService.Application.Features.Discounts.Queries;
using DiscountService.Application.Interfaces;

namespace DiscountService.Application.Features.Discounts.Handlers;

/// <summary>
/// Handler for getting discount by ID
/// </summary>
public class GetDiscountByIdQueryHandler : IRequestHandler<GetDiscountByIdQuery, DiscountDto?>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDiscountByIdQueryHandler> _logger;

    public GetDiscountByIdQueryHandler(
        IDiscountRepository discountRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<GetDiscountByIdQueryHandler> logger)
    {
        _discountRepository = discountRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DiscountDto?> Handle(GetDiscountByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting discount by ID: {Id}", request.Id);

        var cacheKey = $"discount_{request.Id}";
        var cachedDiscount = await _cacheService.GetAsync<DiscountDto>(cacheKey);

        if (cachedDiscount != null)
        {
            return cachedDiscount;
        }

        var discount = await _discountRepository.GetByIdAsync(request.Id);
        if (discount == null)
        {
            return null;
        }

        var discountDto = _mapper.Map<DiscountDto>(discount);
        await _cacheService.SetAsync(cacheKey, discountDto, TimeSpan.FromMinutes(15));

        return discountDto;
    }
}

/// <summary>
/// Handler for getting paginated discounts
/// </summary>
public class GetDiscountsQueryHandler : IRequestHandler<GetDiscountsQuery, DiscountListResponse>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDiscountsQueryHandler> _logger;

    public GetDiscountsQueryHandler(
        IDiscountRepository discountRepository,
        IMapper mapper,
        ILogger<GetDiscountsQueryHandler> logger)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DiscountListResponse> Handle(GetDiscountsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting paginated discounts. Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var (discounts, totalCount) = await _discountRepository.GetPaginatedAsync(
            request.PageNumber, request.PageSize, request.SearchTerm);

        var discountDtos = _mapper.Map<List<DiscountDto>>(discounts);

        return new DiscountListResponse
        {
            Discounts = discountDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

/// <summary>
/// Handler for getting discount usage history
/// </summary>
public class GetDiscountUsageHistoryQueryHandler : IRequestHandler<GetDiscountUsageHistoryQuery, List<DiscountUsageHistoryDto>>
{
    private readonly IDiscountUsageHistoryRepository _usageHistoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDiscountUsageHistoryQueryHandler> _logger;

    public GetDiscountUsageHistoryQueryHandler(
        IDiscountUsageHistoryRepository usageHistoryRepository,
        IMapper mapper,
        ILogger<GetDiscountUsageHistoryQueryHandler> logger)
    {
        _usageHistoryRepository = usageHistoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<DiscountUsageHistoryDto>> Handle(GetDiscountUsageHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting usage history for discount: {DiscountId}", request.DiscountId);

        var usageHistory = await _usageHistoryRepository.GetByDiscountIdAsync(
            request.DiscountId, request.PageNumber, request.PageSize);

        return _mapper.Map<List<DiscountUsageHistoryDto>>(usageHistory);
    }
}

/// <summary>
/// Handler for getting user discount history
/// </summary>
public class GetUserDiscountHistoryQueryHandler : IRequestHandler<GetUserDiscountHistoryQuery, List<DiscountUsageHistoryDto>>
{
    private readonly IDiscountUsageHistoryRepository _usageHistoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserDiscountHistoryQueryHandler> _logger;

    public GetUserDiscountHistoryQueryHandler(
        IDiscountUsageHistoryRepository usageHistoryRepository,
        IMapper mapper,
        ILogger<GetUserDiscountHistoryQueryHandler> logger)
    {
        _usageHistoryRepository = usageHistoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<DiscountUsageHistoryDto>> Handle(GetUserDiscountHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting discount history for user: {UserId}", request.UserId);

        var usageHistory = await _usageHistoryRepository.GetByUserIdAsync(
            request.UserId, request.PageNumber, request.PageSize);

        return _mapper.Map<List<DiscountUsageHistoryDto>>(usageHistory);
    }
}
