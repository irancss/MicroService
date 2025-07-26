using BuildingBlocks.Application.Abstractions;

namespace BuildingBlocks.Application.CQRS.Commands;

/// <summary>
/// [اصلاح شد] Base class for commands that do not return a value.
/// Automatically assigns a unique ID to each command instance.
/// The Id is now an init-only property to work better with records.
/// </summary>
public abstract record CommandBase : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

/// <summary>
/// [اصلاح شد] Base class for commands that return a value.
/// Automatically assigns a unique ID to each command instance.
/// The Id is now an init-only property to work better with records.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public abstract record CommandBase<TResult> : ICommand<TResult>
{
    public Guid Id { get; init; } = Guid.NewGuid();
}