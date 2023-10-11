using AltaHomeWork_1.DAL;
using AltaHomeWork_1.Models;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace AltaHomeWork_1.Controllers;

/// <summary>
///     Контроллер списка книг
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class BooksController : ControllerBase
{
    /// <summary>
    ///     Возвращает список всех книг.
    /// </summary>
    /// <returns>Массив объектов книг</returns>
    /// <response code="200">Успешно отправлен список книг</response>
    [ProducesResponseType(typeof(List<BookInfo>), 200)]
    [HttpGet]
    public async ValueTask<IActionResult> GetAllBooksAsync() =>
        Ok(BookDbContext.BookInfos.Select(pair => pair.Value).ToList());

    /// <summary>
    ///     Заменяет книгу по GUID.
    /// </summary>
    /// <param name="bookInfo">Данные книги с GUID по которому нужно заменить данные</param>
    /// <response code="200">Данные заменены</response>
    /// <response code="404">GUID не найден</response>
    // [ProducesResponseType(typeof(NotFoundResult), 404)]
    [HttpPut]
    public async ValueTask<IActionResult> ReplaceBookAsync([FromBody] BookInfo bookInfo)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        if (!BookDbContext.BookInfos.ContainsKey(bookInfo.Guid))
            return NotFound();

        BookDbContext.BookInfos[bookInfo.Guid] = bookInfo;
        return Ok();
    }

    /// <summary>
    ///     Удаляет книгу по указанному GUID.
    /// </summary>
    /// <param name="bookGuid">GUID книги которую нужно удалить</param>
    /// <response code="200">Данные удалены</response>
    /// <response code="404">GUID не найден</response>
    // [ProducesResponseType(typeof(NotFoundResult), 404)]
    [HttpDelete]
    public async ValueTask<IActionResult> DeleteBookAsync(Guid bookGuid) =>
        BookDbContext.BookInfos.Remove(bookGuid) ? Ok() : NotFound();

    /// <summary>
    ///     Возвращает список книг по списку GUID.
    /// </summary>
    /// <returns>Массив объектов книг или Массив объектов книг и массив GUID которых нет в базе</returns>
    /// <response code="200">Успешно отправлен список книг</response>
    /// <response code="206">Найдена только часть книг</response>
    /// <response code="404">Ничего не найдено</response>
    [ProducesResponseType(typeof(List<BookInfo>), 200)]
    [ProducesResponseType(typeof(PartialBooksResponse), 206)]
    // [ProducesResponseType(typeof(NotFoundResult), 404)]
    [HttpGet("[action]")]
    public async ValueTask<IActionResult> GetBooksByGuidsAsync([FromQuery] Guid[] guids)
    {
        var foundBooks = new List<BookInfo>(guids.Length);
        var notFoundGuids = new List<Guid>(guids.Length);

        foreach (var guid in guids)
            if (BookDbContext.BookInfos.TryGetValue(guid, out var bookInfo))
                foundBooks.Add(bookInfo);
            else
                notFoundGuids.Add(guid);

        if (notFoundGuids.Count <= 0)
            return Ok(foundBooks);

        if (notFoundGuids.Count == guids.Length)
            return NotFound();

        Response.StatusCode = StatusCodes.Status206PartialContent;
        return new JsonResult(new PartialBooksResponse { FoundBooks = foundBooks, NotFoundGuids = notFoundGuids });
    }

    /// <summary>
    ///     Добавляет новую книгу.
    /// </summary>
    /// <param name="createBookRequestModel">Данные о книге</param>
    /// <returns></returns>
    /// <response code="201">Книга успешно добавлена</response>
    /// <response code="400">Запрос содержит не корректные данные</response>
    [HttpPost("[action]")]
    public async ValueTask<IActionResult> AddNewBookAsync([FromBody] CreateBookRequestModel createBookRequestModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var bookInfo = new BookInfo
        {
            Guid = Guid.NewGuid(),
            Title = createBookRequestModel.Title,
            Author = createBookRequestModel.Author,
            PriceRub = createBookRequestModel.PriceRub
        };

        BookDbContext.BookInfos.Add(bookInfo.Guid, bookInfo);
        //будь это контекст из EF. Тут нужно было бы сохранить изменения

        return Created($"/{bookInfo.Guid}", null);
    }

    /// <summary>
    ///     Поиск по автору.
    /// </summary>
    /// <param name="author">Автор</param>
    /// <returns>Найденные книги</returns>
    /// <response code="200">Результат поиска</response>
    [ProducesResponseType(typeof(List<BookInfo>), 200)]
    [HttpGet("[action]")]
    public async ValueTask<IActionResult> FindBooksByAuthorAsync(string author) =>
        Ok(BookDbContext.BookInfos
            .Where(pair => pair.Value.Author.Contains(author))
            .Select(pair => pair.Value)
            .ToList());

    /// <summary>
    ///     Удаляет всё.
    /// </summary>
    /// <response code="200">Всё удалено</response>
    [HttpDelete("[action]")]
    public async ValueTask<IActionResult> DropAllAsync()
    {
        BookDbContext.BookInfos = new Dictionary<Guid, BookInfo>();
        return Ok();
    }
}
