namespace wtf.cluster.JoyCon.Calibration;

/// <summary>
/// Calibration data for the controller.
/// </summary>
public class CalibrationData
{
    /// <summary>
    /// Left stick calibration data, can be null for user calibration. Also, can be invalid if controller has no left stick.
    /// </summary>
    public StickCalibration? LeftStickCalibration { get; set; }

    /// <summary>
    /// Right stick calibration data, can be null for user calibration. Also, can be invalid if controller has no right stick.
    /// </summary>
    public StickCalibration? RightStickCalibration { get; set; }

    /// <summary>
    /// IMU calibration data, can be null for user calibration.
    /// </summary>
    public ImuCalibration? ImuCalibration { get; set; }

    /// <summary>
    /// Combines two calibrations, taking the values from the second one if they are not null.
    /// </summary>
    /// <param name="a">First calibration.</param>
    /// <param name="b">Second calibration.</param>
    /// <returns>Combined calibration.</returns>
    public static CalibrationData operator +(CalibrationData a, CalibrationData b) => new()
    {
        LeftStickCalibration = b.LeftStickCalibration ?? a.LeftStickCalibration,
        RightStickCalibration = b.RightStickCalibration ?? a.RightStickCalibration,
        ImuCalibration = b.ImuCalibration ?? a.ImuCalibration
    };

    /// <inheritdoc/>
    public override string ToString() => $"Left stick: ({LeftStickCalibration}), Right stick: ({RightStickCalibration}), IMU: ({ImuCalibration})";
}
