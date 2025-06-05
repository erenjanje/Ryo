using OpenTK.Mathematics;

namespace Ryo.Tiles;

public interface ITileMap : IComponent {
    public enum Type {
        Dirt,
        Grass,
        Count,
    }

    public record struct Tile(Type Type);

    public Tile this[int x, int y] { get; set; }
    public Tile this[Vector2i coordinates] { get; set; }
}