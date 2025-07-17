namespace Shopping.SharedKernel.Core.Services;
using Shopping.SharedKernel.Core.Contracts;
public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}