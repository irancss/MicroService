using AutoMapper;
using VendorService.application.Interfaces;
using VendorService.Domain.Interfaces;
using VendorService.Domain.Models;

namespace VendorService.Application.Services;


public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;

    public VendorService(IVendorRepository vendorRepository, IMapper mapper)
    {
        _vendorRepository = vendorRepository;
        _mapper = mapper;
    }


    public async Task<VendorDto> GetVendorByIdAsync(string vendorId)
    {
        if (string.IsNullOrEmpty(vendorId))
        {
            throw new ArgumentException("Vendor ID cannot be null or empty.", nameof(vendorId));
        }

        var vendor = await _vendorRepository.GetByIdAsync(vendorId);
        if (vendor == null)
        {
            throw new KeyNotFoundException($"Vendor with ID {vendorId} not found.");
        }

        return _mapper.Map<VendorDto>(vendor);
    }

    public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync()
    {
        var vendors = await _vendorRepository.GetAllVendorsAsync();
        return _mapper.Map<List<VendorDto>>(vendors);
    }

    public async Task<string> CreateVendorAsync(VendorDto command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var vendorId = await _vendorRepository.AddAsync(_mapper.Map<Vendor>(command));
        return vendorId;
    }

    public async Task<VendorDto> UpdateVendorAsync(VendorDto command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var vendor = await _vendorRepository.UpdateAsync(_mapper.Map<Vendor>(command));
        return _mapper.Map<VendorDto>(vendor);
    }

    public async Task DeleteVendorAsync(string vendorId)
    {
        if (string.IsNullOrEmpty(vendorId))
        {
            throw new ArgumentException("Vendor ID cannot be null or empty.", nameof(vendorId));
        }

        await _vendorRepository.DeleteVendorAsync(vendorId);
    }

    public async Task<List<VendorDto>> GetTopRatedVendorsAsync(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        var vendors = await _vendorRepository.GetTopRatedVendorsAsync(count);
        return _mapper.Map<List<VendorDto>>(vendors);
    }

    public async Task<List<VendorDto>> SearchVendorsAsync(string searchTerms)
    {
        if (string.IsNullOrEmpty(searchTerms))
        {
            throw new ArgumentException("Search terms cannot be null or empty.", nameof(searchTerms));
        }
        var vendors = await _vendorRepository.SearchVendorsAsync(searchTerms);
        return _mapper.Map<List<VendorDto>>(vendors);
    }
}
