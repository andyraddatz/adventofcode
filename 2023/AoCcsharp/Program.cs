using AoCcsharp;

var _stopWatch = new System.Diagnostics.Stopwatch();
var _overallStopWatch = new System.Diagnostics.Stopwatch();
_overallStopWatch.Start();
_stopWatch.Start();
Console.WriteLine($"Day 1 - Part 1: {Day1.Part1()} --- {_stopWatch.ElapsedMilliseconds} ms");
_stopWatch.Restart();
Console.WriteLine($"Day 1 - Part 2: {Day1.Part2()} --- {_stopWatch.ElapsedMilliseconds} ms");
_stopWatch.Restart();
Console.WriteLine($"Day 2 - Part 1: {Day2.Part1()} --- {_stopWatch.ElapsedMilliseconds} ms");
_stopWatch.Restart();
Console.WriteLine($"Day 2 - Part 2: {Day2.Part2()} --- {_stopWatch.ElapsedMilliseconds} ms");
_stopWatch.Restart();
Console.WriteLine($"Day 3 - Part 1: {Day3.Part1()} --- {_stopWatch.ElapsedMilliseconds} ms");
_stopWatch.Stop();
Console.WriteLine($"Total time: {_overallStopWatch.ElapsedMilliseconds} ms");
_overallStopWatch.Stop();