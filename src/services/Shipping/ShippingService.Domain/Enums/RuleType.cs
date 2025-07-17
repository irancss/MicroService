namespace ShippingService.Domain.Enums;

public enum RuleType
{
    DayOfWeek = 1,
    TimeRange = 2,
    DateRange = 3,
    Product = 4,
    Category = 5,
    Weight = 6,
    Geography = 7
}

public enum DayOfWeek
{
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6
}
