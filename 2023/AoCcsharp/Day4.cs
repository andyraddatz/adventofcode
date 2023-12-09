using System.Data;
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
            var myNumbers = GetNumbers(line);
            var winningNumbers = GetNumbers(line, myNumbers: false);
            var points = 0;
            StringBuilder gameReporter = new("                                 | ");
            foreach (var num in myNumbers)
            {
                if (winningNumbers.Contains(num))
                {
                    points = points == 0 ? 1 : (points * 2);
                    gameReporter.Append($"{num,-3}");
                }
                else gameReporter.Append(" . ");
            }
            Debug.WriteLine($"{line}");
            Debug.WriteLine($"{points}pts - {gameReporter}");
            totalPoints += points;
        }

        return totalPoints; // sum of points all cards are worth
    }

    private static IEnumerable<string> GetNumbers(string line, bool myNumbers = true)
    {
        // not the most parseable, but I was spending a long time finding a more terse way and it wasn't worth it
        return line[(line.IndexOf(':') + 1)..].Split('|')[myNumbers ? 1 : 0].Split(' ').Where(n => !string.IsNullOrWhiteSpace(n));
    }

    public static int Part2()
    {
        // todo: tally total cards after 
        // var totalCardTally = 0;
        // copying N subsequent cards where N is the number of winning numbers on a given card
        // so if card 1 wins twice, add copies of 2 and 3
        // if card 2 wins 3 times, add copies of 3, 4, and 5 for the original 
        // and 3, 4, and 5 again for the copy from card 1's win
        // so copies compound as we go
        var cardStacks = new Dictionary<int, int>();

        foreach (var line in _input)
        {
            // no format validations because i can see all the data
            var cardId = int.Parse(line[..line.IndexOf(':')].Split(' ').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray()[1]);

            // init with 1 card or add to stack
            if (cardStacks.TryGetValue(cardId, out _))
                cardStacks[cardId]++;
            else
                cardStacks.Add(cardId, 1);

            var myNumbers = GetNumbers(line);
            var winningNumbers = GetNumbers(line, myNumbers: false);
            var winnersCount = myNumbers.Intersect(winningNumbers).Count();
            Debug.WriteLine($"Winners for card {cardId}: {winnersCount} ({string.Join(',', myNumbers.Intersect(winningNumbers))})");

            // foreach card in the stack (after previous copies)
            for (int i = cardStacks[cardId]; i > 0; i--)
                // for each winner, copy another step ahead on the stack
                for (int j = 1; j <= winnersCount; j++)
                    if (!cardStacks.TryGetValue(cardId + j, out _))
                        cardStacks.Add(cardId + j, 1);
                    else cardStacks[cardId + j]++;
        }

        return cardStacks.Sum(gs => gs.Value);
    }
}