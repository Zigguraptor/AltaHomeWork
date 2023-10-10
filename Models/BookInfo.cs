#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.ComponentModel.DataAnnotations;

namespace AltaHomeWork_1.Models;

public class BookInfo
{
    [Required(ErrorMessage = $"{nameof(Guid)} не может быть пустым")]
    public Guid Guid { get; init; }

    [Required(AllowEmptyStrings = false, ErrorMessage = $"{nameof(Title)} не может быть пустым")]
    public required string Title { get; init; }

    [Required(AllowEmptyStrings = false, ErrorMessage = $"{nameof(Author)} не может быть пустым")]
    public required string Author { get; init; }

    [Required(ErrorMessage = $"{nameof(PriceRub)} не может быть пустым")]
    [Range(0.0, double.PositiveInfinity, ErrorMessage = $"Стоимость должна быть положительным числом")]
    public required decimal PriceRub { get; set; }
}
