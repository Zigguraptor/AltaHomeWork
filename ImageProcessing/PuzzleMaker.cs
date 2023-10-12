using MathMatrixKit.Matrix;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace AltaHomeWork_1.ImageProcessing;

public static class PuzzleMaker
{
    [ThreadStatic] private static Random? _random;
    private static readonly object GlobalRandomSync = new();
    private static readonly Random GlobalRandom = new();

    private static Random Random => _random ??= new Random(GetThreadSafeNext());

    private static int GetThreadSafeNext()
    {
        lock (GlobalRandomSync)
            return GlobalRandom.Next();
    }

    public static Matrix<T> GeneratePuzzle<T>(IMatrix<T> matrix, int xCount, int yCount)
    {
        var fragments = new Matrix<T>(matrix).Split(xCount, yCount).ToList();
        if (fragments.Count < 1) return new Matrix<T>(0, 0);
        if (fragments.Count == 1) return fragments[0];

        var mixedFragments = fragments.OrderBy(_ => Random.Next()).ToList();

        return AssembleFragments(mixedFragments, xCount, yCount);
    }

    private static Matrix<T> AssembleFragments<T>(IReadOnlyList<Matrix<T>> fragments, int xCount, int yCount)
    {
        var lines = new List<Matrix<T>>(yCount);

        for (var y = 0; y < yCount; y++)
        {
            var line = fragments[y * xCount];
            for (var x = 1; x < xCount; x++)
                line.AppendToRight(fragments[y * xCount + x]);
            lines.Add(line);
        }

        for (var i = 1; i < lines.Count; i++)
            lines[0].AppendToBottom(lines[i]);

        return lines[0];
    }
}
