using System;
using System.IO;
using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Represents a simple input report from a Joy-Con controller.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 12, Pack = 1)]
public class InputSimple : IJoyConReport
{
    /// <summary>
    /// The possible positions of the stick.
    /// </summary>
    public enum HatPosition : byte
    {
        /// <summary>
        /// Stick is in the up position.
        /// </summary>
        Up = 0x00,
        /// <summary>
        /// Stick is in the up-right position.
        /// </summary>
        UpRight = 0x01,
        /// <summary>
        /// Stick is in the right position.
        /// </summary>
        Right = 0x02,
        /// <summary>
        /// Stick is in the down-right position.
        /// </summary>
        DownRight = 0x03,
        /// <summary>
        /// Stick is in the down position.
        /// </summary>
        Down = 0x04,
        /// <summary>
        /// Stick is in the down-left position.
        /// </summary>
        DownLeft = 0x05,
        /// <summary>
        /// Stick is in the left position.
        /// </summary>
        Left = 0x06,
        /// <summary>
        /// Stick is in the up-left position.
        /// </summary>
        UpLeft = 0x07,
        /// <summary>
        /// Stick is in the center position.
        /// </summary>
        Center = 0x08
    }

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte reportId;

    /// <summary>
    /// Gets the HID report ID.
    /// </summary>
    public byte ReportId => reportId;

    /// <summary>
    /// Gets the simple button data.
    /// </summary>
    public ButtonsSimple Buttons { get; }

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte stickHatData;

    /// <summary>
    /// Gets the simple stick hat data as when controller is held sideways.
    /// </summary>
    public HatPosition StickHatPosition => (HatPosition)stickHatData;

    private readonly StickPositionSimple leftStick;

    /// <summary>
    /// Gets the left stick data of the Pro Controller.
    /// </summary>
    public IStickPosition ProConLeftStick => leftStick;

    private readonly StickPositionSimple rightStick;

    /// <summary>
    /// Gets the right stick data of the Pro Controller.
    /// </summary>
    public IStickPosition ProConRightStick => rightStick;

    /// <summary>
    /// Creates an InputSimple object from raw data.
    /// </summary>
    /// <param name="data">The raw data.</param>
    /// <returns>The created InputSimple object.</returns>
    /// <exception cref="InvalidDataException">Thrown when there is not enough data to fill the InputSimple class.</exception>
    public static InputSimple FromBytes(byte[] data)
    {
        var rawsize = Marshal.SizeOf(typeof(InputSimple));
        if (data.Length < rawsize)
        {
            throw new InvalidDataException("Not enough data to fill InputSimple class. Array length: " + data.Length + ", struct length: " + rawsize);
        }
        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.Copy(data, 0, buffer, rawsize);
        var retobj = (InputSimple)Marshal.PtrToStructure(buffer, typeof(InputSimple));
        Marshal.FreeHGlobal(buffer);
        return retobj;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private InputSimple()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"Buttons: ({Buttons}), StickHatData: {StickHatPosition}, ProConLeftStick: ({ProConLeftStick}), ProConRightStick: ({ProConRightStick})";
}
