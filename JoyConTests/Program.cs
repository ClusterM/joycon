using HidSharp;
using System.Runtime.InteropServices;
using System.Text;
using wtf.cluster.JoyCon;
using wtf.cluster.JoyCon.Calibration;
using wtf.cluster.JoyCon.ExtraData;
using wtf.cluster.JoyCon.HomeLed;
using wtf.cluster.JoyCon.InputReports;
using wtf.cluster.JoyCon.Rumble;

Console.OutputEncoding = Encoding.UTF8;

HidDevice? device = null;
// Get a list of all HID devices
DeviceList list = DeviceList.Local;
if (OperatingSystem.IsWindows())
{
    // Get all devices developed by Nintendo by vendor ID
    var nintendos = list.GetHidDevices(0x057e);
    // Get the first Nintendo controller
    device = nintendos.FirstOrDefault();
    // It works fine for Windows, but...
}
else
{
    // Linux has more complicated HID device management, we can't get device's vendor ID,
    // so let's filter devices by their report descriptor
    // It should work in Windows too, so it's more multiplatform solution
    var hidDevices = list.GetHidDevices();
    foreach (var d in hidDevices)
    {
        var rd = d.GetReportDescriptor();
        if (rd != null)
        {
            if (
                (rd.OutputReports.Count() == 4)
                && (rd.OutputReports.Where(r => r.ReportID == 0x01).Count() == 1)
                && (rd.OutputReports.Where(r => r.ReportID == 0x10).Count() == 1)
                && (rd.OutputReports.Where(r => r.ReportID == 0x11).Count() == 1)
                && (rd.OutputReports.Where(r => r.ReportID == 0x12).Count() == 1)
                && (rd.InputReports.Count() == 6)
                && (rd.InputReports.Where(r => r.ReportID == 0x21).Count() == 1)
                && (rd.InputReports.Where(r => r.ReportID == 0x30).Count() == 1)
                && (rd.InputReports.Where(r => r.ReportID == 0x31).Count() == 1)
                && (rd.InputReports.Where(r => r.ReportID == 0x32).Count() == 1)
                && (rd.InputReports.Where(r => r.ReportID == 0x33).Count() == 1)
                && (rd.InputReports.Where(r => r.ReportID == 0x3F).Count() == 1)
            )
            {
                device = d;
                break;
            }
        }
    }
}
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
