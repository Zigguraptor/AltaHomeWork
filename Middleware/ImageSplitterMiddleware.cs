using AltaHomeWork_1.ImageProcessing;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace AltaHomeWork_1.Middleware;

public class ImageSplitterMiddleware
{
    private readonly RequestDelegate _next;

    public ImageSplitterMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        if (context.Response.ContentType != null && context.Response.ContentType.Contains("image") &&
            context.Response.StatusCode == 200)
        {
            responseBody.Position = 0;

            var image = await Image.LoadAsync<Rgb24>(responseBody);
            var wrappedImage = new Rgb24ImageWrapper(image);
            var generatePuzzle = PuzzleMaker.GeneratePuzzle(wrappedImage, 8, 8);
            
            generatePuzzle.CopyTo(wrappedImage);
            
            responseBody.Position = 0;
            
            await image.SaveAsPngAsync(responseBody);
            
            context.Response.ContentLength = responseBody.Length;
        }
        
        responseBody.Position = 0;
        await responseBody.CopyToAsync(originalBodyStream);
    }
}
