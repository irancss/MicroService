using VendorService.Domain.Models;

namespace VendorService.Domain.Interfaces
{
    public interface IVendorRepository
    {
        Task<Vendor?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<string> AddAsync(Vendor vendor, CancellationToken cancellationToken = default);
        Task<Vendor> UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default);

        Task<bool> DeleteVendorAsync(string vendorId, CancellationToken cancellationToken = default);
        Task<List<Vendor>> GetTopRatedVendorsAsync(int count , CancellationToken cancellationToken = default);
        Task<List<Vendor>> GetAllVendorsAsync(CancellationToken cancellationToken = default);

        Task<List<Vendor>> SearchVendorsAsync(string query, CancellationToken cancellationToken = default);
    }
}
