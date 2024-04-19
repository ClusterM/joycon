using System.Runtime.InteropServices;

namespace wtf.cluster.JoyCon.InputReports;

/// <summary>
/// Standard full input report + subcommand reply.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class InputFullWithSubCmdReply : InputFull
{
    /// <summary>
    /// Reply to the last subcommand.
    /// </summary>
    public SubCmdReply SubcommandReply { get; }

    /// <inheritdoc/>
    public override string ToString() => SubcommandReply.ToString();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private InputFullWithSubCmdReply()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}
