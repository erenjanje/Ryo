using OpenTK.Mathematics;

namespace Ryo.Tiles;

public class Atlas {
    public Texture Texture { get; } = new("Assets/TileMap.png");
    public const int CellUnit = 16;
    public static readonly Vector2i CellSize = new(CellUnit, CellUnit);

    public Vector2 GetPosition(int x, int y) => (x, y) * CellSize;
    public Vector2 GetPosition(Vector2i coordinates) => coordinates * CellSize;
}