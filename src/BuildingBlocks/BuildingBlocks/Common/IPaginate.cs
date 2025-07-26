namespace BuildingBlocks.Common;

/// <summary>
/// Represents a paginated list of items.
/// [نکته] این اینترفیس به عنوان استاندارد واحد برای صفحه‌بندی در لایه داده استفاده می‌شود.
/// </summary>
public interface IPaginate<T>
{
    int From { get; }
    int Index { get; }
    int Size { get; }
    int Count { get; }
    int Pages { get; }
    IList<T> Items { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
}