namespace Engine.UI; 

[Flags]
public enum UIFlags : byte {
    None        = 0b00000000,
    FocusDirty  = 0b00000001,
    FocusLocked = 0b00000010
}

public static class UIFlagsExtensions {
    public static bool hasFlag   (this     UIFlags flags, UIFlags flag) => (flags & flag) > 0;
    public static void toggleFlag(ref this UIFlags flags, UIFlags flag) => flags ^= flag;
    public static void addFlag   (ref this UIFlags flags, UIFlags flag) => flags ^= flag;
    public static void removeFlag(ref this UIFlags flags, UIFlags flag) => flags &= ~flag;
}