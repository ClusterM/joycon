using System.Runtime.InteropServices;
using wtf.cluster.joycon.JoyConReports.Calibration;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Accelerometer and gyroscope data frame.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 12, Pack = 1)]
public class ImuFrame
{
    [MarshalAs(UnmanagedType.I2)]
    private readonly short accelX;
    /// <summary>
    /// Accelerometer X value.
    /// </summary>
    public short AccelX => accelX;

    [MarshalAs(UnmanagedType.I2)]
    private readonly short accelY;
    /// <summary>
    /// Accelerometer Y value.
    /// </summary>
    public short AccelY => accelY;

    [MarshalAs(UnmanagedType.I2)]
    private readonly short accelZ;
    /// <summary>
    /// Accelerometer Z value.
    /// </summary>
    public short AccelZ => accelZ;

    [MarshalAs(UnmanagedType.I2)]
    private readonly short gyroX;
    /// <summary>
    /// Gyroscope X value.
    /// </summary>
    public short GyroX => gyroX;

    [MarshalAs(UnmanagedType.I2)]
    private readonly short gyroY;
    /// <summary>
    /// Gyroscope Y value.
    /// </summary>
    public short GyroY => gyroY;

    [MarshalAs(UnmanagedType.I2)]
    private readonly short gyroZ;
    /// <summary>
    /// Gyroscope Z value.
    /// </summary>
    public short GyroZ => gyroZ;

    private ImuFrame()
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"Accel: ({AccelX}, {AccelY}, {AccelZ}), Gyro: ({GyroX}, {GyroY}, {GyroZ})";

    /// <summary>
    /// Get the calibrated values for this frame.
    /// </summary>
    /// <param name="calibration">Calibration data.</param>
    /// <param name="accSensitivity">Accelerometer sensitivity, default is 8G.</param>
    /// <param name="gyroSensitivity">Gyroscope sensitivity, default is 2000DPS.</param>
    /// <returns></returns>
    public ImuFrameCalibrated GetCalibrated(ImuCalibration calibration, JoyCon.AccSensitivity accSensitivity = JoyCon.AccSensitivity.G8, JoyCon.GyroSensitivity gyroSensitivity = JoyCon.GyroSensitivity.DPS2000)
    {
        double accRange = accSensitivity switch
        {
            JoyCon.AccSensitivity.G2 => 2,
            JoyCon.AccSensitivity.G4 => 4,
            JoyCon.AccSensitivity.G8 => 8,
            JoyCon.AccSensitivity.G16 => 16,
            _ => 16
        };
        var accCoefX = calibration.AccSensivityX / 16384d;
        var accCoefY = calibration.AccSensivityY / 16384d;
        var accCoefZ = calibration.AccSensivityZ / 16384d;
        var accXG = (AccelX - calibration.AccOriginX) * accRange / short.MaxValue * accCoefX;
        var accYG = (AccelY - calibration.AccOriginY) * accRange / short.MaxValue * accCoefY;
        var accZG = (AccelZ - calibration.AccOriginZ) * accRange / short.MaxValue * accCoefZ;

        double gyroRange = gyroSensitivity switch
        {
            JoyCon.GyroSensitivity.DPS250 => 250,
            JoyCon.GyroSensitivity.DPS500 => 500,
            JoyCon.GyroSensitivity.DPS1000 => 1000,
            JoyCon.GyroSensitivity.DPS2000 => 2000,
            _ => 4000
        };
        var gyroCoefX = calibration.GyroSensivityX / 16384d;
        var gyroCoefY = calibration.GyroSensivityY / 16384d;
        var gyroCoefZ = calibration.GyroSensivityZ / 16384d;
        var gyroX = (GyroX - calibration.GyroOriginX) * gyroRange / short.MaxValue * gyroCoefX;
        var gyroY = (GyroY - calibration.GyroOriginY) * gyroRange / short.MaxValue * gyroCoefY;
        var gyroZ = (GyroZ - calibration.GyroOriginZ) * gyroRange / short.MaxValue * gyroCoefZ;

        return new ImuFrameCalibrated
        {
            AccelX = accXG,
            AccelY = accYG,
            AccelZ = accZG,
            GyroX = gyroX,
            GyroY = gyroY,
            GyroZ = gyroZ
        };
    }
}
