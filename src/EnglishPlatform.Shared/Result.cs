namespace EnglishPlatform.Shared;

/// <summary>
/// Generic Result pattern for service layer responses.
/// </summary>
public class Result<T>
{
    public bool Success { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public List<string> Errors { get; private set; } = new();

    public static Result<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static Result<T> Fail(string error) =>
        new() { Success = false, Errors = new List<string> { error } };

    public static Result<T> Fail(List<string> errors) =>
        new() { Success = false, Errors = errors };
}

/// <summary>
/// Non-generic Result for void operations.
/// </summary>
public class Result
{
    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public List<string> Errors { get; private set; } = new();

    public static Result Ok(string? message = null) =>
        new() { Success = true, Message = message };

    public static Result Fail(string error) =>
        new() { Success = false, Errors = new List<string> { error } };

    public static Result Fail(List<string> errors) =>
        new() { Success = false, Errors = errors };
}
