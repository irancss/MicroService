
using AutoMapper;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;

public class categoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public categoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);
        await _categoryRepository.AddAsync(category);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(string categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        return _mapper.Map<CategoryDto?>(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(string categoryId, CategoryDto categoryDto)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
        if (existingCategory == null) return null;

        var updatedCategory = _mapper.Map(categoryDto, existingCategory);
        await _categoryRepository.UpdateAsync(updatedCategory);
        return _mapper.Map<CategoryDto>(updatedCategory);
    }

    public async Task<bool> DeleteCategoryAsync(string categoryId)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
        if (existingCategory == null) return false;

        await _categoryRepository.DeleteAsync(categoryId);
        return true;
    }
}