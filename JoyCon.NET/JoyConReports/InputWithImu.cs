using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// IMU sensor data (accelerometer, gyroscope).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputWithImu : InputStandard
{
    /// <summary>
    /// IMU sensor data (accelerometer, gyroscope).
    /// </summary>
    public Imu Imu { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal InputWithImu()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => base.ToString() + $", IMU: ({Imu})";
}
