using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProductDbContext _context;
        private bool _disposed;
        public IProductRepository Products { get; }
        public IQuestionRepository Questions { get; }
        public IAnswerRepository Answers { get; }
        public IBrandRepository Brands { get; }
        public ICategoryRepository Categories { get; }
        public IProductImage ProductImage { get; }
        public ITagRepository Tags { get; }
        public IProductTagRepository ProductTagRepository { get;  }
        public IProductVariantStock ProductVariantStock { get; }



        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public UnitOfWork(ProductDbContext context)
        {
            _context = context;

            Products = new ProductRepository(_context);
            Questions = new QuestionRepository(_context);
            Answers = new AnswerRepository(_context);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
