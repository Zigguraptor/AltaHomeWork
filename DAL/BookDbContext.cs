using AltaHomeWork_1.Models;

namespace AltaHomeWork_1.DAL;

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
