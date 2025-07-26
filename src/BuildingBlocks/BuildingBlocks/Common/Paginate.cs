namespace BuildingBlocks.Common;

/// <summary>
/// A paginated list implementation that supports converting item types.
/// </summary>
internal class Paginate<TSource, TResult> : IPaginate<TResult>
{
    public Paginate(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int index, int size, int from)
    {
        if (from > index) throw new ArgumentException($"From: {from} > Index: {index}, must From <= Index");

        var enumerable = source as TSource[] ?? source.ToArray();

        Index = index;
        Size = size;
        From = from;
        Count = enumerable.Length;
        Pages = (int)Math.Ceiling(Count / (double)Size);

        var items = enumerable.Skip((Index - From) * Size).Take(Size).ToArray();
        Items = new List<TResult>(converter(items));
    }

    public Paginate(IPaginate<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
    {
        Index = source.Index;
        Size = source.Size;
        From = source.From;
        Count = source.Count;
        Pages = source.Pages;
        Items = new List<TResult>(converter(source.Items));
    }

    public int Index { get; }
    public int Size { get; }
    public int Count { get; }
    public int Pages { get; }
    public int From { get; }
    public IList<TResult> Items { get; }
    public bool HasPrevious => Index > From;
    public bool HasNext => Index < Pages - 1;
}

/// <summary>
/// The default implementation of a paginated list.
/// </summary>
public class Paginate<T> : IPaginate<T>
{
    internal Paginate(IEnumerable<T> source, int index, int size, int from)
    {
        if (from > index) throw new ArgumentException($"From: {from} > Index: {index}, must From <= Index");

        var enumerable = source as T[] ?? source.ToArray();

        Index = index;
        Size = size;
        From = from;
        Count = enumerable.Length;
        Pages = (int)Math.Ceiling(Count / (double)Size);
        Items = enumerable.Skip((Index - From) * Size).Take(Size).ToList();
    }

    internal Paginate()
    {
        Items = Array.Empty<T>();
    }

    public int From { get; set; }
    public int Index { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
    public int Pages { get; set; }
    public IList<T> Items { get; set; }
    public bool HasPrevious => Index > From;
    public bool HasNext => Index < Pages - 1;
}