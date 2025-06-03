using System.Runtime.CompilerServices;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo;

public static class Utils {
    public readonly record struct KeyData(Keys Key, KeyModifiers Modifiers);

    public class Wrapper<T>(T initialValue) {
        public T Value { get; set; } = initialValue;
    }

    public class ThreadSafeWrapper<T>(T initialValue) {
        public T Value {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        } = initialValue;
    }

    public static Event<T> Event<T>(this IEvent<T> self) where T : struct {
        return self.On;
    }
}