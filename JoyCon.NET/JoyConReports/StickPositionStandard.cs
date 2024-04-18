using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Standard analog stick data with X and Y values.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 3, Pack = 1)]
public class StickPositionStandard : IStickPosition
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    private readonly byte[] leftStick;

    /// <summary>
    /// Stick X value.
    /// </summary>
    public ushort X => (ushort)(leftStick[0] | ((leftStick[1] & 0xF) << 8));

    /// <summary>
    /// Stick Y value.
    /// </summary>
    public ushort Y => (ushort)((leftStick[1] >> 4) | (leftStick[2] << 4));

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private StickPositionStandard()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"X: {X}, Y: {Y}";
}
