namespace wtf.cluster.JoyCon.Calibration;

/// <summary>
/// Parameters for the stick (dead-zone and range ratio).
/// </summary>
public class StickParameters
{
    /// <summary>
    /// It is used to all directions, so it isn't divided by 2. It behaves like a radial dead-zone. Changing it as big as a half axis range, produces a circular d-pad style behavior. The default values for Joy-Con translates to ~15% and ~10% for Pro controller.
    /// </summary>
    public ushort DeadZone { get; private set; }

    /// <summary>
    /// Making this very small, produces d-pad like movement on the cross but still retains circular directionality. So it probably produces a float coefficient.
    /// </summary>
    public ushort RangeRatio { get; private set; }

    internal static StickParameters FromBytes(byte[] data)
    {
        var unpackedData = JoyCon.UnpackUShort(data);
        return new StickParameters
        {
            DeadZone = unpackedData[2],
            RangeRatio = unpackedData[3]
        };
    }

    /// <inheritdoc/>
    public override string ToString() => $"DeadZone: {DeadZone}, RangeRatio: {RangeRatio}";
}