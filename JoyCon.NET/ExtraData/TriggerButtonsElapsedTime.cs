using System.Runtime.InteropServices;

namespace wtf.cluster.JoyCon.ExtraData;

/// <summary>
/// Elapsed time for each trigger button in milliseconds.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 14, Pack = 1)]
public class TriggerButtonsElapsedTime
{
    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort l;
    /// <summary>
    /// L button elapsed time in milliseconds.
    /// </summary>
    public int L => l * 10;

    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort r;
    /// <summary>
    /// R button elapsed time in milliseconds.
    /// </summary>
    public int R => r * 10;

    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort zl;
    /// <summary>
    /// ZL button elapsed time in milliseconds.
    /// </summary>
    public int ZL => zl * 10;

    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort zr;
    /// <summary>
    /// ZR button elapsed time in milliseconds.
    /// </summary>
    public int ZR => zr * 10;

    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort sl;
    /// <summary>
    /// SL button elapsed time in milliseconds.
    /// </summary>
    public int SL => sl * 10;

    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort sr;
    /// <summary>
    /// SR button elapsed time in milliseconds.
    /// </summary>
    public int SR => sr * 10;

    [MarshalAs(UnmanagedType.U2)]
    private readonly ushort home;
    /// <summary>
    /// Home button elapsed time in milliseconds.
    /// </summary>
    public int Home => home * 10;

    /// <inheritdoc/>
    public override string ToString() => $"L={L}ms, R={R}ms, ZL={ZL}ms, ZR={ZR}ms, SL={SL}ms, SR={SR}ms, Home={Home}ms";
}
