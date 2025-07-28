using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Common;
using CustomerService.Application.Dtos;
using CustomerService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Queries.GetCustomers
{
    public record GetCustomersQuery(int PageIndex, int PageSize, string? SearchTerm) : IQuery<IPaginate<CustomerDto>>;
    public class GetCustomersQueryHandler : IQueryHandler<GetCustomersQuery, IPaginate<CustomerDto>>
    {
        private readonly CustomerDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetCustomersQueryHandler(CustomerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IPaginate<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Customers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(c =>
                    EF.Functions.ILike(c.FirstName, $"%{request.SearchTerm}%") ||
                    EF.Functions.ILike(c.LastName, $"%{request.SearchTerm}%") ||
                    EF.Functions.ILike(c.Email, $"%{request.SearchTerm}%"));
            }

            // استفاده از ProjectTo برای بهینه‌سازی کوئری و جلوگیری از load کردن کل موجودیت
            var paginatedResult = await query
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .ToPaginateAsync(request.PageIndex, request.PageSize, 0, cancellationToken);

            return paginatedResult;
        }
    }
}
