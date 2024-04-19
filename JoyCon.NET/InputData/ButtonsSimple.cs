using System.Runtime.InteropServices;

namespace wtf.cluster.JoyCon.InputData;

/// <summary>
/// Buttons state, simple version.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 2, Pack = 1)]
public class ButtonsSimple
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private readonly byte[] buttons;

    /*
    Byte 0  Down	Right	Left	Up	SL	SR	--	--
    Byte 1	Minus	Plus	Left Stick	Right Stick	Home	Capture	L / R	ZL / ZR
    */

    /// <summary>
    /// Gets the state of the lower button ("Down" or "A") as when controller is held sideways.
    /// </summary>
    public bool Down => (buttons[0] & 0x01) != 0;

    /// <summary>
    /// Gets the state of the right button ("Right" or "X") as when controller is held sideways.
    /// </summary>
    public bool Right => (buttons[0] & 0x02) != 0;

    /// <summary>
    /// Gets the state of the left button ("Left" or "B") as when controller is held sideways.
    /// </summary>
    public bool Left => (buttons[0] & 0x04) != 0;

    /// <summary>
    /// Gets the state of the upper button ("Up" or "Y") as when controller is held sideways.
    /// </summary>
    public bool Up => (buttons[0] & 0x08) != 0;

    /// <summary>
    /// Button SL pressed.
    /// </summary>
    public bool SL => (buttons[0] & 0x10) != 0;

    /// <summary>
    /// Button SR pressed.
    /// </summary>
    public bool SR => (buttons[0] & 0x20) != 0;

    /// <summary>
    /// Button Minus pressed.
    /// </summary>
    public bool Minus => (buttons[1] & 0x01) != 0;

    /// <summary>
    /// Button Plus pressed.
    /// </summary>
    public bool Plus => (buttons[1] & 0x02) != 0;

    /// <summary>
    /// Left stick pressed.
    /// </summary>
    public bool LStickClick => (buttons[1] & 0x04) != 0;

    /// <summary>
    /// Right stickpressed.
    /// </summary>
    public bool RStickClick => (buttons[1] & 0x08) != 0;

    /// <summary>
    /// Button Home pressed.
    /// </summary>
    public bool Home => (buttons[1] & 0x10) != 0;

    /// <summary>
    /// Button Capture pressed.
    /// </summary>
    public bool Capture => (buttons[1] & 0x20) != 0;

    /// <summary>
    /// Button L or R pressed.
    /// </summary>
    public bool LorR => (buttons[1] & 0x40) != 0;

    /// <summary>
    /// Button ZL or ZR pressed.
    /// </summary>
    public bool ZLorZR => (buttons[1] & 0x80) != 0;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ButtonsSimple()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"↑: {Up}, ↓: {Down}, ←: {Left}, →: {Right}, SL: {SL}, SR: {SR}, Minus: {Minus}, Plus: {Plus}, Home: {Home}, Capture: {Capture}, L/R: {LorR}, ZL/ZR: {ZLorZR}ss, LStick: {LStickClick}, RStick: {RStickClick}";
}
