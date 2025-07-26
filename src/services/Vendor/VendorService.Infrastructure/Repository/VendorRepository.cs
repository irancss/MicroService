using VendorService.Domain.Interfaces;
using VendorService.Domain.Models;

namespace VendorService.Infrastructure.Repository
{
    public class VendorRepository : IVendorRepository
    {
        public async Task<Vendor?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<string> AddAsync(Vendor vendor, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Vendor> UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteVendorAsync(string vendorId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Vendor>> GetTopRatedVendorsAsync(int count, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Vendor>> GetAllVendorsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Vendor>> SearchVendorsAsync(string query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
