using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// NFC/IR MCU FW update input report.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputWithNfcIrMcuFw : InputStandard
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
    private readonly byte[] nfcIrMcuFwData;
    /// <summary>
    /// NFC/IR MCU FW update input report.
    /// </summary>
    public IReadOnlyList<byte> NfcIrMcuFwData => nfcIrMcuFwData;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private InputWithNfcIrMcuFw()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"NFC/IR MCU FW update: {string.Join(" ", nfcIrMcuFwData.Select(b => b.ToString("X2")))}";
}
