using System.Diagnostics;

var _stopWatch = new System.Diagnostics.Stopwatch();
_stopWatch.Start();
Console.WriteLine($"Day 1 - Part 1: {Day1Part1()}"); // 54697
Console.WriteLine($"--- {_stopWatch.ElapsedMilliseconds} ms ---");
_stopWatch.Stop();

static int Day1Part1()
{
    var input = File.ReadAllLines("data/day1.txt");
    var ret = 0;

    foreach (var line in input)
        for (int i = 0; i < line.Length; i++)
            if (char.IsDigit(line[i]))
            {
                Debug.WriteLine($"First digit of {line} is {line[i]}");
                for (int j = line.Length - 1; j >= 0; j--)
                    if (char.IsDigit(line[j]))
                    {
                        Debug.WriteLine($"Last digit of {line} is {line[j]}");
                        Debug.WriteLine($"Combined: {string.Concat(line[i], line[j])}");
                        Debug.WriteLine($"{ret} + {string.Concat(line[i], line[j])}:");
                        ret += int.Parse(string.Concat(line[i], line[j]));
                        Debug.WriteLine($"New sum: {ret}");
                        break;
                    }
                break;
            }

    return ret;
}