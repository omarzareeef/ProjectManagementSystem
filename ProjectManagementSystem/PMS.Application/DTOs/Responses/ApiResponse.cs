namespace PMS.Application.DTOs.Responses;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public ApiResponse() { }

    private ApiResponse(T? data, string? message)
    {
        Succeeded = true;
        Message = message;
        Data = data;
    }

    private ApiResponse(string message, List<string>? errors)
    {
        Succeeded = false;
        Message = message;
        Errors = errors;
    }

    public static ApiResponse<T> Success(T data, string? message = null)
        => new ApiResponse<T>(data, message);

    public static ApiResponse<T> Failure(string message, List<string>? errors = null)
        => new ApiResponse<T>(message, errors);
}
