using BuildingBlocks.Abstractions;
using BuildingBlocks.Infrastructure;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;
using CustomerService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _dbContext;

        // از یک نمونه از ریپازیتوری‌های عمومی BuildingBlocks برای انجام کارهای تکراری استفاده می‌کنیم.
        private readonly IRepository<Customer> _baseRepository;
        private readonly IRepositoryAsync<Customer> _baseRepositoryAsync;

        // به جای UnitOfWork یا DbContext، یک IRepositoryFactory تزریق می‌کنیم که از BuildingBlocks می‌آید.
        // این روش تمیزتر است، اما برای سادگی فعلاً از تزریق مستقیم DbContext استفاده می‌کنیم
        // تا بتوانیم کوئری‌های خاص Customer را بزنیم.
        public CustomerRepository(CustomerDbContext dbContext)
        {
            // اینجا هم استفاده از dbContext صحیح است، چون ریپازیتوری باید به context دسترسی داشته باشد.
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            // نمونه‌های ریپازیتوری عمومی را با همین context می‌سازیم.
            _baseRepositoryAsync = new RepositoryAsync<Customer>(dbContext);
        }


        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .AsNoTracking() // کوئری‌های خواندنی باید NoTracking باشند.
                .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
        }

        public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Customer?> GetByMobileAsync(string mobileNumber, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.PhoneNumber == mobileNumber, cancellationToken);
        }

        // این کوئری‌ها چون خاص هستند، باید در همین ریپازیتوری پیاده‌سازی شوند.
        public async Task<Customer?> GetByAddressAsync(string streetAddress, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Addresses.Any(a => a.Street == streetAddress), cancellationToken);
        }

        public async Task<Customer?> GetByCityAsync(string city, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Addresses.Any(a => a.City == city), cancellationToken);
        }

        public async Task<Guid> InsertAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
