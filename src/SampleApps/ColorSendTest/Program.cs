using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LifxNetPlus;

namespace ColorSendTest {
	class Program {
		private static LifxClient _client;
		private static List<Device> _devicesBulb;
		private static List<Device> _devicesMulti;
		private static List<Device> _devicesMultiV2;
		private static List<Device> _devicesSwitch;
		private static List<Device> _devicesTile;
		private static List<int> stateList = new();
		private static List<StateExtendedColorZonesResponse> responses = new();

		private static Dictionary<string, LifxColor> _colors;
		private static int _colorIdx;

		static async Task Main() {

			_colors = new Dictionary<string, LifxColor>();
			_colors["red"] = new LifxColor(Color.Red);
			_colors["orange"] = new LifxColor(Color.Orange);
			_colors["yellow"] = new LifxColor(Color.Yellow);
			_colors["green"] = new LifxColor(Color.Green);
			_colors["blue"] = new LifxColor(Color.Blue);
			_colors["indigo"] = new LifxColor(Color.Indigo);
			_colors["violet"] = new LifxColor(Color.Violet);
				
			var tr1 = new TextWriterTraceListener(Console.Out);
			Trace.Listeners.Add(tr1);
			
			_devicesBulb = new List<Device>();
			_devicesMulti = new List<Device>();
			_devicesMultiV2 = new List<Device>();
			_devicesTile = new List<Device>();
			_devicesSwitch = new List<Device>();
			_client = LifxClient.CreateAsync().Result;
			_client.DeviceDiscovered += ClientDeviceDiscovered;
			_client.DeviceLost += ClientDeviceLost;
			Console.WriteLine("Enumerating devices, please wait 15 seconds...");
			_client.StartDeviceDiscovery();
			await Task.Delay(5000);
			_client.StopDeviceDiscovery();
			Console.WriteLine("Please select a device type to test (Enter a number):");
			if (_devicesBulb.Count > 0) {
				Console.WriteLine("1: Bulbs");
			}

			if (_devicesMulti.Count > 0) {
				Console.WriteLine("2: Multi Zone V1");
			}

			if (_devicesMultiV2.Count > 0) {
				Console.WriteLine("3: Multi Zone V2");
			}

			if (_devicesTile.Count > 0) {
				Console.WriteLine("4: Tiles");
			}

			if (_devicesSwitch.Count > 0) {
				Console.WriteLine("5: Switch");
			}

			var selection = int.Parse(Console.ReadLine() ?? "1");
			
			switch (selection) {
				case 1:
					Console.WriteLine("Flashing bulbs on and off.");
					await FlashBulbs();
					break;
				case 2:
					Console.WriteLine("Flashing multizone v1 devices on and off.");
					await FlashMultizoneV2();
					break;
				case 3:
					Console.WriteLine("Flashing multizone v2 devices on and off.");
					await FlashMultizoneV2();
					break;
				case 4:
					Console.WriteLine("Flashing tile devices on and off.");
					await FlashTiles();
					break;
				case 5:
					Console.WriteLine("Toggling switches is not enabled yet.");
					FlashSwitches();
					break;
			}

			Console.WriteLine("All done!");
			Console.ReadKey();
		}

		private static async Task FlashBulbs() {
			// Save our existing states
			var stateList = new List<LightStateResponse>();
			foreach (var b in _devicesBulb) {
				var bulb = b;
				var state = await _client.GetLightStateAsync(bulb);
				stateList.Add(state);
				_client.SetPowerAsync(b, 1).ConfigureAwait(false);
				_client.SetBrightnessAsync(bulb, 255).ConfigureAwait(false);
			}

			
			
			await Task.Delay(500);
			var idx = 0;
			Console.WriteLine("Restoring bulb states.");
			// foreach (var b in _devicesBulb) {
			// 	var bulb = (LightBulb) b;
			// 	var state = stateList[idx];
			// 	await _client.SetBrightnessAsync(bulb, state.Brightness);
			// 	await _client.SetPowerAsync(bulb, state.IsOn ? 1 : 0);
			// 	idx++;
			// }
		}

		private static async Task FlashMultizone() {
			var stateList = new List<int>();
			var responses = new List<StateMultiZoneResponse>();
			foreach (var m in _devicesMulti) {
				var state = await _client.GetPowerAsync(m);
				stateList.Add(state);
				var zoneState = await _client.GetColorZonesAsync(m, 0, 8);
				responses.Add(zoneState);
				_client.SetPowerAsync(m, 1).ConfigureAwait(false);
			}

			var idx = 0;
			foreach (var m in _devicesMulti) {
				var state = responses[idx];
				var count = state.Count;
				var start = state.Index;
				var total = start - count;
				for (var i = start; i < count; i++) {
					var pi = i * 1.0f;
					var progress = (start - pi) / total;
					var apply = i == count - 1;
					_client.SetColorZonesAsync(m, i, i, Rainbow(progress), TimeSpan.Zero, apply);
				}

				idx++;
			}

			await Task.Delay(2000);

			idx = 0;
			Debug.WriteLine("Setting v1 multi to rainbow!");
			var black = new LifxColor(0, 0, 0);
			foreach (var m in _devicesMulti) {
				var state = responses[idx];
				var count = state.Count;
				var start = state.Index;
				var total = start - count;
				for (var i = start; i < count; i++) {
					_client.SetColorZonesAsync(m, i, i, black, TimeSpan.Zero, true);
				}

				idx++;
			}

			idx = 0;
			Debug.WriteLine("Setting v1 multi to black/disabling.");
			foreach (var m in _devicesMulti) {
				var power = stateList[idx];
				if (power == 0) {
					_client.SetPowerAsync(m, power);
				}
			}
		}

		private static LifxColor CycleColor() {
			var color = _colors.ElementAt(_colorIdx).Value;
			_colorIdx++;
			if (_colorIdx >= _colors.Count) {
				_colorIdx = 0;
			}

			color.Kelvin = 3500;
			Console.WriteLine("Using color: " + color.ToHsbkString());
			
			return color;
		}

		private static async Task FlashMultizoneV2() {
			var hex =
				"bc02001411002553d073d542b31500004c4946585632061c0000000000000000fe0100002c010000010000475555ffffe8f5ac0df95379fee8f5ac0d9e52f4fce8f5ac0d43516ffbe8f5ac0de84feaf9e8f5ac0d8d4e65f8e8f5ac0d324de0f6e8f5ac0dd74b5bf5e8f5ac0d2a4b98f4e8f5ac0d7c4ad6f3e8f5ac0d214950f2e8f5ac0dc647cbf0e8f5ac0d6b4646efe8f5ac0d1045c1ede8f5ac0db5433cece8f5ac0d5a42b7eae8f5ac0dff4032e9e8f5ac0d52406fe8e8f5ac0da43fade7e8f5ac0d493e28e6e8f5ac0dee3ca2e4e8f5ac0d933b1de3e8f5ac0d383a98e1e8f5ac0ddd3813e0e8f5ac0d82378edee8f5ac0d273609dde8f5ac0d7a3546dce8f5ac0dcc3484dbe8f5ac0d7133ffd9e8f5ac0d16327ad8e8f5ac0dbb30f4d6e8f5ac0d602f6fd5e8f5ac0d052eead3e8f5ac0daa2c65d2e8f5ac0d4f2be0d0e8f5ac0da12a1dd0e8f5ac0df4295bcfe8f5ac0d4f2bcbce3af6ac0daa2c3cce8cf6ac0d052eadcddef6ac0d602f1dcd30f7ac0dbb308ecc82f7ac0d1632ffcbd4f7ac0d71336fcb26f8ac0dcc34e0ca78f8ac0d273651cacaf8ac0d8237c1c91cf9ac0ddd3832c96ef9ac0d383aa3c8c0f9ac0d933b13c812faac0dee3c84c764faac0d493ef4c6b6faac0df73eadc6dffaac0da43f65c608fbac0dff40d6c55afbac0d5a4246c5acfbac0db543b7c4fefbac0d104528c450fcac0d6b4698c3a2fcac0dc64709c3f4fcac0d21497ac246fdac0dcf4932c26ffdac0d7c4aeac198fdac0dd74b5bc1eafdac0d324dccc03cfeac0d8d4e3cc08efeac0de84fadbfe0feac0d43511dbf32ffac0d9e528ebe84ffac0df953ffbdd6ffac0da754b7bdfeffac0d00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
			 var bytes = Enumerable.Range(0, hex.Length)
			 	.Where(x => x % 2 == 0)
			 	.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
			 	.ToArray();
			 var ep = new IPEndPoint(IPAddress.Any, 56700);
			var pack = LifxClient.ParseMessage(bytes, ep, false);
			//Debug.WriteLine("PACK: " + JsonConvert.SerializeObject(pack));
			Debug.WriteLine("CAP BYTES: " + LifxClient.HexString(pack.Encode()));

			stateList = new List<int>();
			responses = new List<StateExtendedColorZonesResponse>();
			foreach (var m in _devicesMultiV2) {
				var state = await _client.GetPowerAsync(m);
				stateList.Add(state);
				var zoneState = await _client.GetExtendedColorZonesAsync(m);
				responses.Add(zoneState);
				_client.SetPowerAsync(m, 1);
				Debug.WriteLine("Setting devices to rainbow!");
				var idx = 0;
				var colors = new List<LifxColor>();
				for (var i = 0; i < 100; i++) {
					var color = Rainbow(i);
					for(var l = 0;
					l < zoneState.ZonesCount; l++) {
						colors.Add(color);
					}

					_client.SetExtendedColorZonesAsync(m, colors);
						await Task.Delay(1);
				}
			}

			
			

			
		}

		private static void SendBow(int transition) {
			var idx = 0;
			var loopColor = CycleColor();
			foreach (var m in _devicesMultiV2) {
				var state = responses[idx];
				var count = state.ZonesCount;
				var start = state.Index;
				var colors = new List<LifxColor>();
				for (var i = start; i < count; i++) {
					colors.Add(loopColor);
				}

				Console.WriteLine("We should be sending " + colors.Count + " colors.");
				_client.SetExtendedColorZonesAsync(m, colors, (uint) transition).ConfigureAwait(false);
				idx++;
				if (idx >= _devicesMultiV2.Count) idx = 0;
			}
		}

		private static async Task FlashTiles() {
			var chains = new List<StateDeviceChainResponse>();
			foreach (var t in _devicesTile) {
				var state = _client.GetDeviceChainAsync(t).Result;
				chains.Add(state);
				_client.SetPowerAsync(t, 1);
			}

			var idx = 0;
			Debug.WriteLine("Rainbowing tiles!");

			foreach (var t in _devicesTile) {
				var state = chains[idx];
				var colors = new LifxColor[64];
				for (var c = 0; c < 8; c++) {
					var progress = c / 8f;
					var col = Rainbow(progress);
					for (var m = 0; m < 8; m++) {
						colors[m * c] = col;
					}
				}

				for (var i = state.StartIndex; i < state.TotalCount; i++) {
					_client.SetTileState64Async(t, i, 1, 1000, colors.ToArray());
				}

				idx++;
			}

			await Task.Delay(2000);

			idx = 0;
			Debug.WriteLine("Turning off tiles.");
			foreach (var t in _devicesTile) {
				var state = chains[idx];
				var colors = new List<LifxColor>();
				for (var c = 0; c < 64; c++) {
					colors.Add(new LifxColor(0, 0, 0));
				}

				for (var i = state.StartIndex; i < state.TotalCount; i++) {
					_client.SetTileState64Async(t, i, 1, 1000, colors.ToArray());
				}

				_client.SetPowerAsync(t, 0);
				idx++;
			}
		}

		private static void FlashSwitches() {
		}

		private static LifxColor Rainbow(float progress) {
			progress *= .01f;
			Console.WriteLine("Progress is " + progress);
			var div = Math.Abs(progress % 1) * 6;
			var ascending = (int) (div % 1 * 255);
			var descending = 255 - ascending;
			var output = (int) div switch {
				0 => Color.FromArgb(255, ascending, 0),
				1 => Color.FromArgb(descending, 255, 0),
				2 => Color.FromArgb(0, 255, ascending),
				3 => Color.FromArgb(0, descending, 255),
				4 => Color.FromArgb(ascending, 0, 255),
				_ => Color.FromArgb(255, 0, descending)
			};
			return new LifxColor(output);
		}

		private static void ClientDeviceLost(object sender, LifxClient.DeviceDiscoveryEventArgs e) {
			Console.WriteLine("Device lost");
		}

		private static async void ClientDeviceDiscovered(object sender, LifxClient.DeviceDiscoveryEventArgs e) {
			Console.WriteLine($"Fetching version for {e.Device.HostName}");
			var version = await _client.GetDeviceVersionAsync(e.Device);
			var added = false;
			Console.WriteLine($"Version {version.Product} is {version.Version}");
			// Multi-zone devices
			if (version.Product == 31 || version.Product == 32 || version.Product == 38) {
				var extended = false;
				// If new Z-LED or Beam, check if FW supports "extended" commands.
				if (version.Product == 32 || version.Product == 38) {
					if (version.Version >= 277) {
						extended = true;
					}
				}

				if (extended) {
					added = true;
					Console.WriteLine("Adding V2 Multi zone Device.");
					_devicesMultiV2.Add(e.Device);
				} else {
					added = true;
					Console.WriteLine("Adding V1 Multi zone Device.");
					_devicesMulti.Add(e.Device);
				}
			}

			// Tile
			if (version.Product == 55) {
				added = true;
				Console.WriteLine("Adding Tile Device");
				_devicesTile.Add(e.Device);
			}

			// Switch
			if (version.Product == 70) {
				added = true;
				Console.WriteLine("Adding Switch Device.");
				_devicesSwitch.Add(e.Device);
			}

			if (!added) {
				Console.WriteLine("Adding Bulb.");
				_devicesBulb.Add(e.Device);
			}
		}
	}
}