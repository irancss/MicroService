using BuildingBlocks.Core.Contracts;

namespace BuildingBlocks.Core.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}