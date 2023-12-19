namespace AoCcsharp;

using System.Diagnostics;

internal static class Extensions
{
    internal static string GetMemoryMB(this Process proc)
    {
        proc.Refresh();

        // proc.PrivateMemorySize64 returns 0 on macos
        // return Math.Round((decimal)proc.PrivateMemorySize64 / (1024 * 1024), 2).ToString();
        return Math.Round((decimal)proc.WorkingSet64 / (1024 * 1024), 2).ToString();
    }

    internal static List<Tuple<long, long>> Shrink(this IEnumerable<Tuple<long, long>> ranges)
    {
        var sorted = ranges.OrderBy(x => x.Item1).ToList();
        var result = new List<Tuple<long, long>>();
        var current = sorted[0];
        foreach (var range in sorted.Skip(1))
        {
            if (range.Item1 <= current.Item2 + 1)
            {
                current = new Tuple<long, long>(current.Item1, Math.Max(current.Item2, range.Item2));
            }
            else
            {
                result.Add(current);
                current = range;
            }
        }
        result.Add(current);
        return result;

    }
}