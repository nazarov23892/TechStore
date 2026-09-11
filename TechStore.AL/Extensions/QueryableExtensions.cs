namespace TechStore.AL.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Возвращает запрос на постраничный фрагмент исходного <see cref="IQueryable"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceQuery">Исходный запрос.</param>
    /// <param name="pageStarsZero">Номер страницы (нумерация с нуля).</param>
    /// <param name="perPage">Количество элементов на странице.</param>
    public static IQueryable<T> WithPaging<T>(
        this IQueryable<T> sourceQuery, int pageStarsZero, int perPage)
        => sourceQuery
        .Skip(pageStarsZero * perPage)
        .Take(perPage);
}
