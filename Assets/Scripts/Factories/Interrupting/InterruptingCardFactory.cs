using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class InterruptingCardFactory : AbstractCardFactory<InterruptingCard>
    {
        protected static readonly ICardFactory<InterruptingCard> Instance = new InterruptingCardFactory();

        private InterruptingCardFactory() { }

        public static ICardFactory<InterruptingCard> Singleton { get { return Instance; } }

        protected override InterruptingCard FromMetadata(MetadataCard metadataCard)
        {
            return new InterruptingCard(metadataCard);
        }
    }
}
