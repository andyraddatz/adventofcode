
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace AoCcsharp
{
    internal static class Day6
    {
        internal class Race
        {
            public int Time { get; set; }
            public long Distance { get; set; }
        }
        // we really don't need to parse since the values are so few
        // Time:        60     80     86     76
        // Distance:   601   1163   1559   1300
        internal static Race[] _races = [
            new() { Time = 60, Distance = 601 },
            new() { Time = 80, Distance = 1163 },
            new() { Time = 86, Distance = 1559 },
            new() { Time = 76, Distance = 1300 }
        ];
        internal static int Part1()
        {
            var marginOfError = 1;
            marginOfError *= WaysToWin(_races[0]);
            marginOfError *= WaysToWin(_races[1]);
            marginOfError *= WaysToWin(_races[2]);
            marginOfError *= WaysToWin(_races[3]);
            return marginOfError;
        }
        internal static int Part2()
        {
            // TODO: if this never finishes, try a half-distance search approach
            return WaysToWin(new Race { Time = 60808676, Distance = 601116315591300 });
        }

        private static int WaysToWin(Race r)
        {
            var waysToWin = 0;
            // Your toy boat has a starting speed of zero millimeters per millisecond. 
            // For each whole millisecond you spend at the beginning of the race holding down the button, 
            // the boat's speed increases by one millimeter per millisecond.
            // interesting: another case where Parallel is slower
            // object lockObject = new();
            // Parallel.For(0, r.Time, i =>
            for (var i = 0; i <= r.Time; i++)
            {
                var buttonHeld = i;     // ms
                var speed = i;          // mm/ms
                var timeSpentMoving = r.Time - buttonHeld;
                ulong distanceTraveled = (ulong)speed * (ulong)timeSpentMoving;

                // lock (lockObject)
                if (distanceTraveled > (ulong)r.Distance)
                {
                    Debug.WriteLine($"Time: {r.Time} Distance: {r.Distance} ButtonHeld: {buttonHeld} Speed: {speed} TimeSpentMoving: {timeSpentMoving} DistanceTraveled: {distanceTraveled}");
                    waysToWin++;
                }
                else Debug.WriteLine($"DistanceTraveled: {distanceTraveled} < Distance: {r.Distance}");
            }
            // );
            return waysToWin;
        }
    }
}