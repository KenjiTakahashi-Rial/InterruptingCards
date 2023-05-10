namespace InterruptingCards.Models
{
    [System.Serializable]
    public class PackCard
    {
// Must be public for JSON deserialization
#pragma warning disable S1104 // Fields should not have public accessibility
        public string Suit;

        public string Rank;

        public string Name;

        public int Count;
#pragma warning restore S1104 // Fields should not have public accessibility
    }
}