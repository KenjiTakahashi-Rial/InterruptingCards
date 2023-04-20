using System;

using InterruptingCards.Models.Abstract;

namespace InterruptingCards.Controllers
{
    public class PlayerController<S, R> : AbstractPlayer<S, R> where S : Enum where R : Enum
    {
        public PlayerController(ulong id, string name, IHand<S, R> hand = null) : base(id, name, hand) { }
    }
}