namespace wtf.cluster.JoyCon.Calibration;

/// <summary>
/// Parameters for the left and right sticks.
/// </summary>
public class StickParametersSet
{
    /// <summary>
    /// Parameters for the left stick.
    /// </summary>
    public StickParameters LeftStickParameters { get; internal set; }

    /// <summary>
    /// Parameters for the right stick.
    /// </summary>
    public StickParameters RightStickParameters { get; internal set; }

    internal StickParametersSet(StickParameters leftStickParameters, StickParameters rightStickParameters)
    {
        LeftStickParameters = leftStickParameters;
        RightStickParameters = rightStickParameters;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Left: {LeftStickParameters}, Right: {RightStickParameters}";
}
