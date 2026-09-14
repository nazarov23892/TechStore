namespace TechStore.BLL.Exceptions;

/// <summary>
/// Исключение, возникающее если параметры для запрашиваемой операции невалидны.
/// </summary>
public class FailedPreconditionException : Exception
{
    /// <summary>
    /// Создает экземпляр <see cref="FailedPreconditionException"/>
    /// </summary>
    public FailedPreconditionException(string message)
         : base(message)
    { }
}
