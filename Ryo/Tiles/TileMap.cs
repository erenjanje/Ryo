using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo.Tiles;

public sealed record TileMap : ITileMap {
    public TileMap(int width, int height) {
        this.Width = width;
        this.Height = height;
        _tiles = new ITileMap.Tile[width, height];
    }

    public void Register(IGameEvents events) {
        events.OnRender.Subscribe(this.OnRender);
        events.OnMouseDown.Subscribe(this.OnMouseDown);
    }

    private const int Base1 = 1;
    private const int Base2 = (int)ITileMap.Type.Count;
    private const int Base3 = Base2 * Base2;
    private const int Base4 = Base2 * Base2 * Base2;
    private const ITileMap.Type Border = ITileMap.Type.Dirt;
    private const int TileUnit = 32;
    private static readonly Vector2i TileSize = (TileUnit, TileUnit);

    private readonly ITileMap.Tile[,] _tiles;
    private int Width { get; }
    private int Height { get; }

    private int CoordinateToIndex(Vector2i coordinate) {
        var topLeft = (int)this[coordinate.X - 1, coordinate.Y - 1].Type;
        var topRight = (int)this[coordinate.X, coordinate.Y - 1].Type;
        var bottomLeft = (int)this[coordinate.X - 1, coordinate.Y].Type;
        var bottomRight = (int)this[coordinate.X, coordinate.Y].Type;
        return topLeft * Base1 + topRight * Base2 + bottomLeft * Base3 + bottomRight * Base4;
    }

    public ITileMap.Tile this[int x, int y] {
        get => x < 0 || x >= Width || y < 0 || y >= Height ? new ITileMap.Tile(Border) : _tiles[x, y];

        set {
            if (!(x < 0 || x >= Width || y < 0 || y >= Height)) _tiles[x, y] = value;
        }
    }

    public ITileMap.Tile this[Vector2i position] {
        get => this[position.X, position.Y];
        set => this[position.X, position.Y] = value;
    }

    private Vector2i FromScreenPosition(Vector2 position) =>
        (Vector2i)position / TileSize;

    private Vector2 ToScreenPosition(Vector2i coordinates) =>
        coordinates * TileSize - TileSize / 2;

    private void OnRender(object sender, IGameEvents.Render args) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var index = this.CoordinateToIndex((x, y));
                args.Renderer.DrawTile(this.ToScreenPosition((x, y)), TileSize, (index, 0));
            }
        }
    }

    private void OnMouseDown(object sender, IGameEvents.MouseDown args) {
        var coordinate = this.FromScreenPosition(args.MousePosition);
        this[coordinate] =
            new ITileMap.Tile(args.Button == MouseButton.Left ? ITileMap.Type.Dirt : ITileMap.Type.Grass);
    }
}