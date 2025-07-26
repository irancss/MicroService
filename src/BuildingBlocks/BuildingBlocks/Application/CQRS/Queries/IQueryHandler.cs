using MediatR;
using BuildingBlocks.Application.Abstractions;

namespace BuildingBlocks.Application.CQRS.Queries;

/// <summary>
/// Defines a handler for a query.
/// </summary>
/// <typeparam name="TQuery">The type of the query being handled.</typeparam>
/// <typeparam name="TResult">The type of the result from the handler.</typeparam>
public interface IQueryHandler<in TQuery, TResult> :
    IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
}