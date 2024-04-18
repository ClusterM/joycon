using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// IMU sensor data (accelerometer, gyroscope).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputWithImuExtra : InputWithImu
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 313)]
    private readonly byte[] extraData;
    /// NFC/IR data input report. Max 313 bytes.
    public IReadOnlyList<byte> ExtraData => extraData;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private InputWithImuExtra()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}
