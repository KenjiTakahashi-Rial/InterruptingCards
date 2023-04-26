using System.Linq;

using UnityEngine;

using InterruptingCards.Models;
using InterruptingCards.Behaviours;
using System.Collections.Generic;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardHandManager : AbstractHandManager<PlayingCardSuit, PlayingCardRank>
    {
        [SerializeField] private List<PlayingCardBehaviour> _cardSlots;

        private IList<ICardBehaviour<PlayingCardSuit, PlayingCardRank>> _convertedCardSlots;

        protected override IList<ICardBehaviour<PlayingCardSuit, PlayingCardRank>> CardSlots
        {
            get => _convertedCardSlots;
        }

        private void Awake()
        {
            _convertedCardSlots = _cardSlots.Cast<ICardBehaviour<PlayingCardSuit, PlayingCardRank>>().ToList();
        }
    }
}
