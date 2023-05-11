using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class BasicPlayerFactory : IPlayerFactory<BasicCard, IHand<BasicCard>, BasicPlayer>
    {
        protected static readonly IPlayerFactory<BasicCard, IHand<BasicCard>, BasicPlayer>
            Instance = new BasicPlayerFactory();
        
        private BasicPlayerFactory() { }

        public static IPlayerFactory<BasicCard, IHand<BasicCard>, BasicPlayer> Singleton
        { 
            get { return Instance; } 
        }

        public virtual BasicPlayer Create(ulong id, string name)
        {
            return new BasicPlayer(id, name);
        }
    }
}
