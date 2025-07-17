namespace Shopping.SharedKernel.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result(bool isSuccess, T? data, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T data) => new(true, data);
    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
    public static Result<T> Failure(List<string> errors) 
    {
        var result = new Result<T>(false, default);
        result.Errors = errors;
        return result;
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new(true);
    public static Result Failure(string errorMessage) => new(false, errorMessage);
    public static Result Failure(List<string> errors)
    {
        var result = new Result(false);
        result.Errors = errors;
        return result;
    }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
