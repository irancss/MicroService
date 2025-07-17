namespace Shopping.SharedKernel.Domain.Interfaces;

public interface IPublishable
{
    bool IsPublished { get; set; }
    DateTime? PublishedAt { get; set; }
}