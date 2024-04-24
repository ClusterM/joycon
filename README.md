# JoyCon.NET

You can use this .NET library to communicate with Nintendo Switch Joy-Con or Pro controllers. It's based on the [HidSharp](https://www.zer7.com/software/hidsharp) library, which is a cross-platform .NET library for USB HID devices, and it's compatible with Windows, Linux and macOS. So, this library should work on all these platforms too.

## What you can do with this library

* Read input data: buttons, sticks, gyroscope and accelerometer.
* Read factory and user calibration data and use it to get calibrated values.
* Control HD Rumble (frequency and amplitude).
* Set input report mode.
* Set player LEDs.
* Set Home LED pattern.
* Set IMU sensor sensitivity, filter and perfomance parameters.
* Get battery level.
* Get base device info (type, firmware version, etc).
* Get the controller's colors.
* Read/write SPI flash memory.
* Read/write IMU sensor registers directly.

And what you can't do (yet?):
* MCU stuff: NFC, IR camera, Ringfit, etc.

## How to use

It's pretty simple. First, you need to connect your controller via Bluetooth to the computer. Then you need get the controller's HidDevice object using the [HidSharp](https://www.zer7.com/software/hidsharp) library, create a `JoyCon` object, subscribe to the events and call the `Start()` method. Here's an example:

```csharp
using HidSharp;
using System.Text;
using wtf.cluster.JoyCon;
using wtf.cluster.JoyCon.Calibration;
using wtf.cluster.JoyCon.ExtraData;
using wtf.cluster.JoyCon.HomeLed;
using wtf.cluster.JoyCon.InputReports;
using wtf.cluster.JoyCon.Rumble;

Console.OutputEncoding = Encoding.UTF8;

// Get a list of all HID devices using the HidSharp library
DeviceList list = DeviceList.Local;
// Get all devices developed by Nintendo
var nintendos = list.GetHidDevices(0x057e);
// Get the first Nintendo controller
HidDevice? device = nintendos.FirstOrDefault();
if (device == null)
{
    Console.WriteLine("No controller. Please connect Joy-Con or Pro controller via Bluetooth.");
    return;
}
// Create a new JoyCon instance based on the HID device
var joycon = new JoyCon(device);
// Start controller polling
joycon.Start();

// First of all, we need to set format of the input reports,
// most of the time you will need to use InputReportMode.Full mode
await joycon.SetInputReportModeAsync(JoyCon.InputReportType.Full);

// Get some information about the controller
DeviceInfo deviceInfo = await joycon.GetDeviceInfoAsync();
Console.WriteLine($"Type: {deviceInfo.ControllerType}, Firmware: {deviceInfo.FirmwareVersionMajor}.{deviceInfo.FirmwareVersionMinor}");
var serial = await joycon.GetSerialNumberAsync();
Console.WriteLine($"Serial number: {serial ?? "<none>"}");
ControllerColors? colors = await joycon.GetColorsAsync();
if (colors != null)
{
    Console.WriteLine($"Body color: {colors.BodyColor}, buttons color: {colors.ButtonsColor}");
}
else
{
    Console.WriteLine("Colors not specified, seems like the controller is grey.");
}

// Enable IMU (accelerometer and gyroscope)
await joycon.EnableImuAsync(true);
// Enable rumble feature (it's enabled by default, actually)
await joycon.EnableRumbleAsync(true);
// You can control LEDs on the controller
await joycon.SetPlayerLedsAsync(JoyCon.LedState.Off, JoyCon.LedState.On, JoyCon.LedState.Off, JoyCon.LedState.Blinking);

// Get factory calibration data
CalibrationData facCal = await joycon.GetFactoryCalibrationAsync();
// Get user calibration data
CalibrationData userCal = await joycon.GetUserCalibrationAsync();
// Combine both calibrations, e.g. user calibration has higher priority
CalibrationData calibration = facCal + userCal;
// Get some parameters for the sticks
StickParametersSet sticksParameters = await joycon.GetStickParametersAsync();

// Home LED dimming pattern demo. Useless, but fun.
await joycon.SetHomeLedDimmingPatternAsync(new HomeLedDimmingPattern
{
    StepDurationBase = 4,    // base duration is 40ms
    StartLedBrightness = 0,  // 0%, off
    FullCyclesNumber = 0,    // infinite
    HomeLedDimmingSteps =
    {
        new HomeLedDimmingStep
        {
            LedBrightness = 0x0F,    // 100%
            TransitionDuration = 2,  // base * 2 = 40ms * 2 = 80ms
            PauseDuration = 4,       // base * 4 = 40ms * 4 = 160ms
        },
        new HomeLedDimmingStep
        {
            LedBrightness = 0x00,    // LED off
            TransitionDuration = 2,  // base * 2 = 40ms * 2 = 80ms
            PauseDuration = 4,       // base * 4 = 40ms * 4 = 160ms
        },
        new HomeLedDimmingStep { LedBrightness = 0x0F, TransitionDuration = 2, PauseDuration = 4 },
        new HomeLedDimmingStep { LedBrightness = 0x00, TransitionDuration = 2, PauseDuration = 4 },
        new HomeLedDimmingStep { LedBrightness = 0x0F, TransitionDuration = 2, PauseDuration = 4 },
        new HomeLedDimmingStep { LedBrightness = 0x00, TransitionDuration = 2, PauseDuration = 4 },
        new HomeLedDimmingStep
        {
            LedBrightness = 0x08,    // 50%
            TransitionDuration = 8,  // base * 8 = 40ms * 8 = 320ms
            PauseDuration = 4,       // base * 8 = 40ms * 8 = 320ms
        },
        new HomeLedDimmingStep
        {
            LedBrightness = 0x00,    // LED off
            TransitionDuration = 8,  // base * 8 = 40ms * 8 = 320ms
            PauseDuration = 15,      // base * 15 = 40ms * 15 = 600ms
        }
    },
});

// Save the current cursor position
(var cX, var cY) = (Console.CursorLeft, Console.CursorTop);

// Subscribe to the input reports
joycon.ReportReceived += (sender, input) =>
{
    Console.SetCursorPosition(cX, cY);
    // Check the type of the input report, most likely it will be InputWithImu
    if (input is InputFullWithImu j)
    {
        // Some base data from the controller
        Console.WriteLine($"LeftStick: ({j.LeftStick}), RightStick: ({j.RightStick}), Buttons: ({j.Buttons}), Battery: {j.Battery}, Charging: {j.Charging}          ");
        // But we need calibrated data
        StickPositionCalibrated leftStickCalibrated = j.LeftStick.GetCalibrated(calibration.LeftStickCalibration!, sticksParameters.LeftStickParameters.DeadZone);
        StickPositionCalibrated rightStickCalibrated = j.RightStick.GetCalibrated(calibration.RightStickCalibration!, sticksParameters.RightStickParameters.DeadZone);
        Console.WriteLine($"Calibrated LeftStick: (({leftStickCalibrated}), RightStick: ({rightStickCalibrated}))          ");
        // And accelerometer and gyroscope data
        ImuFrameCalibrated calibratedImu = j.Imu.Frames[0].GetCalibrated(calibration.ImuCalibration!);
        Console.WriteLine($"Calibrated IMU: ({calibratedImu})          ");
    }
    else
    {
        Console.WriteLine($"Invalid input report type: {input.GetType()}");
    }
    return Task.CompletedTask;
};

// Error handling
joycon.StoppedOnError += new JoyCon.StoppedOnErrorHandler((_, ex) =>
{
    Console.WriteLine();
    Console.WriteLine($"Critical error: {ex.Message}");
    Console.WriteLine("Controller polling stopped.");
    Environment.Exit(1);
    return Task.CompletedTask;
});

// Periodically send rumble command to the controller
var rumbleTimer = new Timer(async _ =>
{
    try
    {
        await joycon.WriteRumble(new RumbleSet(new RumbleData(40.9, 1), new RumbleData(65, 1)));
    }
    catch (Exception e)
    {
        Console.WriteLine($"Rumble error: {e.Message}");
    }
}, null, 0, 1000);

Console.ReadKey();
await rumbleTimer.DisposeAsync();
joycon.Stop();

Console.WriteLine();
Console.WriteLine("Stopped.");
```

You should call the `SetInputReportModeAsync()` method to set the input report format. Most of the time you'll need to use:
* The `InputReportType.Full` mode, in this mode you'll receive `InputFull` objects, which contains all the data: buttons, sticks, gyroscope, accelerometer, battery level, etc. You'll receive reports at a rate of 60 Hz. This is the most common mode.
* The `InputReportType.Simple` mode, in this mode you'll receive `InputSimple` objects, which contains the base data only. You'll receive reports with every data change.

`ReportReceived` event is fired when a new input report is received from the controller. It will be the object that implements the `IJoyConReport` interface. Possible objects are:
* `InputSimple`, used in the `InputReportType.Simple` mode, contains the base data only.
* `InputFull`, used in the `InputReportType.Full` mode, contains all the data: buttons and sticks.
* `InputFullWithImu` - derived from `InputFull`, contains the base data and IMU data.
* `InputFullWithSubCmdReply` - derived from `InputFull`, contains the base data and subcommand reply data. You should handle subcommand replies only if you send subcommands with `noWaitAck = true` parameter. Otherwise, the library will handle it for you.
* `InputFullWithImuMcuFw` - derived from `InputFull`, contains the base data, IMU data and some additional MCU data. This feature in not implemented in this library.
* `InputFullWithMcuFw` - derived from `InputFull`, contains the base data and MCU firmware update data. This feature in not implemented in this library.

Most of the time you'll need just `InputReportType.Full` mode and handle only `InputFullWithImu` or `InputFull` objects. You should handle the `ReportReceived` event as fast as possible, because the next report will not be received until the current one is processed. If you need to do some heavy calculations, you should use a separate thread or a timer.

`StoppedOnError` event is fired when an critical error occurs during the controller polling than causes the polling to stop, i.e. the controller is disconnected. You should handle this event to stop the application properly or to try to reconnect the controller.

You can find full documentation here: https://clusterm.github.io/joycon/

## Some notes
* Both Joy-Con controllers have both sticks and HD Rumble actuators registers. So, input reports contain data for both sticks and actuators. But you can't use the right stick data for the left controller and vice versa. Missing stick's data is invalid. 
  The same is for the HD Rumble: you can write rumble data to the both actuators but right Joy-Con will handle only the right actuator data and the left Joy-Con will handle only the left actuator data. Pro controller has all the sticks and actuators, so you can read and write data for all of them.
* You'll receive input reports with a raw uncalibrated data. You can get the calibration data using the `GetFactoryCalibrationAsync()`, `GetUserCalibrationAsync()` and `GetStickParametersAsync()` methods, then use `IStickPosition.GetCalibrated()` and `ImuFrame.GetCalibrated()` methods to get calibrated data. User calibration can be set in the Switch's system settings.
* Don't forget to call the `EnableImuAsync()` method if you need to use the accelerometer and gyroscope data, it's disabled by default.
* You can also call the `EnableRumbleAsync()` method but rumble is enabled by default.
* Rumble amlitude is limited by this library to avoid damaging the controller.

## Download on NuGet
`dotnet add package JoyCon.NET`

https://www.nuget.org/packages/JoyCon.NET

## Credits
* [HidSharp](https://www.zer7.com/software/hidsharp/) library by Zer.
* [Nintendo Switch Reverse Engineering](https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/) by dekuNukem.
* Me (Alexey Cluster)

## Donate
* [Buy Me A Coffee](https://www.buymeacoffee.com/cluster)
* [Become a sponsor on GitHub](https://github.com/sponsors/ClusterM)
* [Donation Alerts](https://www.donationalerts.com/r/clustermeerkat)
* [Boosty](https://boosty.to/cluster)
* BTC: 1MBYsGczwCypXhMBocoDQWxx7KZT2iiwzJ
* PayPal is not available in Armenia :(
