

using AutoMapper;
using MediatR;

public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
{
}

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }
}
