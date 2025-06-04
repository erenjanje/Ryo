using OpenTK.Mathematics;

namespace Ryo.Tiles;

public class Atlas {
    public static Atlas Instance { get; } = new();
    public Texture Texture { get; } = new("Assets/TileMap.png");
    public const int CellUnit = 16;
    public readonly Vector2i CellSize = new(CellUnit, CellUnit);

    public Vector2 GetPosition(int x, int y) => (x, y) * this.CellSize;
    public Vector2 GetPosition(Vector2i coordinates) => coordinates * this.CellSize;

    private Atlas() { }
}