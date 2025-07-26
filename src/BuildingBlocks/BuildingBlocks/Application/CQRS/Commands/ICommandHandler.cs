using MediatR;
using BuildingBlocks.Application.Abstractions;

namespace BuildingBlocks.Application.CQRS.Commands;

/// <summary>
/// Defines a handler for a command that returns a result.
/// </summary>
public interface ICommandHandler<in TCommand, TResult> :
    IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
}

/// <summary>
/// Defines a handler for a command that does not return a result.
/// </summary>
public interface ICommandHandler<in TCommand> :
    IRequestHandler<TCommand> where TCommand : ICommand
{
}