namespace ProductService.Domain.Interfaces // Or ProductService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; } // Assuming you have IProductRepository
        IQuestionRepository Questions { get; }
        IAnswerRepository Answers { get; }
        IBrandRepository Brands { get; }
        ICategoryRepository Categories { get; }
        ITagRepository Tags { get; }
        IProductTagRepository ProductTagRepository { get; }

        Task<int> CommitAsync(); // Or SaveChangesAsync or CompleteAsync
    }
}