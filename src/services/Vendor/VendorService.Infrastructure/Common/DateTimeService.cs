namespace VendorService.Infrastructure.Common;

  public class DateTimeService : IDateTime
  {
      public DateTime Now => DateTime.Now;
  }