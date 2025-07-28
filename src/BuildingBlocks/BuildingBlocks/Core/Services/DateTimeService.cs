using BuildingBlocks.Core.Contracts;

namespace BuildingBlocks.Core.Services;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}