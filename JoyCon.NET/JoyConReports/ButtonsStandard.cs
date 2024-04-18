using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Button states.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 3, Pack = 1)]
public class ButtonsStandard
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    private readonly byte[] buttons = new byte[3];
    /*
     Byte 0     Y           X           B           A           SR          SL          R           ZR
     Byte 1     Minus	    Plus	    R-Stick     L-Stick     Home        Capture     --          Charging Grip
     Byte 2     Down	    Up	        Right	    Left	    SR	        SL	        L	        ZL
    */

    /// <summary>
    /// Button Y pressed.
    /// </summary>
    public bool Y => (buttons[0] & 0x01) != 0;
    /// <summary>
    /// Button X pressed.
    /// </summary>
    public bool X => (buttons[0] & 0x02) != 0;
    /// <summary>
    /// Button B pressed.
    /// </summary>
    public bool B => (buttons[0] & 0x04) != 0;
    /// <summary>
    /// Button A pressed.
    /// </summary>
    public bool A => (buttons[0] & 0x08) != 0;
    /// <summary>
    /// Button SR pressed (left or right JoyCon).
    /// </summary>
    public bool SR => ((buttons[0] & 0x10) != 0) || ((buttons[2] & 0x10) != 0);
    /// <summary>
    /// Button SL pressed (left or right JoyCon).
    /// </summary>
    public bool SL => ((buttons[0] & 0x20) != 0) || ((buttons[2] & 0x20) != 0);
    /// <summary>
    /// Button R pressed.
    /// </summary>
    public bool R => (buttons[0] & 0x40) != 0;
    /// <summary>
    /// Button ZR pressed.
    /// </summary>
    public bool ZR => (buttons[0] & 0x80) != 0;
    /// <summary>
    /// Button Minus pressed.
    /// </summary>
    public bool Minus => (buttons[1] & 0x01) != 0;
    /// <summary>
    /// Button Plus pressed.
    /// </summary>
    public bool Plus => (buttons[1] & 0x02) != 0;
    /// <summary>
    /// Right stickpressed.
    /// </summary>
    public bool RStickClick => (buttons[1] & 0x04) != 0;
    /// <summary>
    /// Left stick pressed.
    /// </summary>
    public bool LStickClick => (buttons[1] & 0x08) != 0;
    /// <summary>
    /// Button Home pressed.
    /// </summary>
    public bool Home => (buttons[1] & 0x10) != 0;
    /// <summary>
    /// Button Capture pressed.
    /// </summary>
    public bool Capture => (buttons[1] & 0x20) != 0;
    /// <summary>
    /// Charging grip connected.
    /// </summary>
    public bool ChargingGrip => (buttons[1] & 0x80) != 0;
    /// <summary>
    /// D-Pad down pressed.
    /// </summary>
    public bool Down => (buttons[2] & 0x01) != 0;
    /// <summary>
    /// D-Pad up pressed.
    /// </summary>
    public bool Up => (buttons[2] & 0x02) != 0;
    /// <summary>
    /// D-Pad right pressed.
    /// </summary>
    public bool Right => (buttons[2] & 0x04) != 0;
    /// <summary>
    /// D-Pad left pressed.
    /// </summary>
    public bool Left => (buttons[2] & 0x08) != 0;
    /// <summary>
    /// Button L pressed.
    /// </summary>
    public bool L => (buttons[2] & 0x40) != 0;
    /// <summary>
    /// Button ZL pressed.
    /// </summary>
    public bool ZL => (buttons[2] & 0x80) != 0;

    private ButtonsStandard()
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"Y: {Y}, X: {X}, B: {B}, A: {A}, SR: {SR}, SL: {SL}, R: {R}, L: {L}, ZR: {ZR}, ZL: {ZL}, Minus: {Minus}, Plus: {Plus}, RStick: {RStickClick}, LStick: {LStickClick}, Home: {Home}, Capture: {Capture}, ChargingGrip: {ChargingGrip}, ↓: {Down}, ↑: {Up}, →: {Right}, ←: {Left}";
}
