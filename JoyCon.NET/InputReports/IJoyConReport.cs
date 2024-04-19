namespace wtf.cluster.JoyCon.InputReports;

/// <summary>
/// Represents a report from the controller.
/// </summary>
public interface IJoyConReport
{
    /// <summary>
    /// HID report ID.
    /// </summary>
    public byte ReportId { get; }
}
