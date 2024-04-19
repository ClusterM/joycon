using System.Runtime.InteropServices;
using wtf.cluster.JoyCon.InputData;

namespace wtf.cluster.JoyCon.InputReports;

/// <summary>
/// Standard full input report.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputFull : IJoyConReport
{
    /// <summary>
    /// Battery level.
    /// </summary>
    public enum BatteryLevels : byte
    {
        /// <summary>
        /// Empty.
        /// </summary>
        Empty = 0,
        /// <summary>
        /// Critical.
        /// </summary>
        Critical = 1,
        /// <summary>
        /// Low.
        /// </summary>
        Low = 2,
        /// <summary>
        /// Medium.
        /// </summary>
        Medium = 3,
        /// <summary>
        /// Full.
        /// </summary>
        Full = 4
    }

    /// <summary>
    /// Connection info.
    /// </summary>
    public enum ConnectionInfo : byte
    {
        /// <summary>
        /// Pro Controller or Charging Grip.
        /// </summary>
        Pro_or_ChrGrip = 0,
        /// <summary>
        /// Ring-Con. Maybe also some other accessories, not sure.
        /// </summary>
        RingCon = 1,
        /// <summary>
        /// Unknown. This value appears when Joy-Con halfways inserted into something.
        /// </summary>
        Unknown = 2,
        /// <summary>
        /// Joy-Con.
        /// </summary>
        JoyCon = 3
    }

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte reportId;
    /// <summary>
    /// HID report ID.
    /// </summary>
    public byte ReportId => reportId;

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte timer;
    /// <summary>
    /// Timer. Increments very fast. Can be used to estimate excess Bluetooth latency.
    /// </summary>
    public byte Timer => timer;

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte batteryAndConnectionInfo;
    /// <summary>
    /// Battery level.
    /// </summary>
    public BatteryLevels Battery => (BatteryLevels)(batteryAndConnectionInfo >> 5);

    /// <summary>
    /// Charging status.
    /// </summary>
    public bool Charging => (batteryAndConnectionInfo & 0b0001_0000) != 0;
    /// <summary>
    /// Connection info.
    /// </summary>
    public ConnectionInfo Connection => (ConnectionInfo)((batteryAndConnectionInfo >> 1) & 0b0000_0011);

    /// <summary>
    /// USB powered.
    /// </summary>
    public bool UsbPowered => (batteryAndConnectionInfo & 0b0000_0001) != 0;

    /// <summary>
    /// Button states.
    /// </summary>
    public ButtonsFull Buttons { get; }

    private readonly StickPositionStandard leftStick;
    /// <summary>
    /// Left stick state.
    /// </summary>
    public IStickPosition LeftStick => leftStick;

    private readonly StickPositionStandard rightStick;
    /// <summary>
    /// Right stick state.
    /// </summary>
    public IStickPosition RightStick => rightStick;

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte rumbleData;
    /// <summary>
    /// Rumble input report (???). Decides if next vibration pattern should be sent.
    /// </summary>
    public byte RumbleData => rumbleData;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal InputFull()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"LeftStick: ({LeftStick}), RightStick: ({RightStick}), Buttons: ({Buttons}), Battery: {Battery}, Charging: {Charging}";
}

