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
}