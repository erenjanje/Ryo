namespace Ryo.EventArgs;

public record struct Load;

public record struct Update(double DeltaTime);

public record struct Render;

public record struct KeyUp(Utils.KeyData Key);

public record struct KeyDown(Utils.KeyData Key);