using System.Linq;

using UnityEngine;

using InterruptingCards.Behaviours;
using System.Collections.Generic;

namespace InterruptingCards.Managers
{
    public class PlayingCardHandManager : AbstractHandManager
    {
        [SerializeField] private List<PlayingCardBehaviour> _cardSlots;

        private IList<ICardBehaviour> _convertedCardSlots = null;

        protected override IList<ICardBehaviour> CardSlots
        {
            get
            {
                _convertedCardSlots ??= _cardSlots.Cast<ICardBehaviour>().ToList();
                return _convertedCardSlots;
            }
        }
    }
}
