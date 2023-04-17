public interface IPlayer
{
    string Name { get; private set; }
    
    IHand Hand { get; set; }
}