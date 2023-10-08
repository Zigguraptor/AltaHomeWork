// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable CS1998
#pragma warning disable CS1591

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace AltaHomeWork_1;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API книжного магазина",
                Description = "API для доступа к списку книг",
                Version = "v1"
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, "AltaHomeWork_1.xml");
            options.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Visits API v1"); });
        }

        var booksGroup = app.MapGroup("/books");
        booksGroup.MapGet("", BookStorageHandler.GetAllBooks);
        booksGroup.MapGet("/byGuids", BookStorageHandler.GetBooksByGuids);
        booksGroup.MapGet("/byAuthor", BookStorageHandler.FindBooksByAuthor);
        booksGroup.MapPost("", BookStorageHandler.AddNewBook);
        booksGroup.MapPut("", BookStorageHandler.ReplaceBook);
        booksGroup.MapDelete("", BookStorageHandler.DeleteBook);
        booksGroup.MapDelete("/dropAll", BookStorageHandler.DropAll);

        app.Run();
    }
}

public class BookInfo
{
    [Required] public Guid Guid { get; init; }
    [Required(AllowEmptyStrings = true)] public required string Title { get; init; }
    [Required(AllowEmptyStrings = true)] public required string Author { get; init; }
    [Required] public required decimal PriceRub { get; set; }
}

public class CreateBookRequestModel
{
    [Required(ErrorMessage = $"{nameof(Title)} -- обязательное поле")]
    public string Title { get; init; } = null!;

    [Required(ErrorMessage = $"{nameof(Author)} -- обязательное поле")]
    public string Author { get; init; } = null!;

    [Required(ErrorMessage = $"{nameof(PriceRub)} -- обязательное поле")]
    public decimal PriceRub { get; init; }
}

public class PartialBooksResponse
{
    [Required] public required List<BookInfo> FoundBooks { get; init; }
    [Required] public required List<Guid> NotFoundGuids { get; init; }
}

internal static class BookDbContext
{
    static BookDbContext()
    {
        BookInfos = new Dictionary<Guid, BookInfo>();

        foreach (var bookInfo in TestBookInfos())
            BookInfos.Add(bookInfo.Guid, bookInfo);
    }

    public static Dictionary<Guid, BookInfo> BookInfos { get; set; }

    private static IEnumerable<BookInfo> TestBookInfos()
    {
        yield return new BookInfo
        {
            Guid = Guid.NewGuid(),
            Title = "Война и мир. Том I",
            Author = "Л. Н. Толстой.",
            PriceRub = 165.99m
        };
        yield return new BookInfo
        {
            Guid = Guid.NewGuid(),
            Title = "Война и мир. Том II",
            Author = "Л. Н. Толстой.",
            PriceRub = 489.99m
        };
        yield return new BookInfo
        {
            Guid = Guid.NewGuid(),
            Title = "Война и мир. Том III",
            Author = "Л. Н. Толстой.",
            PriceRub = 356.99m
        };
        yield return new BookInfo
        {
            Guid = Guid.NewGuid(),
            Title = "Война и мир. Том IV",
            Author = "Л. Н. Толстой.",
            PriceRub = 234.99m
        };
    }
}

public static class BookStorageHandler
{
    /// <summary>
    ///     Возвращает список всех книг.
    /// </summary>
    /// <returns>Массив объектов книг</returns>
    /// <response code="200">Успешно отправлен список книг</response>
    [ProducesResponseType(typeof(List<BookInfo>), 200)]
    public static async ValueTask<IResult> GetAllBooks() =>
        TypedResults.Ok(BookDbContext.BookInfos.Select(pair => pair.Value).ToList());

    /// <summary>
    ///     Возвращает список книг по списку GUID.
    /// </summary>
    /// <returns>Массив объектов книг или Массив объектов книг и массив GUID которых нет в базе</returns>
    /// <response code="200">Успешно отправлен список книг</response>
    /// <response code="206">Найдена только часть книг</response>
    /// <response code="404">Ничего не найдено</response>
    [ProducesResponseType(typeof(List<BookInfo>), 200)]
    [ProducesResponseType(typeof(PartialBooksResponse), 206)]
    public static async ValueTask<IResult> GetBooksByGuids([FromQuery] Guid[] guids)
    {
        var foundBooks = new List<BookInfo>(guids.Length);
        var notFoundGuids = new List<Guid>(guids.Length);

        foreach (var guid in guids)
            if (BookDbContext.BookInfos.TryGetValue(guid, out var bookInfo))
                foundBooks.Add(bookInfo);
            else
                notFoundGuids.Add(guid);

        if (notFoundGuids.Count <= 0)
            return TypedResults.Ok(foundBooks);

        if (notFoundGuids.Count == guids.Length)
            return TypedResults.NotFound();

        return Results.Json(new PartialBooksResponse { FoundBooks = foundBooks, NotFoundGuids = notFoundGuids },
            statusCode: StatusCodes.Status206PartialContent);
    }

    /// <summary>
    ///     Добавляет новую книгу.
    /// </summary>
    /// <param name="createBookRequestModel">Данные о книге</param>
    /// <returns></returns>
    /// <response code="201">Книга успешно добавлена</response>
    /// <response code="400">Запрос содержит не корректные данные</response>
    public static async ValueTask<IResult> AddNewBook([FromBody] CreateBookRequestModel createBookRequestModel)
    {
        // Валидация моделей должна быть не тут. А через ModelState, но мы в статическом методе, статического класса.
        if (createBookRequestModel is not { Title: not null, Author: not null })
            return TypedResults.BadRequest();

        var bookInfo = new BookInfo
        {
            Guid = Guid.NewGuid(),
            Title = createBookRequestModel.Title,
            Author = createBookRequestModel.Author,
            PriceRub = createBookRequestModel.PriceRub
        };

        BookDbContext.BookInfos.Add(bookInfo.Guid, bookInfo);
        //будь это контекст из EF. Тут нужно было бы сохранить изменения

        return TypedResults.Created($"/{bookInfo.Guid}");
    }

    /// <summary>
    ///     Заменяет книгу по GUID.
    /// </summary>
    /// <param name="bookInfo">Данные книги с GUID по которому нужно заменить данные</param>
    /// <response code="200">Данные заменены</response>
    /// <response code="400">GUID не найден</response>
    public static async ValueTask<IResult> ReplaceBook([FromBody] BookInfo bookInfo)
    {
        if (!BookDbContext.BookInfos.ContainsKey(bookInfo.Guid))
            return TypedResults.BadRequest();

        BookDbContext.BookInfos[bookInfo.Guid] = bookInfo;
        return TypedResults.Ok();
    }

    /// <summary>
    ///     Удаляет книгу по указанному GUID.
    /// </summary>
    /// <param name="bookGuid">GUID книги которую нужно удалить</param>
    /// <response code="200">Данные удалены</response>
    /// <response code="400">GUID не найден</response>
    public static async ValueTask<IResult> DeleteBook(Guid bookGuid) =>
        BookDbContext.BookInfos.Remove(bookGuid)
            ? TypedResults.Ok()
            : TypedResults.BadRequest();

    /// <summary>
    ///     Поиск по автору.
    /// </summary>
    /// <param name="author">Автор</param>
    /// <returns>Найденные книги</returns>
    /// <response code="200">Результат поиска</response>
    [ProducesResponseType(typeof(List<BookInfo>), 200)]
    public static async ValueTask<IResult> FindBooksByAuthor(string author) =>
        TypedResults.Ok(BookDbContext.BookInfos
            .Where(pair => pair.Value.Author.Contains(author))
            .Select(pair => pair.Value)
            .ToList());

    /// <summary>
    ///     Удаляет всё.
    /// </summary>
    /// <response code="200">Всё удалено</response>
    public static async ValueTask<IResult> DropAll()
    {
        BookDbContext.BookInfos = new Dictionary<Guid, BookInfo>();
        return TypedResults.Ok();
    }
}
