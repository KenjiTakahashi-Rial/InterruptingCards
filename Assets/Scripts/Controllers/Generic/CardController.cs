using System;

using InterruptingCards.Models.Abstract;

namespace InterruptingCards.Controllers
{
    public class CardController<S, R> : AbstractCard<S, R> where S : Enum where R : Enum
    {
        public CardController(S suit, R rank) : base(suit, rank) { }
    }
}