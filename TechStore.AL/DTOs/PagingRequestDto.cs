namespace TechStore.AL.DTOs;

/// <summary>
/// Модель запрашиваемой постраничной навигации.
/// </summary>
public class PagingRequestDto
{
    public const int DefaultPerPage = 5;

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = DefaultPerPage;
}