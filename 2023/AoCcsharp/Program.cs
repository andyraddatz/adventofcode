﻿using System.Diagnostics;
using AoCcsharp;

using Process proc = Process.GetCurrentProcess();
var _totalStopWatch = new Stopwatch();
_totalStopWatch.Start();

var _stopWatch = new Stopwatch();
_stopWatch.Start();

DisplayResults("Day1.Part1", Day1.Part1(), proc, _stopWatch);
DisplayResults("Day1.Part2", Day1.Part2(), proc, _stopWatch);
DisplayResults("Day2.Part1", Day2.Part1(), proc, _stopWatch);
DisplayResults("Day2.Part2", Day2.Part2(), proc, _stopWatch);
DisplayResults("Day3.Part1", Day3.Part1(), proc, _stopWatch);
DisplayResults("Day3.Part2", Day3.Part2(), proc, _stopWatch);
DisplayResults("Day4.Part1", Day4.Part1(), proc, _stopWatch);

_stopWatch.Stop();
_totalStopWatch.Stop();
Console.WriteLine($"Total time: {_totalStopWatch.ElapsedMilliseconds} ms");

static void DisplayResults(string title, int result, Process proc, Stopwatch stopWatch)
{
    Console.WriteLine($"{title}: {result,-10} {stopWatch.ElapsedMilliseconds,-3}ms - {proc.GetMemoryMB(),-5}MB");
    stopWatch.Restart();
}