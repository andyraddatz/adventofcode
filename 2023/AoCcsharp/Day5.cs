using System.Data;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Microsoft.VisualBasic;

namespace AoCcsharp;

public static class Day5
{

    static readonly string[] _input = File.ReadAllLines("data/day5.txt");
    static readonly long[] _seeds;
    static Tuple<long, long>[] _seedRanges;
    static Mapping[]
        _seedToSoil = [],
        _soilToFertilizer = [],
        _fertilizerToWater = [],
        _waterToLight = [],
        _lightToTemperature = [],
        _temperatureToHumidity = [],
        _humidityToLocation = [];
    static Day5()
    {
        _seeds = _input.First().Split(':').Last().Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(long.Parse).ToArray();

        // again using the fact that I can see the data
        for (int i = 3; i <= 37; i++)
            _seedToSoil =
            [
                .. _seedToSoil,
                GetMappingForIndex(i)
            ];
        for (int i = 40; i <= 72; i++)
            _soilToFertilizer =
            [
                .. _soilToFertilizer,
                GetMappingForIndex(i)
            ];
        for (int i = 75; i <= 102; i++)
            _fertilizerToWater =
            [
                .. _fertilizerToWater,
                GetMappingForIndex(i)
            ];
        for (int i = 105; i <= 119; i++)
            _waterToLight =
            [
                .. _waterToLight,
                GetMappingForIndex(i)
            ];
        for (int i = 122; i <= 153; i++)
            _lightToTemperature =
            [
                .. _lightToTemperature,
                GetMappingForIndex(i)
            ];
        for (int i = 156; i <= 187; i++)
            _temperatureToHumidity =
            [
                .. _temperatureToHumidity,
                GetMappingForIndex(i)
            ];
        for (int i = 190; i <= 205; i++)
            _humidityToLocation =
            [
                .. _humidityToLocation,
                GetMappingForIndex(i)
            ];

        _seedRanges = GetSeedRangesInclusive(_seeds);

        // SetExampleValues();
        _seedToSoil.VerifyNoOverlaps();
        _soilToFertilizer.VerifyNoOverlaps();
        _fertilizerToWater.VerifyNoOverlaps();
        _waterToLight.VerifyNoOverlaps();
        _lightToTemperature.VerifyNoOverlaps();
        _temperatureToHumidity.VerifyNoOverlaps();
        _humidityToLocation.VerifyNoOverlaps();
    }

    internal static void VerifyNoOverlaps(this Mapping[] map)
    {
        if (map.Any(m => map.Any(m2 =>
                m != m2 &&
                (
                m.SourceStart >= m2.SourceStart && m.SourceStart <= m2.SourceEnd ||
                m.SourceEnd >= m2.SourceStart && m.SourceEnd <= m2.SourceEnd ||
                m2.SourceStart >= m.SourceStart && m2.SourceStart <= m.SourceEnd ||
                m2.SourceEnd >= m.SourceStart && m2.SourceEnd <= m.SourceEnd
                )
        )))
        {
            throw new Exception("Overlapping mappings");
        }
    }

    private static Mapping GetMappingForIndex(int i) => new(new(
                    long.Parse(_input[i].Split(' ')[0]),
                    long.Parse(_input[i].Split(' ')[1]),
                    long.Parse(_input[i].Split(' ')[2])));

    public static long Part1()
    {
        long lowestLocationNumber = long.MaxValue;
        foreach (var seed in _seeds)
        {
            var soil = Convert(seed, _seedToSoil);
            var fertilizer = Convert(soil, _soilToFertilizer);
            var water = Convert(fertilizer, _fertilizerToWater);
            var light = Convert(water, _waterToLight);
            var temp = Convert(light, _lightToTemperature);
            var humidity = Convert(temp, _temperatureToHumidity);
            var locationNumber = Convert(humidity, _humidityToLocation);

            // is it lower?
            if (locationNumber < lowestLocationNumber)
                lowestLocationNumber = locationNumber;
        }

        return lowestLocationNumber;
    }
    private static long Convert(long input, Mapping[] map, bool reverse = false)
    {
        if (reverse)
            return ConvertReverse(input, map);

        foreach (var mapping in map)
        {
            // Tuple<destination range start, source range start, range length>
            var destStart = mapping.DestinationStart;
            var srcStart = mapping.SourceStart;
            var rngLength = mapping.RangeLength;

            var srcEnd = srcStart + rngLength;

            // if input is in the source range
            if (input >= srcStart && input < srcEnd)
            {
                Debug.WriteLine(@$"
                    srcStart:   {srcStart,10}
                    input:      {input,10}
                    srcEnd:     {srcEnd,10}
                    rngLength:  {rngLength,10}
                    destStart:  {destStart,10}
                    advance:    {input - srcStart,10}
                    dest:       {destStart + (input - srcStart),10}
                    ");

                // return the destination range start 
                // + the difference between the input and the source start
                return destStart + (input - srcStart);
            }
        }

        // if no mapping return same input
        return input;
    }
    private static long ConvertReverse(long input, Mapping[] map)
    {
        // copilot wrote this entire function with one TAB key press
        foreach (var mapping in map)
        {
            // Tuple<destination range start, source range start, range length>
            var destStart = mapping.DestinationStart;
            var srcStart = mapping.SourceStart;
            var rngLength = mapping.RangeLength;

            var srcEnd = srcStart + rngLength;

            // if input is in the source range
            if (input >= destStart && input < destStart + rngLength)
            {
                Debug.WriteLine(@$"
                    destStart:  {destStart,10}
                    input:      {input,10}
                    srcStart:   {srcStart,10}
                    srcEnd:     {srcEnd,10}
                    rngLength:  {rngLength,10}
                    advance:    {input - destStart,10}
                    src:        {srcStart + (input - destStart),10}
                    ");

                // return the destination range start 
                // + the difference between the input and the source start
                return srcStart + (input - destStart);
            }
        }

        // if no mapping return same input
        return input;
    }
    public static long Part2Attempt3()
    {
        // SetExampleValues();
        long lowestLocationNumber = long.MaxValue;
        int pathsCompleted = 0;
        Tuple<long, long>? lowestLocationRange = default;
        object lockObj = new();
        // _ = Parallel.ForEach(seedRanges, sr =>
        Parallel.For(0, _seedRanges.Length, i =>
        {
            // var soilRanges = Convert(_seedRanges[0], _seedToSoil).Distinct().ToArray();
            var soilRanges = Convert(_seedRanges[i], _seedToSoil).Distinct().ToArray();
            Parallel.For(0, soilRanges.Length, j =>
            {
                // var fertilizerRanges = Convert(soilRanges[0], _soilToFertilizer).Distinct().ToArray();
                var fertilizerRanges = Convert(soilRanges[j], _soilToFertilizer).Distinct().ToArray();
                Parallel.For(0, fertilizerRanges.Length, k =>
                {
                    // var waterRanges = Convert(fertilizerRanges[0], _fertilizerToWater).Distinct().ToArray();
                    var waterRanges = Convert(fertilizerRanges[k], _fertilizerToWater).Distinct().ToArray();
                    Parallel.For(0, waterRanges.Length, l =>
                    {
                        // var lightRanges = Convert(waterRanges[0], _waterToLight).Distinct().ToArray();
                        var lightRanges = Convert(waterRanges[l], _waterToLight).Distinct().ToArray();
                        Parallel.For(0, lightRanges.Length, m =>
                        {
                            // var tempRanges = Convert(lightRanges[0], _lightToTemperature).Distinct().ToArray();
                            var tempRanges = Convert(lightRanges[m], _lightToTemperature).Distinct().ToArray();
                            Parallel.For(0, tempRanges.Length, n =>
                            {
                                // var humidityRanges = Convert(tempRanges[0], _temperatureToHumidity).Distinct().ToArray();
                                var humidityRanges = Convert(tempRanges[n], _temperatureToHumidity).Distinct().ToArray();
                                Parallel.For(0, humidityRanges.Length, o =>
                                {
                                    var locationNumberRanges = Convert(humidityRanges[0], _humidityToLocation).Distinct().ToArray();

                                    var lowestLocationNumberInRange = locationNumberRanges.Min(lnr => lnr.Item1);

                                    Debug.WriteLine($"Paths completed: {++pathsCompleted, 10} - i: {i, 3} - j: {j, 3} - k: {k, 3} - l: {l, 3} - m: {m, 3} - n: {n, 3} - o: {o, 3} - lowestLocationNumberInRange: {lowestLocationNumberInRange, 10}");
                                    lock (lockObj)
                                        if (lowestLocationNumberInRange < lowestLocationNumber)
                                        {
                                            Console.WriteLine($"New low: {lowestLocationNumberInRange, 10}");
                                            lowestLocationRange = locationNumberRanges.OrderBy(lnr => lnr.Item1).First();
                                            lowestLocationNumber = lowestLocationNumberInRange;
                                        }
                                }
                                );
                            }
                            );
                        }
                        );
                    }
                    );
                }
                );
            }
            );
        }
        );

        return lowestLocationNumber;
    }
    private static void SetExampleValues()
    {

        // example text
        // seeds: 79 14 55 13
        _seedRanges = [
            new Tuple<long, long>(79, 79 + 14 - 1),
            new Tuple<long, long>(55, 55 + 13 - 1)
        ];

        // seed-to-soil map:
        // 50 98 2
        // 52 50 48
        _seedToSoil = [
            new(new Tuple<long, long, long>(50, 98, 2)),
            new(new Tuple<long, long, long>(52, 50, 48))
        ];

        // soil-to-fertilizer map:
        // 0 15 37
        // 37 52 2
        // 39 0 15
        _soilToFertilizer = [
            new(new Tuple<long, long, long>(0, 15, 37)),
            new(new Tuple<long, long, long>(37, 52, 2)),
            new(new Tuple<long, long, long>(39, 0, 15))
        ];

        // fertilizer-to-water map:
        // 49 53 8
        // 0 11 42
        // 42 0 7
        // 57 7 4
        _fertilizerToWater = [
            new(new Tuple<long, long, long>(49, 53, 8)),
                        new(new Tuple<long, long, long>(0, 11, 42)),
                        new(new Tuple<long, long, long>(42, 0, 7)),
                        new(new Tuple<long, long, long>(57, 7, 4))
        ];

        // water-to-light map:
        // 88 18 7
        // 18 25 70
        _waterToLight = [
            new(new Tuple<long, long, long>(88, 18, 7)),
                            new(new Tuple<long, long, long>(18, 25, 70))
        ];

        // light-to-temperature map:
        // 45 77 23
        // 81 45 19
        // 68 64 13
        _lightToTemperature = [
            new(new Tuple<long, long, long>(45, 77, 23)),
                                new(new Tuple<long, long, long>(81, 45, 19)),
                                new(new Tuple<long, long, long>(68, 64, 13))
        ];

        // temperature-to-humidity map:
        // 0 69 1
        // 1 0 69
        _temperatureToHumidity = [
            new(new Tuple<long, long, long>(0, 69, 1)),
                                    new(new Tuple<long, long, long>(1, 0, 69))
        ];

        // humidity-to-location map:
        // 60 56 37
        // 56 93 4
        _humidityToLocation = [
            new(new Tuple<long, long, long>(60, 56, 37)),
            new(new Tuple<long, long, long>(56, 93, 4))
        ];
    }
    public static long Part2Example()
    {
        SetExampleValues();
        long lowestLocationNumber = long.MaxValue;
        // object lockObj = new();
        // _ = Parallel.ForEach(seedRanges, sr =>
        Debug.WriteLine("Converting seed to soil");
        var soilRanges = _seedRanges.SelectMany(sr => Convert(sr, _seedToSoil)).Distinct().ToList();
        Debug.WriteLine("Converting soil to fertilizer");
        var fertilizerRanges = soilRanges.SelectMany(soil => Convert(soil, _soilToFertilizer)).Distinct().ToList();
        Debug.WriteLine("Converting fertilizer to water");
        var waterRanges = fertilizerRanges.SelectMany(fertilizer => Convert(fertilizer, _fertilizerToWater)).Distinct().ToList();
        Debug.WriteLine("Converting water to light");
        var lightRanges = waterRanges.SelectMany(water => Convert(water, _waterToLight)).Distinct().ToList();
        Debug.WriteLine("Converting light to temperature");
        var tempRanges = lightRanges.SelectMany(light => Convert(light, _lightToTemperature)).Distinct().ToList();
        Debug.WriteLine("Converting temperature to humidity");
        var humidityRanges = tempRanges.SelectMany(temp => Convert(temp, _temperatureToHumidity)).Distinct().ToList();
        Debug.WriteLine("Converting humidity to location");
        var locationNumberRanges = humidityRanges.SelectMany(humidity => Convert(humidity, _humidityToLocation)).Distinct().ToList();
        var lowestLocationNumberInRange = locationNumberRanges.Min(lnr => lnr.Item1);

        var lowestRange = locationNumberRanges.OrderBy(lnr => lnr.Item1).First();

        // copilot wrote the body of this for loop
        for (long i = lowestRange.Item1; i <= lowestRange.Item2; i++)
        {
            var humidity = Convert(i, _humidityToLocation, reverse: true);
            var temp = Convert(humidity, _temperatureToHumidity, reverse: true);
            var light = Convert(temp, _lightToTemperature, reverse: true);
            var water = Convert(light, _waterToLight, reverse: true);
            var fertilizer = Convert(water, _fertilizerToWater, reverse: true);
            var soil = Convert(fertilizer, _soilToFertilizer, reverse: true);

            var seed = Convert(soil, _seedToSoil, reverse: true);

            // does seed land in any of the seed ranges?
            foreach (var seedRange in _seedRanges)
                if (seed >= seedRange.Item1 && seed <= seedRange.Item2)
                {
                    Debug.WriteLine(@$"
                    seedRangeStart: {seedRange.Item1,10}
                    seed:           {seed,10}
                    seedRangeEnd:   {seedRange.Item2,10}
                    from location:  {i,10}
                    ");

                    // lock (lockObj)
                    // is it lower?
                    if (i < lowestLocationNumber)
                        lowestLocationNumber = i;
                }
        }
        
        return lowestLocationNumber;
    }

    public static long Part2Attempt2()
    {
        // can I use this to somehow map each layer of translation to smallest possible space?
        // this time, go backwards from location numbers to seeds
        var lowestPossibleMappedLocationNumber = long.MaxValue;
        foreach (var mapping in _humidityToLocation)
            lowestPossibleMappedLocationNumber =
                mapping.DestinationStart < lowestPossibleMappedLocationNumber
                    ? mapping.DestinationStart
                    : lowestPossibleMappedLocationNumber;

        var seedRanges = GetSeedRangesInclusive(_seeds);

        // naively search from zero to lowest possible mapped
        // in case we hit any seed ranges
        object lockObj = new();
        long lowestLocationNumber = long.MaxValue;

        // for (int i = 0; i < lowestPossibleMappedLocationNumber; i++)

        Parallel.For(0, lowestPossibleMappedLocationNumber, i =>
        {
            var humidity = Convert(i, _humidityToLocation, reverse: true);
            var temp = Convert(humidity, _temperatureToHumidity, reverse: true);
            var light = Convert(temp, _lightToTemperature, reverse: true);
            var water = Convert(light, _waterToLight, reverse: true);
            var fertilizer = Convert(water, _fertilizerToWater, reverse: true);
            var soil = Convert(fertilizer, _soilToFertilizer, reverse: true);

            var seed = Convert(soil, _seedToSoil, reverse: true);

            // does seed land in any of the seed ranges?
            foreach (var seedRange in seedRanges)
                if (seed >= seedRange.Item1 && seed <= seedRange.Item2)
                {
                    Debug.WriteLine(@$"
                    seedRangeStart: {seedRange.Item1,10}
                    seed:           {seed,10}
                    seedRangeEnd:   {seedRange.Item2,10}
                    from location:  {i,10}
                    ");

                    // if so, return the location number
                    // return i;
                    lock (lockObj)
                        if (i < lowestLocationNumber)
                            lowestLocationNumber = i;
                }
        }
        );

        return 0;
    }

    public static long Part2()
    {
        Tuple<long, long>[] seedRanges = GetSeedRangesInclusive(_seeds);

        long lowestLocationNumber = long.MaxValue;
        object lockObj = new();
        _ = Parallel.ForEach(seedRanges, sr =>
        // foreach (var sr in seedRanges)
        {
            var soilRanges = Convert(sr, _seedToSoil);
            foreach (var lowestLocationNumberInRange in
                from soil in soilRanges
                let fertilizerRanges = Convert(soil, _soilToFertilizer)
                from fertilizer in fertilizerRanges
                let waterRanges = Convert(fertilizer, _fertilizerToWater)
                from water in waterRanges
                let lightRanges = Convert(water, _waterToLight)
                from light in lightRanges
                let tempRanges = Convert(light, _lightToTemperature)
                from temp in tempRanges
                let humidityRanges = Convert(temp, _temperatureToHumidity)
                from humidity in humidityRanges
                let locationNumberRanges = Convert(humidity, _humidityToLocation)
                let lowestLocationNumberInRange = locationNumberRanges.Min(lnr => lnr.Item1)
                select lowestLocationNumberInRange)
            {
                // is it lower?
                lock (lockObj)
                    if (lowestLocationNumberInRange < lowestLocationNumber)
                        lowestLocationNumber = lowestLocationNumberInRange;
            }
        }
        );

        return lowestLocationNumber;
    }

    private static Tuple<long, long>[] GetSeedRangesInclusive(long[] seeds)
    {
        Tuple<long, long>[] seedRanges = [];
        var seedRangeStart = 0L;
        for (int i = 0; i < seeds.Length; i++)
        {
            // all even seeds are the start of a range
            if (i % 2 == 0)
                seedRangeStart = seeds[i];
            else
                seedRanges =
                [
                    .. seedRanges,
                    new Tuple<long, long>(seedRangeStart, seedRangeStart + seeds[i] - 1)
                ];
        }

        return seedRanges;
    }

    private static IEnumerable<Tuple<long, long>> Convert(Tuple<long, long> inputRange, Mapping[] map)
    {

        // if input range is fully outside all mappings, return input range
        if (!map.Any(m =>
            inputRange.Item1 >= m.SourceStart && inputRange.Item1 <= m.SourceEnd
            || inputRange.Item2 >= m.SourceStart && inputRange.Item2 <= m.SourceEnd))
        {
            return [inputRange];
            // yield break;
        }
        // for each mapping, find the resulting output ranges based on the input range
        // these may not be contiguous since the input range may span across values 
        // inside and outside of the source range of the mapping
        var inputStart = inputRange.Item1;
        var inputEnd = inputRange.Item2;

        Tuple<long, long>[] leftovers = [];
        Tuple<long, long>[] converted = [];

        foreach (var mapping in map)
        {
            var srcStart = mapping.SourceStart;
            var destStart = mapping.DestinationStart;
            var rngLength = mapping.RangeLength;

            var srcEnd = mapping.SourceEnd;
            var destEnd = mapping.DestinationEnd;

            var offset = mapping.Offset;
            var advance = inputStart - srcStart;

            Debug.WriteLine(@$"
                    inputStart: {inputStart,10}
                    inputEnd:   {inputEnd,10}
                    srcStart:   {srcStart,10}
                    srcEnd:     {srcEnd,10}
                    rngLength:  {rngLength,10}
                    destStart:  {destStart,10}
                    destEnd:    {destEnd,10}
                    offset:     {offset,10}
                    advance:    {advance,10}
                    ");

            // overlapping none (1 range, itself)
            if (inputStart > srcEnd || inputEnd < srcStart)
            {
                Debug.WriteLine($"No overlap: {inputStart} > {srcEnd} || {inputEnd} < {srcStart}");
                // leftovers = [.. leftovers, inputRange];
                continue;
                // yield return new Tuple<long, long>(inputStart, inputEnd);
            }

            // overlapping start (2 ranges, one before and one after srcStart), 
            else if (inputStart < srcStart && inputEnd >= srcStart && inputEnd <= srcEnd)
            {
                Debug.WriteLine($"Overlap start: {inputStart} < {srcStart} && {inputEnd} >= {srcStart} && {inputEnd} <= {srcEnd}");
                // return inputStart-srcStart and srcStart-inputEnd

                leftovers = [.. leftovers, new Tuple<long, long>(inputStart, srcStart - 1)];
                converted = [.. converted, new Tuple<long, long>(destStart, destStart + (inputEnd - srcStart))];
                // yield return new Tuple<long, long>(inputStart, srcStart - 1);
                // yield return new Tuple<long, long>(destStart, inputEnd + offset); // ??? wrong?
            }
            // overlapping start + end (3 ranges, one before, one after, 
            //      and one in between srcStart and srcEnd that is mapped (destStart-destEnd)), 
            else if (inputStart < srcStart && inputEnd > srcEnd)
            {
                Debug.WriteLine($"Overlap start + end: {inputStart} < {srcStart} && {inputEnd} > {srcEnd}");
                // return inputStart-srcStart, destStart-destEnd, and srcEnd-inputEnd
                leftovers = [.. leftovers, new Tuple<long, long>(inputStart, srcStart - 1)];
                converted = [.. converted, new Tuple<long, long>(destStart, destEnd)];
                // yield return new Tuple<long, long>(inputStart, srcStart - 1);
                // yield return new Tuple<long, long>(destStart, destEnd);
                // yield return new Tuple<long, long>(srcEnd + 1, inputEnd);
            }
            // overlapping end (2 ranges),
            else if (inputStart >= srcStart && inputStart <= srcEnd && inputEnd > srcEnd)
            {
                Debug.WriteLine($"Overlap end: {inputStart} >= {srcStart} && {inputStart} <= {srcEnd} && {inputEnd} > {srcEnd}");
                // return [destination-mapped-inputStart]-destEnd and destEnd-inputEnd
                leftovers = [.. leftovers, new Tuple<long, long>(srcEnd + 1, inputEnd)];
                converted = [.. converted, new Tuple<long, long>(destStart + (inputStart - srcStart), destEnd)];
                // yield return new Tuple<long, long>(destStart + (inputStart - srcStart), destEnd);
                // yield return new Tuple<long, long>(srcEnd + 1, inputEnd);
            }
            // if input is in the source range
            else if (inputStart >= srcStart && inputEnd <= srcEnd)
            {
                Debug.WriteLine($"Input is in source range: {inputStart} >= {srcStart} && {inputEnd} <= {srcEnd}");
                // return the destination range start corresponding to the input range start
                // + the difference between the input and the source start
                converted = [.. converted, new Tuple<long, long>(destStart + advance, destStart + (inputEnd - srcStart))];
                // yield return new Tuple<long, long>(destStart + advance, destStart + (inputEnd - srcStart));
            }
            else
                throw new NotImplementedException("Unexpected situation");
        }
        return leftovers.Concat(converted).AsEnumerable();
    }

    private static IEnumerable<Tuple<long, long>> ConvertReverse(Tuple<long, long> destRange, Tuple<long, long, long>[] map)
    {
        throw new NotImplementedException();
    }
}

internal class Mapping(Tuple<long, long, long> mapping)
{
    public long SourceStart { get; } = mapping.Item2;
    public long SourceEnd { get; } = mapping.Item2 + mapping.Item3 - 1;
    public long DestinationStart { get; } = mapping.Item1;
    public long DestinationEnd { get; } = mapping.Item1 + mapping.Item3 - 1;
    public long RangeLength { get; } = mapping.Item3;
    public long Offset { get; } = mapping.Item1 - mapping.Item2;
}