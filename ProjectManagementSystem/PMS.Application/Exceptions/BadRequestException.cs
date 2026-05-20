namespace PMS.Application.Exceptions;

public class BadRequestException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public BadRequestException(string message) : base(message)
    {
        Errors = Array.Empty<string>();
    }

    public BadRequestException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors.ToList();
    }
}
