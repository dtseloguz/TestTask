public sealed record Error(int Code, string Message)
{
    public static readonly Error None = new(400, string.Empty);

    public static Error NotFound() =>
        new(400, "Запрашиваемые данные не найдены");

    public static Error HandleError(string errorText) =>
        new(400, $"{errorText}");

    public static Error ServerError() =>
        new(500, "Internal server error");
    public static Error DatabaseFailure() =>
        new(400, "Ошибка при совершении операции с базой данных");
}