namespace Ryo;

public interface IComponent {
    void Register(ref GameEvents events);
}