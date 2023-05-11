using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class InterruptingCard : BasicCard, IActiveCard
    {
        // Do not call directly; use a factory
        public InterruptingCard(MetadataCard metadataCard) : base(metadataCard)
        {
            ActiveEffect = metadataCard.ActiveEffect;
        }

        public virtual CardActiveEffect ActiveEffect { get; }
    }
}