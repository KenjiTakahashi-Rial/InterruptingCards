public interface IGame
{
    IEnumerable<IPlayer> Players { get; set; }
    void Play();
}