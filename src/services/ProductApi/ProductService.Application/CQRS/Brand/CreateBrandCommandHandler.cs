using ProductService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Application.CQRS.Commands;

namespace ProductService.Application.CQRS.Brand
{
    public class CreateBrandCommandHandler : ICommandHandler<CreateBrandCommand, Guid>
    {
        private readonly IBrandRepository _brandRepository;

        public CreateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<Guid> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = Brand.Create(request.Name, request.Description);

            await _brandRepository.AddAsync(brand);

            // SaveChangesAsync توسط TransactionBehavior به صورت خودکار فراخوانی می‌شود.

            return brand.Id;
        }
    }
}
