using System.Data;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace AoCcsharp;

public static class Day5
{

    static readonly string[] _input = File.ReadAllLines("data/day5.txt");
    static readonly long[] _seeds;
    static readonly Tuple<long, long, long>[]
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
                GetTupleForIndex(i)
            ];
        for (int i = 40; i <= 72; i++)
            _soilToFertilizer =
            [
                .. _soilToFertilizer,
                GetTupleForIndex(i)
            ];
        for (int i = 75; i <= 102; i++)
            _fertilizerToWater =
            [
                .. _fertilizerToWater,
                GetTupleForIndex(i)
            ];
        for (int i = 105; i <= 119; i++)
            _waterToLight =
            [
                .. _waterToLight,
                GetTupleForIndex(i)
            ];
        for (int i = 122; i <= 153; i++)
            _lightToTemperature =
            [
                .. _lightToTemperature,
                GetTupleForIndex(i)
            ];
        for (int i = 156; i <= 187; i++)
            _temperatureToHumidity =
            [
                .. _temperatureToHumidity,
                GetTupleForIndex(i)
            ];
        for (int i = 190; i <= 205; i++)
            _humidityToLocation =
            [
                .. _humidityToLocation,
                GetTupleForIndex(i)
            ];
    }

    private static Tuple<long, long, long> GetTupleForIndex(int i) => new(
                    long.Parse(_input[i].Split(' ')[0]),
                    long.Parse(_input[i].Split(' ')[1]),
                    long.Parse(_input[i].Split(' ')[2]));

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

    private static long Convert(long input, Tuple<long, long, long>[] map, bool reverse = false)
    {
        if (reverse)
            return ConvertReverse(input, map);

        foreach (var mapping in map)
        {
            // Tuple<destination range start, source range start, range length>
            var destStart = mapping.Item1;
            var srcStart = mapping.Item2;
            var rngLength = mapping.Item3;

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

    private static long ConvertReverse(long input, Tuple<long, long, long>[] map)
    {
        // copilot wrote this entire function with one TAB key press
        foreach (var mapping in map)
        {
            // Tuple<destination range start, source range start, range length>
            var destStart = mapping.Item1;
            var srcStart = mapping.Item2;
            var rngLength = mapping.Item3;

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

    /// <summary>
    /// This is also not very efficient
    /// </summary>
    /// <returns></returns>
    public static long Part2Attempt2()
    {
        // this time, go backwards from location numbers to seeds
        var lowestPossibleMappedLocationNumber = long.MaxValue;
        foreach (var mapping in _humidityToLocation)
            lowestPossibleMappedLocationNumber =
                mapping.Item1 < lowestPossibleMappedLocationNumber
                    ? mapping.Item1
                    : lowestPossibleMappedLocationNumber;

        var seedRanges = GetSeedRanges();

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

    /// <summary>
    /// This was never efficient enough to complete in a reasonable amount of time
    /// Starting over with Part2Attempt2
    /// </summary>
    /// <returns></returns>
    public static long Part2()
    {
        Tuple<long, long>[] seedRanges = GetSeedRanges();

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

    private static Tuple<long, long>[] GetSeedRanges()
    {
        Tuple<long, long>[] seedRanges = [];
        var seedRangeStart = 0L;
        for (int i = 0; i < _seeds.Length; i++)
        {
            // all even seeds are the start of a range
            if (i % 2 == 0)
                seedRangeStart = _seeds[i];
            else
                seedRanges =
                [
                    .. seedRanges,
                    new Tuple<long, long>(seedRangeStart, seedRangeStart + _seeds[i])
                ];
        }

        return seedRanges;
    }

    private static IEnumerable<Tuple<long, long>> Convert(Tuple<long, long> inputRange, Tuple<long, long, long>[] map)
    {
        // for each mapping, find the resulting output ranges based on the input range
        // these may not be contiguous since the input range may span across values 
        // inside and outside of the source range of the mapping
        var inputStart = inputRange.Item1;
        var inputEnd = inputRange.Item2;
        foreach (var mapping in map)
        {
            var destStart = mapping.Item1;
            var destEnd = destStart + mapping.Item3;
            var srcStart = mapping.Item2;
            var rngLength = mapping.Item3;
            var srcEnd = srcStart + rngLength;

            Debug.WriteLine(@$"
                    inputStart: {inputStart,10}
                    inputEnd:   {inputEnd,10}
                    srcStart:   {srcStart,10}
                    srcEnd:     {srcEnd,10}
                    rngLength:  {rngLength,10}
                    destStart:  {destStart,10}
                    destEnd:    {destEnd,10}
                    ");

            // overlapping none (1 range, itself)
            if (inputStart > srcEnd || inputEnd < srcStart)
                yield return new Tuple<long, long>(inputStart, inputEnd);

            // overlapping start (2 ranges, one before and one after srcStart), 
            else if (inputStart < srcStart && inputEnd >= srcStart && inputEnd <= srcEnd)
            {
                // return inputStart-srcStart and srcStart-inputEnd
                yield return new Tuple<long, long>(inputStart, srcStart);
                yield return new Tuple<long, long>(srcStart, inputEnd);
            }
            // overlapping start + end (3 ranges, one before, one after, 
            //      and one in between srcStart and srcEnd that is mapped (destStart-destEnd)), 
            else if (inputStart < srcStart && inputEnd > srcEnd)
            {
                // return inputStart-srcStart, destStart-destEnd, and srcEnd-inputEnd
                yield return new Tuple<long, long>(inputStart, srcStart - 1);
                yield return new Tuple<long, long>(destStart, destEnd);
                yield return new Tuple<long, long>(srcEnd + 1, inputEnd);
            }
            // overlapping end (2 ranges),
            else if (inputStart >= srcStart && inputStart <= srcEnd && inputEnd > srcEnd)
            {
                // return [destination-mapped-inputStart]-destEnd and destEnd-inputEnd
                yield return new Tuple<long, long>(destStart + (inputStart - srcStart), destEnd);
                yield return new Tuple<long, long>(srcEnd, inputEnd);
            }
            // if input is in the source range
            else if (inputStart >= srcStart && inputEnd <= srcEnd)
            {
                // return the destination range start 
                // + the difference between the input and the source start
                yield return new Tuple<long, long>(destStart + (inputStart - srcStart), destStart + (inputEnd - srcStart));
            }
            else
                throw new NotImplementedException("Unexpected situation");
        }
    }
}