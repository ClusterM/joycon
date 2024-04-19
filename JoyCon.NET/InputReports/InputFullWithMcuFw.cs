using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace wtf.cluster.JoyCon.InputReports;

/// <summary>
/// Standard full input report + MCU firmware update input report.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputFullWithMcuFw : InputFull
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
    private readonly byte[] mcuFwData;
    /// <summary>
    /// MCU FW update input report.
    /// </summary>
    public IReadOnlyList<byte> McuFwData => mcuFwData;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private InputFullWithMcuFw()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"MCU FW update: {string.Join(" ", mcuFwData.Select(b => b.ToString("X2")))}";
}
