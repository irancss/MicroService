// filepath: c:\Users\SHIELD SYSTEM\source\repos\MicroService\src\services\ProductApi\ProductService.Core\Exceptions\DomainException.cs
using System;
using BuildingBlocks.Domain;

namespace ProductService.Domain.Exceptions // Or ProductService.Domain.Exceptions
{

    public class ProductNotFoundException : DomainException
    {
        public ProductNotFoundException(int productId)
            : base($"Product with ID '{productId}' was not found.")
        {
        }

         public ProductNotFoundException(string message)
            : base(message)
        {
        }

        public ProductNotFoundException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}