using System.Diagnostics;
using System.Text;

namespace AoCcsharp;

public static class Day4
{

    static readonly string[] _input = File.ReadAllLines("data/day4.txt");
    public static int Part1()
    {
        var totalPoints = 0;
        foreach (var line in _input)
        {
            // not the most parseable, but I was spending a long time finding a more terse way and it wasn't worth it
            var myCards = line[(line.IndexOf(':') + 1)..].Split('|')[1].Split(' ').Where(n => !string.IsNullOrWhiteSpace(n));
            var winnerCards = line[(line.IndexOf(':') + 1)..].Split('|')[0].Split(' ').Where(n => !string.IsNullOrWhiteSpace(n));
            var points = 0;
            StringBuilder gameReporter = new("                                 | ");
            foreach (var card in myCards)
            {
                if (winnerCards.Contains(card))
                {
                    points = points == 0 ? 1 : (points * 2);
                    gameReporter.Append($"{card,-3}");
                }
                else gameReporter.Append(" . ");
            }
            Debug.WriteLine($"{line}");
            Debug.WriteLine($"{points}pts - {gameReporter}");
            totalPoints += points;
        }

        return totalPoints; // sum of points all cards are worth
    }
    public static int Part2()
    {
        // todo
        return 0;
    }
}