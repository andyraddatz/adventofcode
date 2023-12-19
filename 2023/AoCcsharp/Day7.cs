namespace AoCcsharp
{
    internal static class Day7
    {
        private static readonly char[] _cards = ['A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'];
        private static readonly char[] _cardsPartTwo = ['A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J'];
        static readonly string[] _input = File.ReadAllLines("data/day7.txt");
        public enum HandType
        {
            FiveOfAKind,
            FourOfAKind,
            FullHouse,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard
        }
        internal class Hand
        {
            public HandType Type
            {
                get
                {
                    // FiveOfAKind,
                    if (Cards.Distinct().Count() == 1)
                    {
                        return HandType.FiveOfAKind;
                    }
                    // FourOfAKind,
                    if (Cards.GroupBy(c => c).Any(g => g.Count() == 4))
                    {
                        return HandType.FourOfAKind;
                    }
                    // FullHouse,
                    if (Cards.GroupBy(c => c).Any(g => g.Count() == 3) && Cards.GroupBy(c => c).Any(g => g.Count() == 2))
                    {
                        return HandType.FullHouse;
                    }
                    // ThreeOfAKind,
                    if (Cards.GroupBy(c => c).Any(g => g.Count() == 3))
                    {
                        return HandType.ThreeOfAKind;
                    }
                    // TwoPair,
                    if (Cards.GroupBy(c => c).Count(g => g.Count() == 2) == 2)
                    {
                        return HandType.TwoPair;
                    }
                    // OnePair,
                    if (Cards.GroupBy(c => c).Any(g => g.Count() == 2))
                    {
                        return HandType.OnePair;
                    }
                    // HighCard
                    return HandType.HighCard;
                }
            }
            public HandType TypePartTwo
            {
                get
                {
                    // Js are now Jokers
                    if (!Cards.Contains('J')) return Type;

                    var jokers = Cards.Count(c => c == 'J');
                    var otherCards = Cards.Where(c => c != 'J').ToArray();

                    // FiveOfAKind,
                    if (jokers == 5 || otherCards.Distinct().Count() == 1)
                    {
                        return HandType.FiveOfAKind;
                    }
                    // FourOfAKind,
                    if (otherCards.GroupBy(c => c).Any(g => g.Count() + jokers == 4))
                    {
                        return HandType.FourOfAKind;
                    }
                    // FullHouse, 1 joker + 2 pair
                    if (otherCards.GroupBy(c => c).Count(g => g.Count() == 2) == 2)
                    {
                        return HandType.FullHouse;
                    }
                    // ThreeOfAKind,
                    if (otherCards.GroupBy(c => c).Any(g => g.Count() + jokers >= 3))
                    {
                        return HandType.ThreeOfAKind;
                    }                    
                    // TwoPair, (should never happen, 1 joker + other pair turns into 3oak, 2 joker + 3 diff cards turns into 3oak)
                    if (jokers == 1 && otherCards.Distinct().Count() == 3 || jokers == 2 && otherCards.Distinct().Count() == 3)
                    {
                        throw new Exception("TwoPair with jokers should never happen");
                        return HandType.TwoPair;
                    }
                    // OnePair, (one joker, no pairs)
                    if (otherCards.Distinct().Count() == 4)
                    {
                        return HandType.OnePair;
                    }
                    // HighCard
                    return HandType.HighCard;
                }
            }
            public char[] Cards { get; set; }
            public int Bid { get; set; }
        }
        private static readonly Hand[] _hands = [];
        static Day7()
        {
            _hands = _input.Select(inp => new Hand
            {
                Cards = [.. inp.Split(' ')[0]],
                Bid = int.Parse(inp.Split(' ')[1])
            }).ToArray();
        }
        internal static int Part1()
        {
            var sortedHands = _hands.OrderByDescending(h => (int)h.Type)
            .ThenByDescending(h => Array.IndexOf(_cards, h.Cards[0]))
            .ThenByDescending(h => Array.IndexOf(_cards, h.Cards[1]))
            .ThenByDescending(h => Array.IndexOf(_cards, h.Cards[2]))
            .ThenByDescending(h => Array.IndexOf(_cards, h.Cards[3]))
            .ThenByDescending(h => Array.IndexOf(_cards, h.Cards[4]));

            var totalValue = 0;
            for (int rank = 1; rank <= sortedHands.Count(); rank++)
            {
                var hand = sortedHands.ElementAt(rank - 1);
                totalValue += hand.Bid * rank;
            }

            return totalValue;
        }
        internal static int Part2()
        {
            var sortedHands = _hands.OrderByDescending(h => (int)h.TypePartTwo)
            .ThenByDescending(h => Array.IndexOf(_cardsPartTwo, h.Cards[0]))
            .ThenByDescending(h => Array.IndexOf(_cardsPartTwo, h.Cards[1]))
            .ThenByDescending(h => Array.IndexOf(_cardsPartTwo, h.Cards[2]))
            .ThenByDescending(h => Array.IndexOf(_cardsPartTwo, h.Cards[3]))
            .ThenByDescending(h => Array.IndexOf(_cardsPartTwo, h.Cards[4]));

            var totalValue = 0;
            for (int rank = 1; rank <= sortedHands.Count(); rank++)
            {
                var hand = sortedHands.ElementAt(rank - 1);
                totalValue += hand.Bid * rank;
            }

            return totalValue;
        }
    }
}