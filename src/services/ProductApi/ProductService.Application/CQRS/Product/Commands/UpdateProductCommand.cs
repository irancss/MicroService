using MediatR;

namespace ProductService.Application.CQRS.Product.Commands;


public class UpdateProductCommand : IRequest<bool>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }

    public UpdateProductCommand(string id, string name, string slug, string description, decimal price, int stockQuantity, bool isActive, int displayOrder)
    {
        Id = id;
        Name = name;
        Slug = slug;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        IsActive = isActive;
        DisplayOrder = displayOrder;
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductService _productService;

    public UpdateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.UpdateAsync(request.Id, request.Name, request.Slug, request.Description, request.Price, request.StockQuantity, request.IsActive, request.DisplayOrder);
    }
}