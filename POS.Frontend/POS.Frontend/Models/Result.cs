namespace POS.Frontend.Models;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public bool IsError { get; set; }
    public bool IsValidationError { get; set; }
    public bool IsSystemError { get; set; }
    public bool IsNotFoundError { get; set; }
    public bool IsConflictError { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
}
