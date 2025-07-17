namespace Shopping.SharedKernel.Core.Contracts
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}