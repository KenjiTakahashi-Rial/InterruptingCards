using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class BasicCardFactory : AbstractCardFactory<BasicCard>
    {
        protected static readonly ICardFactory<BasicCard> Instance = new BasicCardFactory();

        private BasicCardFactory() { }

        public static ICardFactory<BasicCard> Singleton { get { return Instance; } }

        protected override BasicCard FromMetadata(MetadataCard metadataCard)
        {
            return new BasicCard(metadataCard);
        }
    }
}
