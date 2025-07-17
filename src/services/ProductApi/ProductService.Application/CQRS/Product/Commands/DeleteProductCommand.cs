namespace ProductService.Application.CQRS.Product.Commands;

public class DeleteProductCommand : IRequest<bool>
{
    public string Id { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductService _productService;

    public DeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.DeleteAsync(request.Id);
    }
}