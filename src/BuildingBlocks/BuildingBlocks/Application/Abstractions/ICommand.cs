using MediatR;

namespace BuildingBlocks.Application.Abstractions;

/// <summary>
/// A marker interface for a MediatR command that does not return a value.
/// Commands should modify state.
/// </summary>
public interface ICommand : IRequest
{
    /// <summary>
    /// A unique identifier for the command, useful for logging and tracing.
    /// </summary>
    Guid Id { get; }
}

/// <summary>
/// A marker interface for a MediatR command that returns a value.
/// Commands should modify state.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the command.</typeparam>
public interface ICommand<out TResult> : IRequest<TResult>
{
    /// <summary>
    /// A unique identifier for the command, useful for logging and tracing.
    /// </summary>
    Guid Id { get; }
}