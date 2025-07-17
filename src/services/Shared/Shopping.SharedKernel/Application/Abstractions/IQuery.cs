
using MediatR;

namespace Shopping.SharedKernel.Application.Abstractions;

public interface IQuery<out TResult> : IRequest<TResult>
{

}