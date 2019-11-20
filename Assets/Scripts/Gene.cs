using System;

public class Gene : ICloneable
{
    public static readonly float MIN_ACTION_DURATION = 0.05f;
    public static readonly float MAX_ACTION_DURATION = 1.0f;
    public static readonly float MIN_ACTION_VALUE = -1.0f;
    public static readonly float MAX_ACTION_VALUE = 1.0f;

    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    public float Duration { get; set; }

    public Gene()
    {
        Horizontal = Helper.Next(MIN_ACTION_VALUE, MAX_ACTION_VALUE);
        Vertical = Helper.Next(MIN_ACTION_VALUE, MAX_ACTION_VALUE);
        Duration = Helper.Next(MIN_ACTION_DURATION, MAX_ACTION_DURATION);
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
