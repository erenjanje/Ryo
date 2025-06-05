namespace Ryo;

public interface IComponent {
    void Register(ref IGameEvents events);
}