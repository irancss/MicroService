namespace ReportingService.Domain.Entities;

/// <summary>
/// Date dimension table for star schema - supports time-based analytics
/// </summary>
public class DateDimension : BaseEntity
{
    public DateTime Date { get; private set; }
    public int Year { get; private set; }
    public int Month { get; private set; }
    public int Day { get; private set; }
    public int Quarter { get; private set; }
    public int WeekOfYear { get; private set; }
    public string DayOfWeek { get; private set; } = string.Empty;
    public string MonthName { get; private set; } = string.Empty;
    public bool IsWeekend { get; private set; }
    public bool IsHoliday { get; private set; }

    private DateDimension() { } // EF Core

    public DateDimension(DateTime date, bool isHoliday = false)
    {
        Date = date.Date;
        Year = date.Year;
        Month = date.Month;
        Day = date.Day;
        Quarter = (date.Month - 1) / 3 + 1;
        WeekOfYear = GetWeekOfYear(date);
        DayOfWeek = date.DayOfWeek.ToString();
        MonthName = date.ToString("MMMM");
        IsWeekend = date.DayOfWeek == System.DayOfWeek.Saturday || date.DayOfWeek == System.DayOfWeek.Sunday;
        IsHoliday = isHoliday;
    }

    private static int GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        return culture.Calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
    }

    public void MarkAsHoliday()
    {
        IsHoliday = true;
        SetUpdatedAt();
    }
}
