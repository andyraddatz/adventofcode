using System.Text;

namespace AoCcsharp;
public static class Day3
{
    private static readonly string[] _input = File.ReadAllLines("data/day3.txt");
    class PartNumber
    {
        public int Number { get; set; }
        public int StartX { get; set; }
        public int EndX { get; set; }
        public int Y { get; set; }
    }
    class Gear
    {
        public PartNumber? Part1 { get; set; }
        public PartNumber? Part2 { get; set; }
        public int Ratio => Part1?.Number * Part2?.Number ?? throw new Exception("Missing a part number");
    }
    public static int Part1()
    {
        List<PartNumber> partNumbers = GetPartNumbers();
        return partNumbers.Sum(pn => pn.Number);
    }

    private static List<PartNumber> GetPartNumbers()
    {
        StringBuilder partNumberBuilder = new();
        PartNumber nextPartNumber = new();
        List<PartNumber> potentialPartNumbers = [];
        Dictionary<int, int[]> symbolLocations = [];

        // Any numbers adjacent to a symbol (including diagonal) should be summed
        // so valid part numbers have a symbol.Y +/- 1 and symbol.X +/- 1
        for (int y = 0; y < _input.Length; y++)
        {
            var line = _input[y];
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                if (char.IsDigit(c))
                {
                    // Start of a new part number
                    if (partNumberBuilder.Length == 0)
                    {
                        nextPartNumber = new PartNumber
                        {
                            Number = 0,
                            StartX = x,
                            EndX = x,
                            Y = y
                        };
                    }
                    else
                    {
                        // Continuing a part number
                        nextPartNumber.EndX = x;
                    }
                    partNumberBuilder.Append(c);
                }
                else
                {
                    if (partNumberBuilder.Length > 0)
                    {
                        // End of a part number
                        nextPartNumber.Number = int.Parse(partNumberBuilder.ToString());
                        potentialPartNumbers.Add(nextPartNumber);
                        partNumberBuilder.Clear();
                    }

                    if (c != '.')
                    {
                        // Add symbol location
                        if (!symbolLocations.TryGetValue(y, out _))
                            symbolLocations.Add(y, [x]);
                        else
                            symbolLocations[y] = [.. symbolLocations[y], x];
                    }
                }
            }
        }

        var partNumbers = potentialPartNumbers.Where(pn =>
            symbolLocations.Any(sl =>

                // symbol location is at most one line away in Y axis
                sl.Key >= pn.Y - 1
                && sl.Key <= pn.Y + 1

                // symbol location is at most one column away in X axis
                && sl.Value.Any(v =>
                    v >= pn.StartX - 1
                    && v <= pn.EndX + 1)

            )).ToList();

        // debug discards
        // var discardedPartNumbers = potentialPartNumbers.Except(partNumbers).ToList();
        return partNumbers;
    }

    public static int Part2()
    {
        var partNumbers = GetPartNumbers();
        List<Gear> gears = [];

        // Gears are represented by a (*) symbol with exactly 2 adjacent part numbers
        // Gear ratio is those 2 part numbers multiplied
        // all gears should have their gear ratios summed
        for (int y = 0; y < _input.Length; y++)
        {
            var line = _input[y];
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                if (c != '*')
                    continue;

                // Find adjacent part numbers
                int[] yRange = [y - 1, y, y + 1];
                int[] xRange = [x - 1, x, x + 1];
                var adjacentPartNumbers = partNumbers.Where(pn =>
                    yRange.Contains(pn.Y)
                    && (
                        xRange.Contains(pn.StartX)
                        || xRange.Contains(pn.EndX)
                    )
                ).ToList();

                // If there are exactly 2 adjacent part numbers, we have a gear
                if (adjacentPartNumbers.Count == 2)
                {
                    gears.Add(new Gear
                    {
                        Part1 = adjacentPartNumbers[0],
                        Part2 = adjacentPartNumbers[1]
                    });
                }
            }
        }

        return gears.Sum(g => g.Ratio);
    }
}