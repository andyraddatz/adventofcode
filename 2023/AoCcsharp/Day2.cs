using System.Diagnostics;

namespace AoCcsharp;
public static class Day2
{

    private const int _redCount = 12, _greenCount = 13, _blueCount = 14;
    private const int _totalCount = _redCount + _greenCount + _blueCount;
    public static int Part1()
    {
        var sum = 0;
        var _input = File.ReadAllLines("data/day2.txt");
        foreach (var line in _input)
        {
            // get game ID from string format: "Game 1: ..."
            var gameId = line[5..line.IndexOf(':')];
            var possible = true;

            // split game into each pull for that game
            var pulls = line[(line.IndexOf(':') + 2)..].Split("; ");
            foreach (string pull in pulls)
            {
                var colorCounts = pull.Split(", ");
                var totalCount = 0;
                foreach (var cc in colorCounts)
                {
                    var count = int.Parse(cc[..cc.IndexOf(' ')]);
                    totalCount += count;

                    if (totalCount > _totalCount)
                        possible = false;

                    var color = cc[(cc.IndexOf(' ') + 1)..];
                    switch (color)
                    {
                        case "red":
                            if (count > _redCount)
                                possible = false;
                            break;
                        case "green":
                            if (count > _greenCount)
                                possible = false;
                            break;
                        case "blue":
                            if (count > _blueCount)
                                possible = false;
                            break;
                    }
                }
            }

            // determine if game is possible, add game ID to sum
            Debug.WriteLine($"{possible} - {line}");
            if (possible)
                sum += int.Parse(gameId);
        }
        return sum;
    }
    public static int Part2()
    {
        // For each game, find the minimum set of cubes that must have been present. 
        // What is the sum of the power of these sets?
        var sum = 0;
        var _input = File.ReadAllLines("data/day2.txt");
        foreach (var line in _input)
        {
            // get game ID from string format: "Game 1: ..."
            var gameId = line[5..line.IndexOf(':')];
            int minRed = 0, minGreen = 0, minBlue = 0;

            // split game into each pull for that game
            var pulls = line[(line.IndexOf(':') + 2)..].Split("; ");
            foreach (string pull in pulls)
            {
                var colorCounts = pull.Split(", ");
                foreach (var cc in colorCounts)
                {
                    var count = int.Parse(cc[..cc.IndexOf(' ')]);

                    var color = cc[(cc.IndexOf(' ') + 1)..];
                    switch (color)
                    {
                        case "red":
                            if (count > minRed) minRed = count;
                            break;
                        case "green":
                            if (count > minGreen) minGreen = count;
                            break;
                        case "blue":
                            if (count > minBlue) minBlue = count;
                            break;
                    }
                }
            }

            // What is the sum of the power of these sets?
            var power = minRed * minGreen * minBlue;
            if (power == 0) throw new Exception("power is 0?");
            Debug.WriteLine($"[{line}]\r\npower: {minRed}r x {minGreen}g x {minBlue}b = {power}");

            sum += power;
        }
        return sum;
    }
}