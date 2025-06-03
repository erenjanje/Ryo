using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo;

public static class Utils {
    public static KeyModifiers Modifiers(this KeyboardKeyEventArgs e) {
        var ret = KeyModifiers.None;
        if (e.Shift) {
            ret |= KeyModifiers.Shift;
        }

        if (e.Control) {
            ret |= KeyModifiers.Control;
        }

        if (e.Alt) {
            ret |= KeyModifiers.Alt;
        }

        return ret;
    }

    [Flags]
    public enum KeyModifiers {
        None = 0,
        Shift = 1 << 0,
        Control = 1 << 1,
        Alt = 1 << 2,
    }

    public readonly record struct KeyData(Keys Key, KeyModifiers Modifiers);
}