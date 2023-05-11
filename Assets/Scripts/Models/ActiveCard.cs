using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class ActiveCard : BasicCard, IActiveCard
    {
        public ActiveCard(MetadataCard metadataCard) : base(metadataCard)
        {
            ActiveEffect = metadataCard.ActiveEffect;
        }

        public virtual CardActiveEffect ActiveEffect { get; }
    }
}