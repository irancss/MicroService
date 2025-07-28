namespace BuildingBlocks.Core.Contracts;

/// <summary>
/// An abstraction for getting the current date and time.
/// This makes code that depends on time more testable.
/// </summary>
public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}