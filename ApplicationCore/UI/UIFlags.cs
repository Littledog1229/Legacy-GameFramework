namespace ApplicationCore.UI; 

[Flags]
public enum UIFlags : uint {
    None        = 0b00000000000000000000000000000000,
    FocusDirty  = 0b00000000000000000000000000000001,
    FocusLocked = 0b00000000000000000000000000000010
}