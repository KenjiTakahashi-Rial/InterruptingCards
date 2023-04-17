using System;

public interface ICard
{
    System.Enum Suit { get; private set; }
    System.Enum Rank { get; private set }
}