using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace wtf.cluster.joycon.JoyConReports;

/// <summary>
/// Reply to a subcommand (report 0x21).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class SubCmdReply
{
    [MarshalAs(UnmanagedType.U1)]
    private readonly byte subCmdAck;
    /// <summary>
    /// Last subcommand is acknowledged.
    /// </summary>
    public bool Acknowledged => (subCmdAck & 0b1000_0000) != 0;
    /// <summary>
    /// Subcommand reply data type (???).
    /// </summary>
    public byte SubCmdDataType => (byte)(subCmdAck & 0b0111_1111);

    [MarshalAs(UnmanagedType.U1)]
    private readonly byte subCmdID;
    /// <summary>
    /// Reply-to subcommand ID.
    /// </summary>
    public JoyCon.Subcommand SubcommandID => (JoyCon.Subcommand)subCmdID;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 35)]
    private readonly byte[] data;
    /// <summary>
    /// Subcommand reply data.
    /// </summary>
    public IReadOnlyList<byte> Data => data;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private SubCmdReply()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"Subcommand for {SubcommandID}: ack={Acknowledged}";
}
