namespace BuildingBlocks.Domain.Interfaces;

/// <summary>
/// Interface for entities that can be published or unpublished.
/// Useful for scenarios like soft-publishing content.
/// </summary>
public interface IPublishable
{
    bool IsPublished { get; set; }
    DateTime? PublishedAt { get; set; }
}