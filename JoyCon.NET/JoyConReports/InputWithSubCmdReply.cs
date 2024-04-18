using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Standard input report + subcommand reply.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputWithSubCmdReply : InputStandard
{
    /// <summary>
    /// Reply to the last subcommand.
    /// </summary>
    public SubCmdReply SubcommandReply { get; }

    /// <inheritdoc/>
    public override string ToString() => SubcommandReply.ToString();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private InputWithSubCmdReply()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}
