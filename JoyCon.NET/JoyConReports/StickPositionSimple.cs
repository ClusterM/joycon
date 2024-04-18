using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Simple analog stick data with X and Y values.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 4, Pack = 1)]
public class StickPositionSimple : IStickPosition
{
    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort x;
    /// <summary>
    /// X value.
    /// </summary>
    public ushort X => (ushort)(x >> 4); /* lower 4 bits are not used */

    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort y;
    /// <summary>
    /// Y value.
    /// </summary>
    public ushort Y => (ushort)(y >> 4); /* lower 4 bits are not used */

    private StickPositionSimple()
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"X: {X}, Y: {Y}";
}
