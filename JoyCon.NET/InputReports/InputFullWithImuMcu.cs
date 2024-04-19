using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace wtf.cluster.JoyCon.InputReports;

/// <summary>
/// Standatd full input report + IMU sensor data (accelerometer, gyroscope) + MCU data.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputFullWithImuMcu : InputFullWithImu
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 313)]
    private readonly byte[] extraData;
    /// <summary>
    /// MCU data input report. Max 313 bytes.
    /// </summary>
    public IReadOnlyList<byte> ExtraData => extraData;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private InputFullWithImuMcu()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}
