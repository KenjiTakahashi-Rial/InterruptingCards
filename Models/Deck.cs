using System;
using System.Collections.Generic;

public class Deck : IDeck
{
    private List<ICard> _cards;
    private Random random = new Random();

    private int _topIndex
    {
        get { return _cards.Count - 1; }
    }

    private int _bottomIndex
    {
        get { return 0; }
    }

    public Deck()
    {
        _cards = new List<ICard>();
    }

    public void ReplaceAll(IEnumerable<ICard> cards)
    {
        _cards = new List<ICard>(cards);
    }

    public void Shuffle()
    {
        CheckEmpty();

        for (var i = 0; i < _cards.Count - 1; i++)
        {
            var j = random.Next(0, _cards.Count);
            (_cards[i], _cards[j]) = (cards[j], _cards[i]);
        }
    }

    public void PlaceTop(ICard card)
    {
        _cards.Add(card);
    }

    public void PlaceBottom(ICard card)
    {
        _cards.Insert(0, card);
    }

    public void InsertRandom(ICard card)
    {
        CheckEmpty();
        var i = random.Next(0, _cards.Count + 1);
        _cards.Insert(i, card);
    }

    public ICard DrawTop()
    {
        return PopAt(_topIndex);
    }

    public ICard DrawBottom()
    {
        return PopAt(_bottomIndex);
    }

    public ICard Remove(System.Enum suit, System.Enum rank)
    {
        var card = _cards.Find(c => c.Suit == suit && c.Rank == rank);

        if (card == null)
        {
            throw CardNotFoundException(); 
        }

        _cards.Remove(card)
    }

    private void CheckEmpty()
    {
        if (_cards.Count == 0)
        {
            throw DeckEmptyException();
        }
    }

    private ICard PopAt(int i)
    {
        CheckEmpty();
        var card = _cards[i];
        _cards.RemoveAt(i);
        return card;
    }
}