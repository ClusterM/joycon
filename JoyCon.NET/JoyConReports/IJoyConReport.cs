namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Represents a report from a Joy-Con.
/// </summary>
public interface IJoyConReport
{
    /// <summary>
    /// HID report ID.
    /// </summary>
    public byte ReportId { get; }
}
