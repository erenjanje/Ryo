using System.Runtime.Intrinsics;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace Ryo.MotoRyo.Geometry;

public record struct Aabb(Vector2 Min, Vector2 Max) {
    public static Aabb FromPositionSize(Vector2 position, Vector2 size) => new(position, position + size);
    public (Vector2, Vector2) ToPositionSize() => (this.Min, this.Max - this.Min);

    public bool Intersects(Aabb aabb) {
        var l = Vector128.Create(this.Min.X, aabb.Min.X, this.Min.Y, aabb.Min.Y);
        var r = Vector128.Create(aabb.Max.X, this.Max.X, aabb.Max.Y, this.Max.Y);
        return !Vector128.GreaterThanAny(l, r);
    }
}