using MediatR;
using Microsoft.Extensions.Logging;
using ReportingService.Application.Interfaces;
using ReportingService.Domain.Entities;

namespace ReportingService.Application.Commands.ProcessOrderData;

public class ProcessOrderCompletedEventHandler : IRequestHandler<ProcessOrderCompletedEventCommand, ProcessOrderCompletedEventResponse>
{
    private readonly IReportingRepository _repository;
    private readonly ILogger<ProcessOrderCompletedEventHandler> _logger;

    public ProcessOrderCompletedEventHandler(
        IReportingRepository repository,
        ILogger<ProcessOrderCompletedEventHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProcessOrderCompletedEventResponse> Handle(
        ProcessOrderCompletedEventCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing order completed event for OrderId: {OrderId}", request.OrderEvent.OrderId);

            // 1. Ensure Date Dimension exists
            var dateDimension = await EnsureDateDimensionExists(request.OrderEvent.OrderDate, cancellationToken);

            // 2. Ensure Customer Dimension exists
            var customerDimension = await EnsureCustomerDimensionExists(request.OrderEvent, cancellationToken);

            // 3. Process each product in the order
            var orderFactId = Guid.Empty;
            foreach (var item in request.OrderEvent.Items)
            {
                // Ensure Product Dimension exists
                var productDimension = await EnsureProductDimensionExists(item, cancellationToken);

                // Create Order Fact
                var orderFact = new OrderFact(
                    orderId: request.OrderEvent.OrderId,
                    customerId: request.OrderEvent.CustomerId,
                    orderDate: request.OrderEvent.OrderDate,
                    totalAmount: item.Total,
                    currency: request.OrderEvent.Currency,
                    status: request.OrderEvent.Status,
                    totalItems: item.Quantity,
                    productDimensionId: productDimension.Id,
                    customerDimensionId: customerDimension.Id,
                    dateDimensionId: dateDimension.Id,
                    revenue: item.Total,
                    tax: request.OrderEvent.Tax * (item.Total / request.OrderEvent.TotalAmount), // Proportional tax
                    discount: request.OrderEvent.Discount * (item.Total / request.OrderEvent.TotalAmount) // Proportional discount
                );

                await _repository.AddOrderFactAsync(orderFact, cancellationToken);
                orderFactId = orderFact.Id;
            }

            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully processed order completed event for OrderId: {OrderId}", request.OrderEvent.OrderId);

            return new ProcessOrderCompletedEventResponse
            {
                Success = true,
                Message = "Order data processed successfully",
                OrderFactId = orderFactId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order completed event for OrderId: {OrderId}", request.OrderEvent.OrderId);
            
            return new ProcessOrderCompletedEventResponse
            {
                Success = false,
                Message = $"Error processing order data: {ex.Message}"
            };
        }
    }

    private async Task<DateDimension> EnsureDateDimensionExists(DateTime date, CancellationToken cancellationToken)
    {
        var existingDateDimension = await _repository.GetDateDimensionByDateAsync(date.Date, cancellationToken);
        if (existingDateDimension != null)
        {
            return existingDateDimension;
        }

        var dateDimension = new DateDimension(date.Date);
        await _repository.AddDateDimensionAsync(dateDimension, cancellationToken);
        return dateDimension;
    }

    private async Task<CustomerDimension> EnsureCustomerDimensionExists(
        Domain.Events.OrderCompletedEvent orderEvent, 
        CancellationToken cancellationToken)
    {
        var existingCustomerDimension = await _repository.GetCustomerDimensionByCustomerIdAsync(orderEvent.CustomerId, cancellationToken);
        if (existingCustomerDimension != null)
        {
            return existingCustomerDimension;
        }

        var customerDimension = new CustomerDimension(
            customerId: orderEvent.CustomerId,
            email: orderEvent.Customer.Email,
            firstName: orderEvent.Customer.FirstName,
            lastName: orderEvent.Customer.LastName,
            country: orderEvent.Customer.Country,
            city: orderEvent.Customer.City,
            segment: orderEvent.Customer.Segment,
            registrationDate: orderEvent.Customer.RegistrationDate
        );

        await _repository.AddCustomerDimensionAsync(customerDimension, cancellationToken);
        return customerDimension;
    }

    private async Task<ProductDimension> EnsureProductDimensionExists(
        Domain.Events.OrderItem item, 
        CancellationToken cancellationToken)
    {
        var existingProductDimension = await _repository.GetProductDimensionByProductIdAsync(item.ProductId, cancellationToken);
        if (existingProductDimension != null)
        {
            return existingProductDimension;
        }

        var productDimension = new ProductDimension(
            productId: item.ProductId,
            name: item.ProductName,
            category: item.Category,
            subCategory: item.SubCategory,
            brand: item.Brand,
            price: item.Price,
            currency: "USD" // Default currency, could be extracted from order
        );

        await _repository.AddProductDimensionAsync(productDimension, cancellationToken);
        return productDimension;
    }
}
