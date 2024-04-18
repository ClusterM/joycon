using wtf.cluster.joycon.JoyConReports.Calibration;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Stick position: X and Y values.
/// </summary>
public interface IStickPosition
{
    /// <summary>
    /// Stick X value.
    /// </summary>
    public ushort X { get; }

    /// <summary>
    /// Stick Y value.
    /// </summary>
    public ushort Y { get; }

    /// <summary>
    /// Returns calculated and calibrated stick position as values from -1 to 1.
    /// </summary>
    /// <param name="calibration">Calibration data as <see cref="StickCalibration"/>, can be acquired using <see cref="JoyCon.GetFactoryCalibrationAsync"/> and <see cref="JoyCon.GetUserCalibrationAsync"/>.</param>
    /// <param name="deadZone">Deadzone value, can be acquired using <see cref="JoyCon.GetSticksParametersAsync"/>, but it's near the same for all controllers. Default is 200.</param>
    /// <returns></returns>
    public StickPositionCalibrated GetCalibrated(StickCalibration calibration, uint deadZone = 200)
    {
        //double deadZoneRadius = Math.Sqrt(deadZone * deadZone);
        double x;
        double y;

        /* square deadzone */
        if ((X >= calibration.XCenter && X <= calibration.XCenter + deadZone) || (X <= calibration.XCenter && X >= calibration.XCenter - deadZone))
        {
            x = 0;
        }
        else if (X < calibration.XCenter)
        {
            var centerNonDeadZone = calibration.XCenter - deadZone;
            x = -(double)(centerNonDeadZone - X) / (centerNonDeadZone - (calibration.XCenter - calibration.XMin));
        }
        else
        {
            var centerNonDeadZone = calibration.XCenter + deadZone;
            x = (X - centerNonDeadZone) / (double)(calibration.XCenter + calibration.XMax - centerNonDeadZone);
        }
        if ((Y >= calibration.YCenter && Y <= calibration.YCenter + deadZone) || (Y <= calibration.YCenter && Y >= calibration.YCenter - deadZone))
        {
            y = 0;
        }
        else if (Y < calibration.YCenter)
        {
            var centerNonDeadZone = calibration.YCenter - deadZone;
            y = -(double)(centerNonDeadZone - Y) / (centerNonDeadZone - (calibration.YCenter - calibration.YMin));
        }
        else
        {
            var centerNonDeadZone = calibration.YCenter + deadZone;
            y = (Y - centerNonDeadZone) / (double)(calibration.YCenter + calibration.YMax - centerNonDeadZone);
        }

        // TODO: radial deadzone?

        return new StickPositionCalibrated { X = x, Y = y };
    }
}
