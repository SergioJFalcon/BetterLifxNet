using LifxNetPlus;
using System;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static LifxNetPlus.LifxClient client;

    private static async void Client_DeviceDiscovered(object? sender, LifxClient.DeviceDiscoveryEventArgs e)
    {
        Console.WriteLine($"Device {e.Device.MacAddressName} found @ {e.Device.HostName}");
        var version = await client.GetDeviceVersionAsync(e.Device);
        Console.WriteLine($"Version: {version}");
        //var label = await client.GetDeviceLabelAsync(e.Device);
        var my_lightbulb = e.Device as Device;
        if (my_lightbulb == null)
        {
            Console.WriteLine("Device is not a lightbulb");
            return;
        }
        var dev_label = await client.GetDeviceLabelAsync(my_lightbulb);
        Console.WriteLine($"Label: {dev_label}");

        var state = await client.GetLightStateAsync(my_lightbulb);
        if (state == null)
        {
            Console.WriteLine("State is null");
            return;
        }
        Console.WriteLine($"State: {state}");
        Console.WriteLine($"{state.Label}\n\tIs on: {state.IsOn}\n\tHue: {state.Hue}\n\tSaturation: {state.Saturation}\n\tBrightness: {state.Brightness}\n\tTemperature: {state.Kelvin}");

        var green_color = new LifxColor(Color.Green);
        var blue_color = new LifxColor(
          color: Color.Blue,
          brightness: 0.1
        );
        // var orange_color = new LifxColor(Color.Orange);
        var yellow_color = new LifxColor(Color.Yellow, 0.1);

        Console.WriteLine("Setting color to blue");

        // Set the color to blue
        await client.SetColorAsync(my_lightbulb, blue_color);
        Thread.Sleep(3000);
        
        var my_wave_type = LifxNetPlus.LifxClient.WaveFormType.Sine;
        Console.WriteLine($"Setting Waveform to: {my_wave_type}");

        await client.SetWaveForm(
          my_lightbulb, // device
          true,         // transient flag
          yellow_color,     // your color
          1000,          // period in ms
          10000F,          // cycles
          0,// 16383,            // skewRatio
          LifxNetPlus.LifxClient.WaveFormType.Pulse // waveform type
        );

				// await client.SetWarning(my_lightbulb);

				Thread.Sleep(3000);

        // Wait 5 seconds
        // Thread.Sleep(5000);
        // Console.WriteLine("Attempting to override Waveform command with resetting it to original color");
        
        // var red_color = new LifxColor(
        //   color: Color.Red,
        //   brightness: 0.1
        // );
        // await client.SetColorAsync(my_lightbulb, red_color);


        // await client.SetColorAsync(my_lightbulb, green_color, 9000);
        await client.GetLightPowerAsync(my_lightbulb);


        // Create infinte loop
        // while (true)
        // {
        //     for (int i = 1; i <= 65535; i++)
        //     {
        //         Console.WriteLine($"idx: {i}");
        //         await client.SetColorAsync(
        //           bulb: my_lightbulb, 
        //           hue: (ushort)i,
        //           saturation: 65535,
        //           brightness: 20000,
        //           kelvin: 4500,
        //           transitionDuration: TimeSpan.FromMilliseconds(1000)
        //         );
        //     }
        // }

        // var new_state = await client.GetLightStateAsync(my_lightbulb);
        
        // // await client.SetColorAsync(my_lightbulb, black_color, 3500);
        // Console.WriteLine($"{new_state.Label}\n\tIs on: {new_state.IsOn}\n\tHue: {new_state.Hue}\n\tSaturation: {new_state.Saturation}\n\tBrightness: {new_state.Brightness}\n\tTemperature: {new_state.Kelvin}");

    }

    private static void Client_DeviceLost(object? sender, LifxClient.DeviceDiscoveryEventArgs e)
    {
        Console.WriteLine("Device lost");
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting discovery");
        var task = LifxNetPlus.LifxClient.CreateAsync();
        task.Wait();

        client = task.Result;
        client.DeviceDiscovered += Client_DeviceDiscovered;
        client.DeviceLost += Client_DeviceLost;
        client.StartDeviceDiscovery();

        Console.ReadKey();
        Console.WriteLine("Stopping discovery");
    }
}
