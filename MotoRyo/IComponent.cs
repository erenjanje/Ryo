using Ryo.MotoRyo.Event;

namespace Ryo.MotoRyo;

public interface IComponent {
    void Register(IGameEvents events);
}