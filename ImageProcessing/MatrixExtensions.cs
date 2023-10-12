using MathMatrixKit.Matrix;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace AltaHomeWork_1.ImageProcessing;

public static class MatrixExtensions
{
    public static void CopyTo<T>(this IMatrix<T> thisMatrix, IMatrix<T> matrix)
    {
        if (thisMatrix.Height != matrix.Height || thisMatrix.Width != matrix.Width)
            throw new ArgumentException();

        for (var x = 0; x < matrix.Width; x++)
        for (var y = 0; y < matrix.Height; y++)
            matrix[x, y] = thisMatrix[x, y];
    }
}
