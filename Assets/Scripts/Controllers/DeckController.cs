using System;

using UnityEngine;

using InterruptingCards.Models.Abstract;

namespace InterruptingCards.Controllers
{
    public class DeckController<S, R> : AbstractDeck<S, R> where S : Enum where R : Enum
    {
        [SerializeField] private CardController _topCard;


    }
}
