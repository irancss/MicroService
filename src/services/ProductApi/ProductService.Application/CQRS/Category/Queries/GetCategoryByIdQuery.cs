
using AutoMapper;
using MediatR;

public class GetCategoryByIdQuery : IRequest<CategoryDto?>
{
    public string CategoryId { get; }

    public GetCategoryByIdQuery(string categoryId)
    {
        CategoryId = categoryId;
    }
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
        return _mapper.Map<CategoryDto?>(category);
    }
}
