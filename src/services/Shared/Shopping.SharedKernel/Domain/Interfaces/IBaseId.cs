
namespace Shopping.SharedKernel.Domain.Interfaces;
public interface IBaseId<T> where T : class
{
    public T Id { get; set; }
}