
using MediatR;
using Shopping.SharedKernel.Application.Abstractions;

namespace Shopping.SharedKernel.Application.CQRS.Queries;

public interface IQueryHandler<in TQuery, TResult> :
    IRequestHandler<TQuery, TResult> where TQuery :
    IQuery<TResult>
{

}