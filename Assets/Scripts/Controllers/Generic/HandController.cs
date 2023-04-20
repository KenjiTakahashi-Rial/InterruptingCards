using System;

using InterruptingCards.Models.Abstract;

namespace InterruptingCards.Controllers
{
    public class HandController<S, R> : AbstractHand<S, R> where S : Enum where R : Enum { }
}
