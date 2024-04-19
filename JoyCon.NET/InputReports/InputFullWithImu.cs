using System.Runtime.InteropServices;
using wtf.cluster.JoyCon.InputData;

namespace wtf.cluster.JoyCon.InputReports;

/// <summary>
/// Standard full input report + IMU sensor data (accelerometer, gyroscope).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputFullWithImu : InputFull
{
    /// <summary>
    /// IMU sensor data (accelerometer, gyroscope).
    /// </summary>
    public ImuReport Imu { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal InputFullWithImu()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => base.ToString() + $", IMU: ({Imu})";
}
