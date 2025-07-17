
using AutoMapper;
using MediatR;

public class CreateProductCommand : IRequest<string>
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }

    // Additional properties can be added as needed
}
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
{
    private IProductService _productService;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}