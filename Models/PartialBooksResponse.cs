#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.ComponentModel.DataAnnotations;

namespace AltaHomeWork_1.Models;

public class PartialBooksResponse
{
    [Required] public required List<BookInfo> FoundBooks { get; init; }
    [Required] public required List<Guid> NotFoundGuids { get; init; }
}
