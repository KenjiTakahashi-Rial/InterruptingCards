namespace InterruptingCards.Models
{
    public class Deck<S, R> : IDeck<S, R> where S : Enum where R : Enum
    {
        private List<ICard<S, R>> _cards;
        private readonly Random _random = new();

        private int TopIndex
        {
            get { return _cards.Count - 1; }
        }

        private int BottomIndex { get; } = 0;

        public Deck()
        {
            _cards = new List<ICard<S, R>>();
        }

        public void ReplaceAll(IEnumerable<ICard<S, R>> cards)
        {
            _cards = new List<ICard<S, R>>(cards);
        }

        public void Shuffle()
        {
            CheckEmpty();

            for (var i = 0; i < _cards.Count - 1; i++)
            {
                var j = _random.Next(0, _cards.Count);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public void PlaceTop(ICard<S, R> card)
        {
            _cards.Add(card);
        }

        public void PlaceBottom(ICard<S, R> card)
        {
            _cards.Insert(0, card);
        }

        public void InsertRandom(ICard<S, R> card)
        {
            CheckEmpty();
            var i = _random.Next(0, _cards.Count + 1);
            _cards.Insert(i, card);
        }

        public ICard<S, R> DrawTop()
        {
            return PopAt(TopIndex);
        }

        public ICard<S, R> DrawBottom()
        {
            return PopAt(BottomIndex);
        }

        public ICard<S, R> Remove(S suit, R rank)
        {
            var card = _cards.Find(c => c.Suit.Equals(suit) && c.Rank.Equals(rank)) ?? throw new CardNotFoundException();
            _cards.Remove(card);

            return card;
        }

        private void CheckEmpty()
        {
            if (_cards.Count == 0)
            {
                throw new DeckEmptyException();
            }
        }

        private ICard<S, R> PopAt(int i)
        {
            CheckEmpty();
            var card = _cards[i];
            _cards.RemoveAt(i);
            return card;
        }
    }
}