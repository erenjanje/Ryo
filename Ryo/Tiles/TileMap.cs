using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo.Tiles;

public record TileMap {
    public interface IRequiredEvents : IEvent<GameEvents.Render>, IEvent<GameEvents.MouseDown>;

    public enum Type {
        Dirt,
        Grass,
        Count,
    }

    public record struct Tile(Type Type);

    public TileMap(IRequiredEvents events, int width, int height) {
        this.Width = width;
        this.Height = height;
        _tiles = new Tile[width, height];
        events.Event<GameEvents.Render>().Subscribe(this.OnRender);
        events.Event<GameEvents.MouseDown>().Subscribe(this.OnMouseDown);
    }

    private const int Base1 = 1;
    private const int Base2 = (int)Type.Count;
    private const int Base3 = Base2 * Base2;
    private const int Base4 = Base2 * Base2 * Base2;
    private const Type Border = Type.Dirt;
    private const int TileUnit = 32;
    private static readonly Vector2i TileSize = (TileUnit, TileUnit);

    private readonly Tile[,] _tiles;
    private int Width { get; }
    private int Height { get; }

    private int CoordinateToIndex(Vector2i coordinate) {
        var topLeft = (int)this[coordinate.X - 1, coordinate.Y - 1].Type;
        var topRight = (int)this[coordinate.X, coordinate.Y - 1].Type;
        var bottomLeft = (int)this[coordinate.X - 1, coordinate.Y].Type;
        var bottomRight = (int)this[coordinate.X, coordinate.Y].Type;
        return topLeft * Base1 + topRight * Base2 + bottomLeft * Base3 + bottomRight * Base4;
    }

    public Tile this[int x, int y] {
        get => x < 0 || x >= Width || y < 0 || y >= Height ? new Tile(Border) : _tiles[x, y];

        set {
            if (!(x < 0 || x >= Width || y < 0 || y >= Height)) _tiles[x, y] = value;
        }
    }

    private Tile this[Vector2i position] {
        get => this[position.X, position.Y];
        set => this[position.X, position.Y] = value;
    }

    private Vector2i FromScreenPosition(Vector2 position) =>
        (Vector2i)position / TileSize;

    private Vector2 ToScreenPosition(Vector2i coordinates) =>
        coordinates * TileSize - TileSize / 2;

    private void OnRender(object sender, GameEvents.Render args) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var index = this.CoordinateToIndex((x, y));
                args.Renderer.DrawTile(this.ToScreenPosition((x, y)), TileSize, (index, 0));
            }
        }
    }

    private void OnMouseDown(object sender, GameEvents.MouseDown args) {
        var coordinate = this.FromScreenPosition(args.MousePosition);
        this[coordinate] = new Tile(args.Button == MouseButton.Left ? Type.Dirt : Type.Grass);
    }
}