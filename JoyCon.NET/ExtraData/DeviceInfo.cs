using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace wtf.cluster.JoyCon.ExtraData;

/// <summary>
/// Device information.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 12, Pack = 1)]
public class DeviceInfo
{
    /// <summary>
    /// Controller type.
    /// </summary>
    public enum Controller : byte
    {
        /// <summary>
        /// Left Joy-Con.
        /// </summary>
        JoyConLeft = 0x01,
        /// <summary>
        /// Right Joy-Con.
        /// </summary>
        JoyConRight = 0x02,
        /// <summary>
        /// Pro Controller.
        /// </summary>
        ProController = 0x03
    }

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte firmwareVersionMajor;
    /// <summary>
    /// Controller firmware version (major).
    /// </summary>
    public byte FirmwareVersionMajor => firmwareVersionMajor;

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte firmwareVersionMinor;
    /// <summary>
    /// Controller firmware version (minor).
    /// </summary>
    public byte FirmwareVersionMinor => firmwareVersionMinor;

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte contollerType;
    /// <summary>
    /// Controller type.
    /// </summary>
    public Controller ControllerType => (Controller)contollerType;

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte unknown1; // Seems to be always 02.

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    private readonly byte[] macAddress;
    /// <summary>
    /// Controller MAC address.
    /// </summary>
    public IReadOnlyList<byte> MacAddress => macAddress;

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte unknown2; // Seems to be always 01.

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte colorsAreUsed;
    /// <summary>
    /// Colors in SPI are used.
    /// </summary>
    public bool ColorsAreUsed => colorsAreUsed == 1;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private DeviceInfo()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"{ControllerType}, firmware: v{FirmwareVersionMajor}.{FirmwareVersionMinor}, MAC: {string.Join(":", MacAddress.Select(b => $"{b:X2}"))}";
}
