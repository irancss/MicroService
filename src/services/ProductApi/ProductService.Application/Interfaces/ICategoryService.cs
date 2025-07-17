
public interface ICategoryService
{
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    Task<CategoryDto?> GetCategoryByIdAsync(string categoryId);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> UpdateCategoryAsync(string categoryId, CategoryDto categoryDto);
    Task<bool> DeleteCategoryAsync(string categoryId);
}