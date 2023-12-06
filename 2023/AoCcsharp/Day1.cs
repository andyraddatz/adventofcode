using System.Diagnostics;

namespace AoCcsharp;
public static class Day1
{
    public static int Part1()
    {
        var input = File.ReadAllLines("data/day1.txt");
        var ret = 0;

        foreach (var line in input)
            for (int i = 0; i < line.Length; i++)
                if (char.IsDigit(line[i]))
                {
                    // Debug.WriteLine($"First digit of {line} is {line[i]}");
                    for (int j = line.Length - 1; j >= 0; j--)
                        if (char.IsDigit(line[j]))
                        {
                            // Debug.WriteLine($"Last digit of {line} is {line[j]}");
                            // Debug.WriteLine($"Combined: {string.Concat(line[i], line[j])}");
                            // Debug.WriteLine($"{ret} + {string.Concat(line[i], line[j])}:");
                            ret += int.Parse(string.Concat(line[i], line[j]));
                            // Debug.WriteLine($"New sum: {ret}");
                            break;
                        }
                    break;
                }

        return ret;
    }

    public static int Part2()
    {
        var input = File.ReadAllLines("data/day1.txt");
        var ret = 0;

        foreach (var line in input)
            for (int i = 0; i < line.Length; i++)
                if (char.IsDigit(line[i]) || i == line.Length - 1)
                {
                    char firstDigit = FindStringDigitBefore(line, i) ?? line[i];

                    for (int j = line.Length - 1; j >= 0; j--)
                        if (char.IsDigit(line[j]) || j == 0)
                        {
                            char lastDigit = FindStringDigitAfter(line, j) ?? line[j];
                            // Debug.WriteLine($"output of {line} is {string.Concat(firstDigit, lastDigit)}");
                            ret += int.Parse(string.Concat(firstDigit, lastDigit));
                            break;
                        }
                    break;
                }
        return ret;
    }

    static readonly string[] _spelledOutDigits = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
    private static char? FindStringDigitAfter(string line, int j)
    {
        for (int c = line.Length - 1; c > j; c--)
            foreach (var digit in _spelledOutDigits)
                if (line[c..].Length >= digit.Length && line.Substring(c, digit.Length) == digit)
                    return DigitToChar(digit);
        return null;
    }
    private static char? FindStringDigitBefore(string line, int i)
    {
        for (int c = 0; c < i; c++)
            foreach (var digit in _spelledOutDigits)
                if (line[c..].Length >= digit.Length && line.Substring(c, digit.Length) == digit)
                    return DigitToChar(digit);
        return null;
    }
    private static char? DigitToChar(string digit) => digit switch
    {
        "one" => '1',
        "two" => '2',
        "three" => '3',
        "four" => '4',
        "five" => '5',
        "six" => '6',
        "seven" => '7',
        "eight" => '8',
        "nine" => '9',
        _ => null,
    };
}