using System.Drawing;

namespace wtf.cluster.joycon;

/// <summary>
/// Colors of the controller.
/// </summary>
public class ControllerColors
{
    /// <summary>
    /// Color of the controller body.
    /// </summary>
    public Color BodyColor { get; set; }

    /// <summary>
    /// Color of the controller buttons.
    /// </summary>
    public Color ButtonsColor { get; set; }

    /// <summary>
    /// Color of the controller left grip (Pro controller only, since firmware 5.0.0).
    /// </summary>
    public Color LeftGripColor { get; set; }

    /// <summary>
    /// Color of the controller right grip (Pro controller only, since firmware 5.0.0).
    /// </summary>
    public Color RightGripColor { get; set; }

    /// <inheritdoc/>
    public override string ToString() => $"Body: {BodyColor}, Buttons: {ButtonsColor}, Left grip: {LeftGripColor}, Right grip: {RightGripColor}";
}
