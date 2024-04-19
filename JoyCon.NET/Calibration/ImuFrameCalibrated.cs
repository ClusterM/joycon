namespace wtf.cluster.JoyCon.Calibration;

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
    public override string ToString()
    {
        var accXSign = AccelX >= 0 ? "+" : string.Empty;
        var accYSign = AccelY >= 0 ? "+" : string.Empty;
        var accZSign = AccelZ >= 0 ? "+" : string.Empty;
        var gyroXSign = GyroX >= 0 ? "+" : string.Empty;
        var gyroYSign = GyroY >= 0 ? "+" : string.Empty;
        var gyroZSign = GyroZ >= 0 ? "+" : string.Empty;
        return $"Accel: ({accXSign}{AccelX:0.00}G, {accYSign}{AccelY:0.00}G, {accZSign}{AccelZ:0.00}G), Gyro: ({gyroXSign}{GyroX:000.00}°/s, {gyroYSign}{GyroY:000.00}°/s, {gyroZSign}{GyroZ:000.00}°/s)";
    }
}
