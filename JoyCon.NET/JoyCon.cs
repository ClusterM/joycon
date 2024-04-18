using HidSharp;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wtf.cluster.joycon.JoyConReports;
using wtf.cluster.joycon.JoyConReports.Calibration;

namespace wtf.cluster.joycon;

/// <summary>
/// Joy-Con controller to control Joy-Con or Pro Controller.
/// </summary>
public class JoyCon
{
    private const int maxAckWaitCommands = 16;

    private readonly SemaphoreSlim semaphore = new(1, 1);
    private byte packetNum = 0;

    /// <summary>
    /// Subcommands.
    /// </summary>
    public enum Subcommand : byte
    {
        /// <summary>
        /// Get Only Controller State. Does nothing actually, but can be used to get Controller state only (w/o 6-Axis sensor data), like any subcommand that does nothing.
        /// </summary>
        Nothing = 0x00,
        /// <summary>
        /// Bluetooth manual pairing.
        /// </summary>
        BluetoothManualPairing = 0x01,
        /// <summary>
        /// Request device info.
        /// </summary>
        RequestDeviceInfo = 0x02,
        /// <summary>
        /// Set input report mode.
        /// </summary>
        SetInputReportMode = 0x03,
        /// <summary>
        /// Trigger buttons elapsed time. Replies with 7 little-endian uint16. The values are in 10ms. They reset by turning off the controller.
        /// </summary>
        TriggerButtonsElapseTime = 0x04,
        /// <summary>
        ///  Get page list state. Replies a uint8 with a value of x01 if there's a Host list with BD addresses/link keys in memory.
        /// </summary>
        GetPageListState = 0x05,
        /// <summary>
        /// Set HCI state (disconnect/page/pair/turn off). Causes the controller to change power state.
        /// </summary>
        SetHciState = 0x06,
        /// <summary>
        /// Reset pairing info. Initializes the 0x2000 SPI section.
        /// </summary>
        ResetPairingInfo = 0x07,
        /// <summary>
        /// Set shipment low power state. If set, the feature Triggered Broadcom Fast Connect scans when in suspened or disconnected state is disabled. Additionally, it sets the low power mode, when disconnected, to HID OFF.
        /// </summary>
        SetShipmentLowPowerState = 0x08,
        /// <summary>
        /// SPI flash read.
        /// </summary>
        SpiFlashRead = 0x10,
        /// <summary>
        /// SPI flash Write.
        /// </summary>
        SpiFlashWrite = 0x11,
        /// <summary>
        /// SPI sector erase.
        /// </summary>
        SpiFlashSectorErase = 0x12,
        /// <summary>
        /// Reset NFC/IR MCU.
        /// </summary>
        ResetNfcIrMcu = 0x20,
        /// <summary>
        /// Set NFC/IR MCU configuration. Write configuration data to MCU. This data can be IR configuration, NFC configuration or data for the 512KB MCU firmware update.
        /// </summary>
        SetNfcIrMcuConfig = 0x21,
        /// <summary>
        /// Set NFC/IR MCU state. 
        /// </summary>
        SetNfcIrMcuState = 0x22,
        /// <summary>
        /// Set unknown data (fw 3.86 and up). Sets a byte to x01 (enable something?) and sets also an unknown data (configuration? for NFC/IR MCU?) to the bt device struct that copies it from given argument.
        /// </summary>
        SetUnknownX24 = 0x24,
        /// <summary>
        /// Reset <see cref="SetUnknownX24"/> unknown data (fw 3.86 and up).
        /// </summary>
        ResetUnknownX24 = 0x25,
        /// <summary>
        /// Set unknown NFC/IR MCU data.
        /// </summary>
        SetUnknownNfcIrMcuData = 0x28,
        /// <summary>
        /// Get <see cref="SetUnknownNfcIrMcuData"/> NFC/IR MCU data.
        /// </summary>
        GetNfcIrMcuData = 0x29,
        /// <summary>
        /// Set GPIO Pin Output value (2 pin @ port 2).
        /// </summary>
        SetGpioOutputPort2 = 0x2A,
        /// <summary>
        /// Get <see cref="GetNfcIrMcuData"/> NFC/IR MCU data.
        /// </summary>
        GetNfcIrMcuData2 = 0x2B,
        /// <summary>
        /// Set player lights.
        /// </summary>
        SetPlayerLights = 0x30,
        /// <summary>
        /// Get player lights.
        /// </summary>
        GetPlayerLights = 0x31,
        /// <summary>
        /// Set HOME Light.
        /// </summary>
        SetHomeLight = 0x38,
        /// <summary>
        /// Enable IMU (6-Axis sensor).
        /// </summary>
        EnableImu = 0x40,
        /// <summary>
        /// Set IMU sensitivity.
        /// </summary>
        SetImuSensitivity = 0x41,
        /// <summary>
        /// Write to IMU registers. Consult LSM6DS3.pdf for all registers and their meaning. The registers addresses are mapped 1:1 in the subcmd. With this subcmd you can completely control the IMU.
        /// </summary>
        WriteImuRegister = 0x42,
        /// <summary>
        /// Read IMU registers.
        /// </summary>
        ReadImuRegister = 0x43,
        /// <summary>
        /// Enable vibration.
        /// </summary>
        EnableVibration = 0x48,
        /// <summary>
        /// Get regulated voltage. Internally, the values come from 1000mV - 1800mV regulated voltage samples, that are translated to 1320-1680 values. These follow a curve between 3.3V and 4.2V. So a 2.5x multiplier can get us the real battery voltage in mV.
        /// </summary>
        GetRegulatedVoltage = 0x50,
        /// <summary>
        /// Set GPIO Pin Output value (7, 15 pins @ port 1).
        /// </summary>
        SetGpioOutputPort1 = 0x51,
        /// <summary>
        /// Get GPIO Pin Input/Output value.
        /// </summary>
        GetGpioInput = 0x52
    }

    /// <summary>
    /// Input report mode, e.g. format of the data sent by the controller.
    /// </summary>
    public enum InputReportMode : byte
    {
        /// <summary>
        /// Used with cmd <see cref="WriteSpiFlashAsync"/> (?). Active polling for NFC/IR camera data. <see cref="InputReportMode.NfcIrMode"/> data format must be set first.
        /// </summary>
        NfcIr0 = 0x00,
        /// <summary>
        /// Same as <see cref="NfcIr0"/>. Active polling mode for NFC/IR MCU configuration data.
        /// </summary>
        NfcIr1 = 0x01,
        /// <summary>
        /// Same as <see cref="NfcIr0"/>. Active polling mode for NFC/IR data and configuration. For specific NFC/IR modes.
        /// </summary>
        NfcIr2 = 0x02,
        /// <summary>
        /// Same as <see cref="NfcIr0"/>. Active polling mode for IR camera data. For specific IR modes.
        /// </summary>
        NfcIr3 = 0x03,
        /// <summary>
        /// MCU update state report?
        /// </summary>
        McuUpdateStateReport = 0x23,
        /// <summary>
        /// Standard full mode. Pushes current state @60Hz.
        /// </summary>
        Full = 0x30,
        /// <summary>
        /// NFC/IR mode. Pushes large packets @60Hz
        /// </summary>
        NfcIrMode = 0x31,
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown1 = 0x32,
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown2 = 0x33,
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown3 = 0x35,
        /// <summary>
        /// Simple mode. Pushes updates with every data change.
        /// </summary>
        Simple = 0x3F
    }

    /// <summary>
    /// Gyroscope sensitivity.
    /// </summary>
    public enum GyroSensitivity : byte
    {
        /// <summary>
        /// ±250dps
        /// </summary>
        DPS250 = 0x00,
        /// <summary>
        /// ±500dps
        /// </summary>
        DPS500 = 0x01,
        /// <summary>
        /// ±1000dps
        /// </summary>
        DPS1000 = 0x02,
        /// <summary>
        /// ±2000dps (default)
        /// </summary>
        DPS2000 = 0x03
    }

    /// <summary>
    /// Gyroscope performance rate.
    /// </summary>
    public enum GyroPerformance : byte
    {
        /// <summary>
        /// 833Hz (high performance)
        /// </summary>
        Hz833 = 0x00,
        /// <summary>
        /// 208Hz (default)
        /// </summary>
        Hz208 = 0x01
    }

    /// <summary>
    /// Accelerometer sensitivity.
    /// </summary>
    public enum AccSensitivity : byte
    {
        /// <summary>
        /// ±8G (default)
        /// </summary>
        G8 = 0x00,
        /// <summary>
        /// ±4G
        /// </summary>
        G4 = 0x01,
        /// <summary>
        /// ±2G
        /// </summary>
        G2 = 0x02,
        /// <summary>
        /// ±16G
        /// </summary>
        G16 = 0x03
    }

    /// <summary>
    /// Accelerometer Anti-aliasing filter bandwidth.
    /// </summary>
    public enum AccFilter : byte
    {
        /// <summary>
        /// 200Hz.
        /// </summary>
        Hz200 = 0,
        /// <summary>
        /// 100Hz (default).
        /// </summary>
        Hz100 = 1,
    }

    /// <summary>
    /// LED state.
    /// </summary>
    public enum LedState : byte
    {
        /// <summary>
        /// LED is off.
        /// </summary>
        Off = 0,
        /// <summary>
        /// LED is on.
        /// </summary>
        On = 0b0000_0001,
        /// <summary>
        /// LED is blinking.
        /// </summary>
        Blinking = 0b0001_0000
    }

    /// <summary>
    /// Get the HID stream.
    /// </summary>
    public HidStream HidStream { get; }

    /// <summary>
    /// Create a new Joy-Con controller to control Joy-Con or Pro Controller.
    /// </summary>
    /// <param name="hidStream">HID stream.</param>
    public JoyCon(HidStream hidStream) => HidStream = hidStream;

    private async Task<IJoyConReport> ReadAsyncNotThreadSafe(CancellationToken cancellationToken = default)
    {
        var buf = new byte[512];
        var len = await HidStream.ReadAsync(buf, 0, buf.Length);

        return len == 0
            ? throw new InvalidDataException("No data.")
            : buf[0] switch
            {
                0x21 => FromBytes<InputWithSubCmdReply>(buf),
                0x23 => FromBytes<InputWithNfcIrMcuFw>(buf),
                0x30 => FromBytes<InputWithImu>(buf),
                0x31 => FromBytes<InputWithImuExtra>(buf),
                /* unknown */
                0x32 or 0x33 => FromBytes<InputWithImu>(buf),
                0x3F => FromBytes<InputSimple>(buf),
                _ => throw new InvalidDataException($"Unknown report ID: 0x{buf[0]:X2}"),
            };
    }

    /// <summary>
    /// Read a report from the controller.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The report.</returns>
    public async Task<IJoyConReport> ReadAsync(CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await ReadAsyncNotThreadSafe(cancellationToken);
        }
        finally
        {
            _ = semaphore.Release();
        }
    }

    private async Task<SubCmdReply?> WriteSubcommandReportAsync(byte reportID, RumbleSet? rumble, Subcommand subcommandID, byte[]? subcommandData = null, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            var buf = new byte[0x40];
            buf[0] = reportID;
            buf[1] = (byte)(packetNum++ & 0x0F);
            var rumbleRaw = rumble != null ? rumble.ToBytes() : new RumbleSet().ToBytes();
            Array.Copy(rumbleRaw, 0, buf, 2, rumbleRaw.Length);
            ;
            buf[10] = (byte)subcommandID;
            if (subcommandData != null && subcommandData.Length > 0)
            {
                Array.Copy(subcommandData, 0, buf, 11, subcommandData.Length);
            }
            try
            {
                await HidStream.WriteAsync(buf, 0, buf.Length, cancellationToken);
            }
            catch (IOException e) when (e.InnerException is System.ComponentModel.Win32Exception win32Exception && win32Exception.NativeErrorCode == 87)
            {
                // It's ok
            }
            var waitCount = 0;
            while (!noWaitAck)
            {
                IJoyConReport input = await ReadAsyncNotThreadSafe(cancellationToken);
                if (input is InputWithSubCmdReply i && i.SubcommandReply.SubcommandID == subcommandID)
                {
                    return i.SubcommandReply;
                }
                if (waitCount++ > maxAckWaitCommands)
                {
                    throw new TimeoutException("Subcommand acknowledgement not received.");
                }
            }
            return null;
        }
        finally
        {
            _ = semaphore.Release();
        }
    }

    /// <summary>
    /// Write a subcommand to the controller.
    /// </summary>
    /// <param name="rumble">Optional rumble data if you want to keep the vibration on.</param>
    /// <param name="subcommandID">Subcommand ID.</param>
    /// <param name="subcommandData">Subcommand data.</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The subcommand reply as <see cref="SubCmdReply"/> object or null if noWaitAck is true.</returns>
    public async Task<SubCmdReply?> WriteSubcommandAsync(RumbleSet? rumble, Subcommand subcommandID, byte[]? subcommandData = null, bool noWaitAck = false, CancellationToken cancellationToken = default)
        => await WriteSubcommandReportAsync(0x01, rumble, subcommandID, subcommandData, noWaitAck, cancellationToken);

    /// <summary>
    /// Set input report mode, e.g. format of the data sent by the controller. <see cref="InputReportMode.Full"/> is recommended for most cases.
    /// </summary>
    /// <param name="inputReportMode">Input report mode as <see cref="InputReportMode"/>.</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task SetInputReportModeAsync(InputReportMode inputReportMode, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.SetInputReportMode, [(byte)inputReportMode], noWaitAck, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to set input report mode.");
        }
    }

    /// <summary>
    /// Enable or disable IMU sensor.
    /// </summary>
    /// <param name="enable">True to enable, false to disable.</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task EnableImuAsync(bool enable, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.EnableImu, [enable ? (byte)0x01 : (byte)0x00], noWaitAck, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to enable/disable IMU.");
        }
    }

    /// <summary>
    /// Enable or disable vibration.
    /// </summary>
    /// <param name="enable">True to enable, false to disable.</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task EnableRumble(bool enable, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.EnableVibration, [enable ? (byte)0x01 : (byte)0x00], noWaitAck, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to enable/disable vibration.");
        }
    }

    /// <summary>
    /// Write a rumble data to the controller.
    /// </summary>
    /// <param name="rumbleSet">Set of rumble data as <see cref="RumbleSet"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    public async Task WriteRuble(RumbleSet rumbleSet, CancellationToken cancellationToken = default)
        => await WriteSubcommandReportAsync(0x10, rumbleSet, Subcommand.Nothing, null, true, cancellationToken: cancellationToken);

    /// <summary>
    /// Write a rumble data to the controller.
    /// </summary>
    /// <param name="leftRumble">Left rumble data as <see cref="Rumble"/>.</param>
    /// <param name="rightRumble">Right rumble data as <see cref="Rumble"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    public async Task WriteRuble(Rumble leftRumble, Rumble rightRumble, CancellationToken cancellationToken = default)
        => await WriteRuble(new RumbleSet(leftRumble, rightRumble), cancellationToken);

    /// <summary>
    /// Write a rumble data to the controller.
    /// </summary>
    /// <param name="freqLeft">Left rumble frequency.</param>
    /// <param name="ampLeft">Left rumble amplitude.</param>
    /// <param name="freqRight">Right rumble frequency.</param>
    /// <param name="ampRight">Right rumble amplitude.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    public async Task WriteRuble(double freqLeft, double ampLeft, double freqRight, double ampRight, CancellationToken cancellationToken = default)
        => await WriteRuble(new RumbleSet(freqLeft, ampLeft, freqRight, ampRight), cancellationToken);

    /// <summary>
    /// Write a rumble data to the controller.
    /// </summary>
    /// <param name="freqBoth">Frequency for both left and right rumble.</param>
    /// <param name="ampBoth">Amplitude for both left and right rumble.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    public async Task WriteRuble(double freqBoth, double ampBoth, CancellationToken cancellationToken = default)
        => await WriteRuble(new RumbleSet(freqBoth, ampBoth), cancellationToken);

    /// <summary>
    /// Set IMU sensor sensitivity.
    /// </summary>
    /// <param name="gyroSensitivity">Gyroscope sensitivity.</param>
    /// <param name="accSensitivity">Accelerometer sensitivity.</param>
    /// <param name="gyroPerformance">Gyroscope performance rate.</param>
    /// <param name="accFilter">Accelerometer anti-aliasing filter bandwidth.</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task SetImuSensitivity(AccSensitivity accSensitivity, AccFilter accFilter, GyroSensitivity gyroSensitivity, GyroPerformance gyroPerformance, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.SetImuSensitivity, [(byte)gyroSensitivity, (byte)accSensitivity, (byte)gyroPerformance, (byte)accFilter], noWaitAck, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to set IMU sensitivity.");
        }
    }

    /// <summary>
    /// Set LEDs state on the controller.
    /// </summary>
    /// <param name="led1">LED 1 state.</param>
    /// <param name="led2">LED 2 state.</param>
    /// <param name="led3">LED 3 state.</param>
    /// <param name="led4">LED 4 state.</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task SetPlayerLeds(LedState led1, LedState led2, LedState led3, LedState led4, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        byte data = 0;
        LedState[] leds = [led1, led2, led3, led4];
        for (var i = 0; i < 4; i++)
        {
            data |= (byte)((byte)leds[i] << i);
        }

        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.SetPlayerLights, [data], noWaitAck, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to set player LEDs.");
        }
    }

    /// <summary>
    /// Get LEDs state on the controller.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>LEDs state as an array of <see cref="LedState"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task<LedState[]> GetPlayerLeds(CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.GetPlayerLights, null, false, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to get player LEDs.");
        }
        var data = reply!.Data[0];
        return Enumerable.Range(0, 4).Select(i => (LedState)((data >> i) & 0b0001_0001)).ToArray();
    }

    /// <summary>
    /// Get device info.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The device info as <see cref="DeviceInfo"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task<DeviceInfo> GetDeviceInfoAsync(CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.RequestDeviceInfo, null, false, cancellationToken: cancellationToken);
        return reply != null && !reply.Acknowledged
            ? throw new InvalidOperationException("Failed to get device info.")
            : FromBytes<DeviceInfo>(reply!.Data.ToArray());
    }

    /// <summary>
    /// Get battery voltage in mV.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The battery voltage in mV.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task<ushort> GetVoltageAsync(CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.GetRegulatedVoltage, null, false, cancellationToken: cancellationToken);
        return reply != null && !reply.Acknowledged
            ? throw new InvalidOperationException("Failed to get voltage.")
            : (ushort)(BitConverter.ToUInt16(reply!.Data.ToArray(), 0) * 2.5);
    }

    /// <summary>
    /// Get trigger button elapsed time.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The trigger button elapsed time as <see cref="TriggerButtonElapsedTime"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task<TriggerButtonElapsedTime> GetTriggerButtonElapsedTimeAsync(CancellationToken cancellationToken = default)
    {
        /* To be honest, I have no idea what this is. */
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.TriggerButtonsElapseTime, null, false, cancellationToken: cancellationToken);
        return reply != null && !reply.Acknowledged
            ? throw new InvalidOperationException("Failed to get trigger button elapsed time.")
            : FromBytes<TriggerButtonElapsedTime>(reply!.Data.ToArray());
    }

    /// <summary>
    /// Write to IMU register directly. Consult LSM6DS3 datasheet for all registers and their meaning.
    /// </summary>
    /// <param name="address">Address of the register.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task ImuRegisterWriteAsync(byte address, byte value, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.WriteImuRegister, [address, 0x01, value], noWaitAck, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to write IMU register.");
        }
    }

    /// <summary>
    /// Read IMU registers directly. Consult LSM6DS3 datasheet for all registers and their meaning.
    /// </summary>
    /// <param name="address">Address of the first register.</param>
    /// <param name="length">Number of registers to read (up to 32).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Register data.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when length is out of range.</exception>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task<byte[]> ImuRegisterReadAsync(byte address, byte length, CancellationToken cancellationToken = default)
    {
        if (length is < 1 or > 32)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be between 1 and 32.");
        }
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.ReadImuRegister, [address, length], false, cancellationToken: cancellationToken);
        return reply != null && !reply.Acknowledged
            ? throw new InvalidOperationException("Failed to read IMU register.")
            : reply!.Data.Skip(2).Take(reply.Data[1]).ToArray();
    }

    /// <summary>
    /// Read the internal SPI flash data.
    /// </summary>
    /// <param name="address">Address to read.</param>
    /// <param name="length">Number of bytes to read (up to 29).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Read data.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when length is out of range.</exception>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed.</exception>
    public async Task<byte[]> ReadSpiFlashAsync(int address, int length, CancellationToken cancellationToken = default)
    {
        if (length is < 1 or > 29)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be between 1 and 29.");
        }
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.SpiFlashRead, BitConverter.GetBytes(address).Concat([(byte)length]).ToArray(), false, cancellationToken: cancellationToken);
        return reply != null && !reply.Acknowledged
            ? throw new InvalidOperationException("Failed to read SPI flash.")
            : reply!.Data.Skip(5).Take(reply.Data[4]).ToArray();
    }

    /// <summary>
    /// Write data to the internal SPI flash.
    /// </summary>
    /// <param name="address">Address to write.</param>
    /// <param name="data">Data to write (up to 29 bytes).</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when data length is out of range.</exception>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed or SPI flash is write-protected.</exception>
    public async Task WriteSpiFlashAsync(int address, byte[] data, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        if (data.Length is < 1 or > 29)
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Data length must be between 1 and 29.");
        }
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.SpiFlashWrite, BitConverter.GetBytes(address).Concat([(byte)data.Length]).Concat(data).ToArray(), noWaitAck, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to write SPI flash.");
        }
        if (reply != null && reply.Data[0] != 0x00)
        {
            throw new InvalidOperationException("SPI flash is write-protected.");
        }
    }

    /// <summary>
    /// Erase the SPI flash sector. This will erase 4KB of data.
    /// </summary>
    /// <param name="address">Address of the sector.</param>
    /// <param name="noWaitAck">True to not wait for acknowledgement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="InvalidOperationException">Thrown when acknowledgement is failed or SPI flash is write-protected.</exception>
    public async Task SpiFlashSectorErase(int address, bool noWaitAck = false, CancellationToken cancellationToken = default)
    {
        SubCmdReply? reply = await WriteSubcommandAsync(null, Subcommand.SpiFlashSectorErase, BitConverter.GetBytes(address), noWaitAck, cancellationToken: cancellationToken);
        if (reply != null && !reply.Acknowledged)
        {
            throw new InvalidOperationException("Failed to erase SPI flash sector.");
        }
        if (reply != null && reply.Data[0] != 0x00)
        {
            throw new InvalidOperationException("SPI flash is write-protected.");
        }
    }

    /// <summary>
    /// Read the factory stick calibration data from the SPI flash memory.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The factory calibration data as a <see cref="Calibration"/> object.</returns>
    public async Task<Calibration> GetFactoryCalibrationAsync(CancellationToken cancellationToken = default)
    {
        var imuData = await ReadSpiFlashAsync(0x6020, 24, cancellationToken);
        var sticksData = await ReadSpiFlashAsync(0x603D, 9 * 2, cancellationToken);

        ImuCalibration imuCal = FromBytes<ImuCalibration>(imuData);
        var leftStickCal = StickCalibration.FromBytes(false, sticksData[0..9]);
        var rightStickCal = StickCalibration.FromBytes(true, sticksData[9..18]);

        return new Calibration
        {
            ImuCalibration = imuCal,
            LeftStickCalibration = leftStickCal,
            RightStickCalibration = rightStickCal
        };
    }

    /// <summary>
    /// Get the user calibration data from the SPI flash memory.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user calibration data as <see cref="Calibration"/> object.</returns>
    public async Task<Calibration> GetUserCalibrationAsync(CancellationToken cancellationToken = default)
    {
        var imuData = await ReadSpiFlashAsync(0x8026, 26, cancellationToken);
        var sticksData = await ReadSpiFlashAsync(0x8010, 2 + 9 + 2 + 9, cancellationToken);

        ImuCalibration? imuCal = (imuData[0], imuData[1]) == (0xB2, 0xA1) ? FromBytes<ImuCalibration>(imuData[2..26]) : null;
        StickCalibration? leftStickCal = (sticksData[0], sticksData[1]) == (0xB2, 0xA1) ? StickCalibration.FromBytes(false, sticksData[2..11]) : null;
        StickCalibration? rightStickCal = (sticksData[11], sticksData[12]) == (0xB2, 0xA1) ? StickCalibration.FromBytes(true, sticksData[13..22]) : null;

        return new Calibration
        {
            ImuCalibration = imuCal,
            LeftStickCalibration = leftStickCal,
            RightStickCalibration = rightStickCal
        };
    }

    /// <summary>
    /// Get the minor stick parameters: dead-zone and range ratio.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stick parameters as <see cref="StickParameters"/> object.</returns>
    public async Task<SticksParametersSet> GetSticksParametersAsync(CancellationToken cancellationToken = default)
    {
        var leftData = await ReadSpiFlashAsync(0x6086, 18, cancellationToken);
        var rightData = await ReadSpiFlashAsync(0x6098, 18, cancellationToken);
        return new SticksParametersSet(
            StickParameters.FromBytes(leftData),
            StickParameters.FromBytes(rightData)
        );
    }

    /// <summary>
    /// Get the controller's serial number. Can be null if not present.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The serial number or null if not present.</returns>
    public async Task<string?> GetSerialNumberAsync(CancellationToken cancellationToken = default)
    {
        var data = await ReadSpiFlashAsync(0x6000, 0x10, cancellationToken);
        if (data[0] >= 0x80)
        {
            return null;
        }
        var noZeros = data.Where(b => b != 0x00).ToArray();
        var serial = Encoding.ASCII.GetString(noZeros.Skip(1).ToArray());
        if (serial.Length >= 16)
        {
            serial = serial[0..15];
        }
        return serial;
    }

    /// <summary>
    /// Check if the controller is new. It's programmed to true on the factory. Switch makes sure to set it to false after the first connection. Can be inaccurate.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the controller is new, false otherwise.</returns>
    public async Task<bool> GetIsJoyConNewAsync(CancellationToken cancellationToken = default)
    {
        var data = await ReadSpiFlashAsync(0x5000, 0x01, cancellationToken);
        return data[0] == 1;
    }

    /// <summary>
    /// Get the controller's colors stored in the SPI flash memory. Can be null if not present.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The controller's colors as <see cref="ControllerColors"/> object or null if not present.</returns>
    public async Task<ControllerColors?> GetColorsAsync(CancellationToken cancellationToken = default)
    {
        var present = await ReadSpiFlashAsync(0x601B, 1, cancellationToken);
        if (present[0] != 1)
        {
            return null;
        }
        var data = await ReadSpiFlashAsync(0x6050, 12, cancellationToken);
        var body = Color.FromArgb(data[0], data[1], data[2]);
        var buttons = Color.FromArgb(data[3], data[4], data[5]);
        var leftGrip = Color.FromArgb(data[6], data[7], data[8]);
        var rightGrip = Color.FromArgb(data[9], data[10], data[11]);
        return new ControllerColors
        {
            BodyColor = body,
            ButtonsColor = buttons,
            LeftGripColor = leftGrip,
            RightGripColor = rightGrip
        };
    }

    /*
     * Can't get HID features to work for some reason. TODO.
    public async Task<SubCmdReply> GetLastSubcommandReply(CancellationToken cancellationToken = default)
    {
        var buf = new byte[64];
        buf[0] = 0x02;
        HidStream.SetFeature(buf);
        return FromBytes<SubCmdReply>(buf);
    }
    */

    /// <summary>
    /// Create object from raw data.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <returns>Object.</returns>
    internal static T FromBytes<T>(byte[] data)
    {
        var rawsize = Marshal.SizeOf(typeof(T));
        var tsize = Math.Min(rawsize, data.Length);
        IntPtr buffer = Marshal.AllocHGlobal(tsize);
        Marshal.Copy(data, 0, buffer, tsize);
        var retobj = (T)Marshal.PtrToStructure(buffer, typeof(T));
        Marshal.FreeHGlobal(buffer);
        return retobj;
    }

    internal static ushort[] UnpackUShort(byte[] data) => data.Length % 3 != 0
            ? throw new InvalidOperationException("Invalid data length.")
            : Enumerable.Range(0, data.Length * 2 / 3)
            .Select(i => (target: i, source: i * 3 / 2))
            .Select(a => (a.target & 1) switch
            {
                0 => (ushort)(data[a.source + 1] << 8 & 0xF00 | data[a.source]),
                _ => (ushort)(data[a.source + 1] << 4 | data[a.source] >> 4)
            }).ToArray();
}
