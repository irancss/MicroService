
using MediatR;

public class UpdateCategoryCommand : IRequest<CategoryDto>
{
    public string CategoryId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryService _categoryService;

    public UpdateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryDto = new CategoryDto
        {
            //Id = request.CategoryId,
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder
        };

        return await _categoryService.UpdateCategoryAsync(request.CategoryId, categoryDto);
    }
}
