using InterruptingCards.Models;

namespace InterruptingCards.Models
{
    public class PlayerFactory
    {
        private PlayerFactory() { }

        public static PlayerFactory Singleton { get; } = new();

        public Player Create(ulong id, string name)
        {
            return new Player(id, name);
        }
    }
}
