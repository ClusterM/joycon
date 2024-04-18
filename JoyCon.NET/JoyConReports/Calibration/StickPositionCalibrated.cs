namespace wtf.cluster.joycon.JoyConReports.Calibration;

/// <summary>
/// Simple struct to store calculated and calibrated stick position as X and Y values from -1 to 1.
/// </summary>
public struct StickPositionCalibrated
{
    /// <summary>
    /// X value of the stick position, from -1 to 1.
    /// </summary>
    public double X { get; internal set; }

    /// <summary>
    /// Y value of the stick position, from -1 to 1.
    /// </summary>
    public double Y { get; internal set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var xSign = X >= 0 ? "+" : string.Empty;
        var ySign = Y >= 0 ? "+" : string.Empty;
        return $"X: {xSign}{X * 100:0.}%, Y: {ySign}{Y * 100:0.}%";
    }
}
