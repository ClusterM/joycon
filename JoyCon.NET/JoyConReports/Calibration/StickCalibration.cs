using System.Linq;

namespace wtf.cluster.joycon.JoyConReports.Calibration;

/// <summary>
/// Stick calibration data.
/// </summary>
public class StickCalibration
{
    /// <summary>
    /// X Axis Min below center.
    /// </summary>
    public ushort XMin { get; set; }

    /// <summary>
    /// X Axis Center.
    /// </summary>
    public ushort XCenter { get; set; }

    /// <summary>
    /// X Axis Max above center.
    /// </summary>
    public ushort XMax { get; set; }

    /// <summary>
    /// Y Axis Min below center.
    /// </summary>
    public ushort YMin { get; set; }

    /// <summary>
    /// Y Axis Center.
    /// </summary>
    public ushort YCenter { get; set; }

    /// <summary>
    /// Y Axis Max above center.
    /// </summary>
    public ushort YMax { get; set; }

    internal static StickCalibration FromBytes(bool isRight, byte[] calibrationData)
    {
        var unpackedCalibrationData = JoyCon.UnpackUShort(calibrationData.ToArray());
        StickCalibration cal = !isRight
            ? new StickCalibration
            {
                XMax = unpackedCalibrationData[0],
                YMax = unpackedCalibrationData[1],
                XCenter = unpackedCalibrationData[2],
                YCenter = unpackedCalibrationData[3],
                XMin = unpackedCalibrationData[4],
                YMin = unpackedCalibrationData[5],
            }
            : new StickCalibration
            {
                XCenter = unpackedCalibrationData[0],
                YCenter = unpackedCalibrationData[1],
                XMin = unpackedCalibrationData[2],
                YMin = unpackedCalibrationData[3],
                XMax = unpackedCalibrationData[4],
                YMax = unpackedCalibrationData[5],
            };

        return cal;
    }

    /// <inheritdoc/>
    public override string ToString() => $"X: {XMin} - {XCenter} - {XMax}, Y: {YMin} - {YCenter} - {YMax}";
}
