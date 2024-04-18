using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports.Calibration;

/// <summary>
/// IMU calibration data: accelerometer and gyroscope.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 24, Pack = 1)]
public class ImuCalibration
{
    [MarshalAs(UnmanagedType.I2)]
    private short accOriginX;

    [MarshalAs(UnmanagedType.I2)]
    private short accOriginY;

    [MarshalAs(UnmanagedType.I2)]
    private short accOriginZ;

    [MarshalAs(UnmanagedType.I2)]
    private short accSensivityX;

    [MarshalAs(UnmanagedType.I2)]
    private short accSensivityY;

    [MarshalAs(UnmanagedType.I2)]
    private short accSensivityZ;

    [MarshalAs(UnmanagedType.I2)]
    private short gyroOriginX;

    [MarshalAs(UnmanagedType.I2)]
    private short gyroOriginY;

    [MarshalAs(UnmanagedType.I2)]
    private short gyroOriginZ;

    [MarshalAs(UnmanagedType.I2)]
    private short gyroSensivityX;

    [MarshalAs(UnmanagedType.I2)]
    private short gyroSensivityY;

    [MarshalAs(UnmanagedType.I2)]
    private short gyroSensivityZ;

    /// <summary>
    /// Accelerometer X origin position when completely horizontal and stick is upside.
    /// </summary>
    public short AccOriginX { get => accOriginX; set => accOriginX = value; }

    /// <summary>
    /// Accelerometer Y origin position when completely horizontal and stick is upside.
    /// </summary>
    public short AccOriginY { get => accOriginY; set => accOriginY = value; }

    /// <summary>
    /// Accelerometer Z origin position when completely horizontal and stick is upside.
    /// </summary>
    public short AccOriginZ { get => accOriginZ; set => accOriginZ = value; }

    /// <summary>
    /// Accelerometer X sensitivity special coeff, for default sensitivity: ±8G.
    /// </summary>
    public short AccSensivityX { get => accSensivityX; set => accSensivityX = value; }

    /// <summary>
    /// Accelerometer Y sensitivity special coeff, for default sensitivity: ±8G.
    /// </summary>
    public short AccSensivityY { get => accSensivityY; set => accSensivityY = value; }

    /// <summary>
    /// Accelerometer Z sensitivity special coeff, for default sensitivity: ±8G.
    /// </summary>
    public short AccSensivityZ { get => accSensivityZ; set => accSensivityZ = value; }

    /// <summary>
    /// Gyroscope X origin position when completely horizontal and stick is upside.
    /// </summary>
    public short GyroOriginX { get => gyroOriginX; set => gyroOriginX = value; }

    /// <summary>
    /// Gyroscope Y origin position when completely horizontal and stick is upside.
    /// </summary>
    public short GyroOriginY { get => gyroOriginY; set => gyroOriginY = value; }

    /// <summary>
    /// Gyroscope Z origin position when completely horizontal and stick is upside.
    /// </summary>
    public short GyroOriginZ { get => gyroOriginZ; set => gyroOriginZ = value; }

    /// <summary>
    /// Gyroscope X sensitivity special coeff, for default sensitivity: ±2000dps.
    /// </summary>
    public short GyroSensivityX { get => gyroSensivityX; set => gyroSensivityX = value; }

    /// <summary>
    /// Gyroscope Y sensitivity special coeff, for default sensitivity: ±2000dps.
    /// </summary>
    public short GyroSensivityY { get => gyroSensivityY; set => gyroSensivityY = value; }

    /// <summary>
    /// Gyroscope Z sensitivity special coeff, for default sensitivity: ±2000dps.
    /// </summary>
    public short GyroSensivityZ { get => gyroSensivityZ; set => gyroSensivityZ = value; }

    /// <inheritdoc/>
    public override string ToString() => $"Accel: ({AccOriginX}, {AccOriginY}, {AccOriginZ}), Gyro: ({GyroOriginX}, {GyroOriginY}, {GyroOriginZ})";
}
