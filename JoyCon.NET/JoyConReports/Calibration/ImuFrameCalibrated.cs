namespace wtf.cluster.joycon.JoyConReports.Calibration;

/// <summary>
/// Represents a calculated and calibrated IMU frame.
/// </summary>
public class ImuFrameCalibrated
{
    /// <summary>
    /// Acceleration on the X axis, in Gs.
    /// </summary>
    public double AccelX { get; internal set; }

    /// <summary>
    /// Acceleration on the Y axis, in Gs.
    /// </summary>
    public double AccelY { get; internal set; }

    /// <summary>
    /// Acceleration on the Z axis, in Gs.
    /// </summary>
    public double AccelZ { get; internal set; }

    /// <summary>
    /// Gyroscope value on the X axis, in degrees per second.
    /// </summary>
    public double GyroX { get; internal set; }

    /// <summary>
    /// Gyroscope value on the Y axis, in degrees per second.
    /// </summary>
    public double GyroY { get; internal set; }

    /// <summary>
    /// Gyroscope value on the Z axis, in degrees per second.
    /// </summary>
    public double GyroZ { get; internal set; }

    /// <inheritdoc/>
    public override string ToString() => $"Accel: ({AccelX:00.00}G, {AccelY:00.00}G, {AccelZ:00.00}G), Gyro: ({GyroX:000.00}°/s, {GyroY:000.00}°/s, {GyroZ:000.00}°/s)";
}
