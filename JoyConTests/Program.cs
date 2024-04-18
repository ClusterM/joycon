﻿using HidSharp;
using wtf.cluster.joycon;
using wtf.cluster.joycon.JoyConReports;
using wtf.cluster.joycon.JoyConReports.Calibration;

namespace JoyConTests;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Get a list of all HID devices
        DeviceList list = DeviceList.Local;
        // Get all devices developed by Nintendo
        HidDevice[] nintendos = list.GetHidDevices(0x057e).ToArray();
        // Get the first Nintendo controller
        HidDevice? device = nintendos.FirstOrDefault();
        if (device == null)
        {
            Console.WriteLine("No controller. Please connect Joy-Con or Pro controller via Bluetooth.");
            return;
        }
        // Open the device
        using HidStream hidStream = device.Open();
        // Create a new JoyCon instance based on the HID stream
        var joycon = new JoyCon(hidStream);

        // First of all, we need to set format of the input reports,
        // most of the time you will need to use InputReportMode.Full mode
        await joycon.SetInputReportModeAsync(JoyCon.InputReportMode.Full);

        // Get some information about the controller
        DeviceInfo deviceInfo = await joycon.GetDeviceInfoAsync();
        Console.WriteLine($"Type: {deviceInfo.ControllerType}, Firmware: {deviceInfo.FirmwareVersionMajor}.{deviceInfo.FirmwareVersionMinor}, {deviceInfo}");
        var serial = await joycon.GetSerialNumberAsync();
        Console.WriteLine($"Serial number: {serial ?? "<none>"}");
        ControllerColors? colors = await joycon.GetColorsAsync();
        if (colors != null)
        {
            Console.WriteLine($"Body color: {colors.BodyColor}, buttons color: {colors.ButtonsColor}");
        }
        else
        {
            Console.WriteLine("Colors not specified, seems like controller is gray.");
        }

        // Enable IMU (accelerometer and gyroscope)
        await joycon.EnableImuAsync(true);
        // Enable rumble feature (it's enabled by default, actually)
        await joycon.EnableRumble(true);
        // You can control LEDs on the controller
        await joycon.SetPlayerLeds(JoyCon.LedState.Off, JoyCon.LedState.On, JoyCon.LedState.Off, JoyCon.LedState.Blinking);

        // Get factory calibration data
        Calibration facCal = await joycon.GetFactoryCalibrationAsync();
        // Get user calibration data
        Calibration userCal = await joycon.GetUserCalibrationAsync();
        // Combine both calibrations, e.g. user calibration has higher priority
        Calibration calibration = facCal + userCal;
        // Get some parameters for the sticks
        SticksParametersSet sticksParameters = await joycon.GetSticksParametersAsync();

        var i = 0;
        while (true)
        {
            try
            {
                // Read input report from the controller
                IJoyConReport input = await joycon.ReadAsync();
                // Check the type of the input report, most likely it will be InputWithImu
                if (input is InputWithImu j)
                {
                    // Some base data from the controller
                    Console.WriteLine($"LeftStick: ({j.LeftStick}), RightStick: ({j.RightStick}), Buttons: ({j.Buttons}), Battery: {j.Battery}, Charging: {j.Charging}, Connection: {j.Connection}");
                    // But we need calibrated data
                    StickPositionCalibrated leftStickCalibrated = j.LeftStick.GetCalibrated(calibration.LeftStickCalibration!, sticksParameters.LeftStickParameters.DeadZone);
                    StickPositionCalibrated rightStickCalibrated = j.RightStick.GetCalibrated(calibration.RightStickCalibration!, sticksParameters.RightStickParameters.DeadZone);
                    Console.WriteLine($"Calibrated left stick: ({leftStickCalibrated}), right stick: ({rightStickCalibrated}");
                    // And accelerometer and gyroscope data
                    ImuFrameCalibrated calibratedImu = j.Imu.Frames[0].GetCalibrated(calibration.ImuCalibration!);
                    Console.WriteLine($"Calibrated IMU: {calibratedImu}");
                }
                else
                {
                    Console.WriteLine($"Invalid input report type: {input.GetType()}");
                }
                if (i++ % 30 == 0)
                {
                    // Periodically send rumble command to the controller
                    await joycon.WriteRuble(new RumbleSet(new Rumble(40.9, 1), new Rumble(65, 1)));
                    Console.WriteLine("Rumble");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                await Task.Delay(1000);
            }
        }
    }
}