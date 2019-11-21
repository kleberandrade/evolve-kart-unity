using System;

public class Gene : ICloneable
{
    public static readonly float MIN_DURATION = 0.05f;
    public static readonly float MAX_DURATION = 1.0f;
    public static readonly float MIN_HORIZONTAL = -1.0f;
    public static readonly float MAX_HORIZONTAL = 1.0f;
    public static readonly float MIN_VERTICAL = -1.0f;
    public static readonly float MAX_VERTICAL = 1.0f;

    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    public float Duration { get; set; }

    public Gene()
    {
        Horizontal = Helper.Next(MIN_HORIZONTAL, MAX_HORIZONTAL);
        Vertical = Helper.Next(MIN_VERTICAL, MAX_VERTICAL);
        Duration = Helper.Next(MIN_DURATION, MAX_DURATION);
    }

    public Gene(float horizontal, float vertical, float duration)
    {
        Horizontal = horizontal;
        Vertical = vertical;
        Duration = duration;
    }

    public object Clone()
    {
        var gene = (Gene)MemberwiseClone();
        return gene;
    }
}
