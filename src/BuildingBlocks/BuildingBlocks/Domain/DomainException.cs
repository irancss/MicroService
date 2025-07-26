namespace BuildingBlocks.Domain;

/// <summary>
/// Base class for custom exceptions defined in the domain layer.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}