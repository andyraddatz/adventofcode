namespace AoCcsharp;

using System.Diagnostics;

internal static class Extensions
{
    internal static string GetMemoryMB(this Process proc)
    {
        proc.Refresh();
        return Math.Round((decimal)proc.PrivateMemorySize64 / (1024 * 1024), 2).ToString();
    }
}