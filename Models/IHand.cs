using System;

public interface IHand
{
    void Add(ICard card);

    ICard Remove(System.Enum Suit, System.Enum Rank);
}