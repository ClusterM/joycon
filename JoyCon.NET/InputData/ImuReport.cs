using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace wtf.cluster.JoyCon.InputData;

/// <summary>
/// Accelerometer and gyroscope data, 3 frames with 5ms interval.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 36, Pack = 1)]
public class ImuReport
{
    private readonly ImuFrame imuFrame1;
    private readonly ImuFrame imuFrame2;
    private readonly ImuFrame imuFrame3;

    /// <summary>
    /// IMU sensor data frames (always 3).
    /// </summary>
    public IReadOnlyList<ImuFrame> Frames => [imuFrame1, imuFrame2, imuFrame3];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ImuReport()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"Frame 1: ({imuFrame1}), frame 2: ({imuFrame2}), frame 3: ({imuFrame3})";
}
