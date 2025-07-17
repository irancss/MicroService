using VendorService.Domain.Models;

namespace VendorService.application.Interfaces;

public interface IVendorService
{
    Task<VendorDto> GetVendorByIdAsync(string vendorId);
    Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
    Task<string> CreateVendorAsync(VendorDto command);
    Task<VendorDto> UpdateVendorAsync(VendorDto command);
    Task DeleteVendorAsync(string vendorId);

    Task<List<VendorDto>> GetTopRatedVendorsAsync(int count);
    Task<List<VendorDto>> SearchVendorsAsync(string searchTerms); 
}