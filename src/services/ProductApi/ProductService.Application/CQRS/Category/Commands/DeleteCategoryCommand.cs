
using MediatR;

public class DeleteCategoryCommand : IRequest<bool>
{
    public string CategoryId { get; set; }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryService _categoryService;

    public DeleteCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.CategoryId))
        {
            throw new ArgumentException("Category ID cannot be null or empty.", nameof(request.CategoryId));
        }

        return await _categoryService.DeleteCategoryAsync(request.CategoryId);
    }
}