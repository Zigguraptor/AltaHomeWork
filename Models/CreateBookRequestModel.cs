#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.ComponentModel.DataAnnotations;

namespace AltaHomeWork_1.Models;

public class CreateBookRequestModel
{
    [Required(ErrorMessage = $"{nameof(Title)} не может быть пустым")]
    public string Title { get; init; } = null!;

    [Required(ErrorMessage = $"{nameof(Author)} не может быть пустым")]
    public string Author { get; init; } = null!;

    [Required(ErrorMessage = $"{nameof(PriceRub)} не может быть пустым")]
    [Range(0.0, double.PositiveInfinity, ErrorMessage = $"Стоимость должна быть положительным числом")]
    public decimal? PriceRub { get; init; }
}
