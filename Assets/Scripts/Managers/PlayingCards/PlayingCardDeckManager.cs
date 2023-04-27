using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class PlayingCardDeckManager : AbstractDeckManager
    {
        private static readonly ICollection<SuitEnum> SuitVals =
            Enum.GetValues(typeof(SuitEnum))
                .Cast<SuitEnum>()
                .Where(e => e >= SuitEnum.Clubs && e <= SuitEnum.Spades)
                .ToList();

        private static readonly ICollection<RankEnum> RankVals =
            Enum.GetValues(typeof(RankEnum))
                .Cast<RankEnum>()
                .Where(e => e >= RankEnum.Ace && e <= RankEnum.King)
                .ToList();

        [SerializeField] private PlayingCardBehaviour _topCard;

        protected override ICardFactory CardFactory => PlayingCardFactory.Singleton;

        protected override IDeckFactory DeckFactory => PlayingCardDeckFactory.Singleton;

        protected override ICardBehaviour TopCard => _topCard;

        protected override ICollection<SuitEnum> Suits => SuitVals;

        protected override ICollection<RankEnum> Ranks => RankVals;
    }
}
