
using MediatR;

public class CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryService _categoryService;

    public CreateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryDto = new CategoryDto
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder
        };

        var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);
        return createdCategory.Id;
    }
}
