using System.ComponentModel;
using OpenTK.Mathematics;

namespace Ryo.MotoRyo.Rendering;

public interface IRenderer : IComponent {
    void Draw(Vector2 position, Vector2 size, Vector2 texturePosition, Vector2 textureSize);
    void DrawTile(Vector2 position, Vector2i atlasPosition);
    void DrawTile(Vector2 position, Vector2 size, Vector2i atlasPosition);
    void Render();
}