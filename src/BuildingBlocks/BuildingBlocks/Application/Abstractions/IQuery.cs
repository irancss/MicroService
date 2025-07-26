using MediatR;

namespace BuildingBlocks.Application.Abstractions;

/// <summary>
/// A marker interface for a MediatR query.
/// Queries should return a result and not modify state.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
public interface IQuery<out TResult> : IRequest<TResult>
{
}