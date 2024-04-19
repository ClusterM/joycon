using System.Linq;

namespace wtf.cluster.JoyCon.Rumble;

/// <summary>
/// Set of rumble data for left and right rumble actuators.
/// </summary>
public class RumbleSet
{
    /// <summary>
    /// Left rumble actuator frequency and amplitude.
    /// </summary>
    public RumbleData LeftRumble { get; set; }

    /// <summary>
    /// Right rumble actuator frequency and amplitude.
    /// </summary>
    public RumbleData RightRumble { get; set; }

    /// <summary>
    /// Creates a new rumble set.
    /// </summary>
    /// <param name="leftRumble">Left rumble actuator frequency and amplitude.</param>
    /// <param name="rightRumble">Right rumble actuator frequency and amplitude.</param>
    public RumbleSet(RumbleData? leftRumble, RumbleData? rightRumble)
    {
        LeftRumble = leftRumble ?? new RumbleData();
        RightRumble = rightRumble ?? new RumbleData();
    }

    /// <summary>
    /// Creates a new rumble set.
    /// </summary>
    /// <param name="freqLeft">Left rumble actuator frequency.</param>
    /// <param name="ampLeft">Left rumble actuator amplitude.</param>
    /// <param name="freqRight">Right rumble actuator frequency.</param>
    /// <param name="ampRight">Right rumble actuator amplitude.</param>
    public RumbleSet(double freqLeft, double ampLeft, double freqRight, double ampRight)
    {
        LeftRumble = new RumbleData(freqLeft, ampLeft);
        RightRumble = new RumbleData(freqRight, ampRight);
    }

    /// <summary>
    /// Creates a new rumble set with the same frequency and amplitude for both rubmle actuators.
    /// </summary>
    /// <param name="freqBoth">Frequency.</param>
    /// <param name="ampBoth">Amplitude.</param>
    public RumbleSet(double freqBoth, double ampBoth)
    {
        LeftRumble = new RumbleData(freqBoth, ampBoth);
        RightRumble = new RumbleData(freqBoth, ampBoth);
    }

    /// <summary>
    /// Creates a new rumble set with disabled rumble.
    /// </summary>
    public RumbleSet()
    {
        LeftRumble = new RumbleData();
        RightRumble = new RumbleData();
    }

    /// <summary>
    /// Returns raw data
    /// </summary>
    /// <returns>Data</returns>
    public byte[] ToBytes() => LeftRumble.ToBytes().Concat(RightRumble.ToBytes()).ToArray();

    /// <inheritdoc/>
    public override string ToString() => $"Left: {LeftRumble}, Right: {RightRumble}";
}
