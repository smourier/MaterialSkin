namespace MaterialSkin.Animations;

internal static class AnimationEaseInOut
{
    public static double CalculateProgress(double progress) => EaseInOut(progress);
    private static double EaseInOut(double s) => s - Math.Sin(s * 2 * Math.PI) / (2 * Math.PI);
}

public static class AnimationEaseOut
{
    public static double CalculateProgress(double progress) => -1 * progress * (progress - 2);
}

public static class AnimationCustomQuadratic
{
    public static double CalculateProgress(double progress)
    {
        var kickoff = 0.6;
        return 1 - Math.Cos((Math.Max(progress, kickoff) - kickoff) * Math.PI / (2 - (2 * kickoff)));
    }
}