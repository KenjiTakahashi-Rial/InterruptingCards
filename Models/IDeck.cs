using System;

public interface IDeck
{
    void Shuffle();

    void PlaceTop(ICard card);

    void PlaceBottom(ICard card);

    void InsertRandom(ICard card);

    ICard DrawTop();

    ICard DrawBottom();

    ICard Remove(System.Enum suit, System.Enum rank);
}