namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// A marker interface for an aggregate root.
/// An aggregate root is the entry point to an aggregate, a cluster of associated objects
/// that are treated as a single unit for data changes.
/// </summary>
public interface IAggregateRoot
{
    // This is a marker interface, so it has no members.
}