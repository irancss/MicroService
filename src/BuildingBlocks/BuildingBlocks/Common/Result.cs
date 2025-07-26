namespace BuildingBlocks.Common;

/// <summary>
/// A generic result class to represent the outcome of an operation with a return value.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result(bool isSuccess, T? data, string? errorMessage = null, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        if (errors != null)
        {
            Errors = errors;
        }
        else if (errorMessage != null)
        {
            Errors.Add(errorMessage);
        }
    }

    public static Result<T> Success(T data) => new(true, data);
    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
    public static Result<T> Failure(List<string> errors) => new(false, default, errors.FirstOrDefault(), errors);
}

/// <summary>
/// A result class to represent the outcome of an operation without a return value.
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result(bool isSuccess, string? errorMessage = null, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        if (errors != null)
        {
            Errors = errors;
        }
        else if (errorMessage != null)
        {
            Errors.Add(errorMessage);
        }
    }

    public static Result Success() => new(true);
    public static Result Failure(string errorMessage) => new(false, errorMessage);
    public static Result Failure(List<string> errors) => new(false, errors.FirstOrDefault(), errors);
}