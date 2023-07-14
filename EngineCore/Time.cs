namespace Engine; 

public static class Time {
    public static float  DeltaTime        { get; internal set; }
    public static double PreciseDeltaTime { get; internal set; }
    public static double FixedDeltaTime   { get; internal set; }
}