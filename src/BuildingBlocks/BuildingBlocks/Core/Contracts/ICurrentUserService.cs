namespace BuildingBlocks.Core.Contracts;

/// <summary>
/// An abstraction for accessing information about the current user.
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}