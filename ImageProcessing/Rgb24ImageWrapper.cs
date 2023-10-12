#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using MathMatrixKit.Matrix;

namespace AltaHomeWork_1.ImageProcessing;

public readonly struct Rgb24ImageWrapper : IMatrix<Rgb24>
{
    private readonly Image<Rgb24> _image;
    public int Height => _image.Height;
    public int Width => _image.Width;

    public Rgb24 this[int x, int y]
    {
        get => _image[x, y];
        set => _image[x, y] = value;
    }

    public Rgb24ImageWrapper(Image<Rgb24> image)
    {
        _image = image;
    }
}
