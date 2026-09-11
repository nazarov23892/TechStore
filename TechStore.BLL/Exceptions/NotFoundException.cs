namespace TechStore.BLL.Exceptions;

/// <summary>
/// Исключение, возникающее если запрашиваемая сущность не существует.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    { }
}
