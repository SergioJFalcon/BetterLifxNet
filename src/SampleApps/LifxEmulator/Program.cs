using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using LifxNetPlus;
using Newtonsoft.Json;
using Console = Colorful.Console;

namespace LifxEmulator {
	/// <summary>
	/// The program class
	/// </summary>
	internal static class Program {
		/// <summary>
		/// The device version
		/// </summary>
		private static int _deviceVersion;
		/// <summary>
		/// The quit flag
		/// </summary>
		private static bool _quitFlag;
		/// <summary>
		/// The data
		/// </summary>
		private static DeviceData _data;
		/// <summary>
		/// The product info
		/// </summary>
		private static Product _productInfo;
		/// <summary>
		/// The messages
		/// </summary>
		private static List<string> _messages;
		/// <summary>
		/// The client
		/// </summary>
		private static UdpClient _client;
		/// <summary>
		/// Main
		/// </summary>
		public static void Main() {
			_messages = new List<string>();
			
			var tr1 = new TextWriterTraceListener(Console.Out);
			Trace.Listeners.Add(tr1);
			Console.CancelKeyPress += HandleClose;
			var end = new IPEndPoint(IPAddress.Any, 56700);
			var client = new UdpClient(end) {Client = {Blocking = false}};
			client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			client.Client.SendBufferSize = 4096;
			client.Client.ReceiveBufferSize = 4096;
			_client = client;
			
			var hex =
				"bc0200141100d66dd073d542b31500004c4946585632067a0000000000000000fe0100002c010000010000475a90a2f00a80ac0d21821dedde78ac0d057b5beb4975ac0de87398e9b371ac0dcc6cd6e71e6eac0daf6513e6886aac0d935e51e4f266ac0d77578ee25d63ac0d3e4909df325cac0d214246dd9c58ac0d053b84db0655ac0de933c1d97151ac0dcc2cffd7db4dac0db0253cd6454aac0d931e7ad4b046ac0dc11aeab9b0481d10d818a3acb0495511ee165b9fb04a8e1205151392b04bc6131c13cc84b04cff1433118477b04d3716490f3d6ab04e7017770bad4fb150e1198e096642b151191ba4071e35b152521cbb05d627b1538a1dd2038f1ab154c31ee901470db155fb1f00000000b15634216bf5ad1f875cc31e21f0842f725f8a1dd7ea5b3f5d62521c8de5324f4865191b43e0095f3368e119f9dae06e1d6ba818afd5b77e086e70171bcb659ede73ff14d1c53caec976c61387c013beb4798e123dbbeacd9f7c5511f3b5c1dd8a7f1d10a9b098ed7582e40e60ab6ffd6085ac0d2ca86ff5b584ac0d93a66ff15f84ac0df9a46fed0a84ac0d60a36fe9b583ac0dc6a16fe55f83ac0d2ca06fe10a83ac0d939e6fddb582ac0d609b6fd50a82ac0dc6996fd1b581ac0d2c986fcd5f81ac0d93966fc90a81ac0df9946fc5b480ac0d60936fc15f80ac0dc6916fbd0a80ac0dd782c1bd8a80ac0d607beabdca80ac0de87313be0a81ac0d716c3cbe4a81ac0df96465be8a81ac0d825d8ebeca81ac0d0b56b7be0a82ac0d1c4709bf8a82ac0da43f32bfca82ac0d2d385bbf0a83ac0db53084bf4a83ac0d3e29adbf8a83ac0dc621d6bfca83ac0d4f1affbf0a84ac0d00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
			var bytes = Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				.ToArray();
			var pack = new LifxPacket(bytes);
			Console.WriteLine("Captured colorzones: " + JsonConvert.SerializeObject(pack));

			var foobert = ParseMessage(bytes, new IPEndPoint(IPAddress.Any, 56700), _client);
			
			
			Console.WriteLine( "IsLittleEndian:  {0}",
				BitConverter.IsLittleEndian );
			Console.WriteLine("What device would you like to emulate? 1-4");
			Console.WriteLine("(0) - Bulb");
			Console.WriteLine("(1) - Z-LED Gen 1");
			Console.WriteLine("(2) - Z-LED Gen 2");
			Console.WriteLine("(3) - Beam");
			Console.WriteLine("(4) - Tile");
			Console.WriteLine("(5) - Switch");

			_deviceVersion = int.Parse(Console.ReadLine() ?? "0");
			
			switch (_deviceVersion) {
				case 0:
					_productInfo = ProductData.GetProduct(49);
					break;
				case 1:
					_productInfo = ProductData.GetProduct(31);
					break;
				case 2:
					_productInfo = ProductData.GetProduct(32);
					break;
				case 3:
					_productInfo = ProductData.GetProduct(38);
					break;
				case 4:
					_productInfo = ProductData.GetProduct(55);
					break;
				case 5:
					_productInfo = ProductData.GetProduct(70);
					break;
			}
			
			Console.WriteLine("Emulating product: " + _productInfo.Name);
			StartListener().Wait();
		}

		/// <summary>
		/// Handles the close using the specified sender
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="args">The args</param>
		private static void HandleClose(object sender, ConsoleCancelEventArgs args) {
			_quitFlag = true;
		}

		/// <summary>
		/// Starts the listener
		/// </summary>
		private static async Task StartListener() {
			try {
				_data = LoadData();
			} catch (Exception e) {
				Console.WriteLine("Exception loading data: " + e.Message, e.StackTrace);
			}

			_data.ProductData = _productInfo;
			
			var client = _client;
			while (_quitFlag == false) {
				try {
					var result = await client.ReceiveAsync();
					if (result.Buffer.Length <= 0) {
						continue;
					}

					await HandleIncomingMessages(result.Buffer, result.RemoteEndPoint, client);
				} catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
			}

			Console.WriteLine("Canceled.");
		}


		/// <summary>
		/// Handles the incoming messages using the specified data
		/// </summary>
		/// <param name="data">The data</param>
		/// <param name="endpoint">The endpoint</param>
		/// <param name="client">The client</param>
		private static async Task HandleIncomingMessages(byte[] data, IPEndPoint endpoint, UdpClient client) {
			var remote = endpoint;
			await ParseMessage(data, remote, client);

			
		}


		/// <summary>
		/// Broadcasts the message using the specified target
		/// </summary>
		/// <param name="target">The target</param>
		/// <param name="packet">The packet</param>
		/// <param name="client">The client</param>
		private static async Task BroadcastMessageAsync(IPEndPoint target, LifxPacket packet, UdpClient client) {
			var msg = packet.Encode();
			await client.SendAsync(msg, msg.Length, target);
		}

		/// <summary>
		/// Loads the data
		/// </summary>
		/// <returns>The device data</returns>
		private static DeviceData LoadData() {
			var basePath = AppDomain.CurrentDomain.BaseDirectory;
			var path = Path.Combine(basePath, $"config{_deviceVersion}.json");
			if (File.Exists(path)) {
				var jsonText = File.ReadAllText(path);
				return JsonConvert.DeserializeObject<DeviceData>(jsonText);
			}
			Console.WriteLine("Trying to create new DD...");
			return new DeviceData();
		}

		/// <summary>
		/// Saves the data
		/// </summary>
		private static void SaveData() {
			var basePath = AppDomain.CurrentDomain.BaseDirectory;
			var path = Path.Combine(basePath, $"config{_deviceVersion}.json");
			try {
				// serialize JSON directly to a file
				using var file = File.CreateText(path);
				var serializer = new JsonSerializer();
				serializer.Serialize(file, _data);
			} catch (Exception e) {
				Console.WriteLine("Exception: " + e.Message);
			}
		}


		/// <summary>
		/// Parses the message using the specified packet
		/// </summary>
		/// <param name="packet">The packet</param>
		/// <param name="incoming">The incoming</param>
		/// <param name="client">The client</param>
		private static async Task ParseMessage(byte[] packet, IPEndPoint incoming, UdpClient client) {
			var msg = new LifxPacket(packet);
			Debug.WriteLine( $"{incoming.Address}=>LOCAL::{msg.Type}: " + JsonConvert.SerializeObject(msg));

			var colors = new List<LifxColor>();
			if (msg.Type == MessageType.SetColorZones) {
				var start = msg.Payload.GetUint8();
				var end = msg.Payload.GetUint8();
				var color = msg.Payload.GetColor();
				Console.WriteLine($"Setting zones {start} - {end} to {color.ToHsbkString()}");
			}

			if (msg.Type == MessageType.SetTileState64) {
				msg.Payload.Advance(9);
				Console.WriteLine("Colors: ");
				for (var i = 0; i < 64; i++) {
					var color = msg.Payload.GetColor();
					colors.Add(color);
					Console.WriteLine(i + " is " + color.ToHsbkString());
				}

				_data.ColorArray = colors.ToArray();
				Console.WriteLine("");
			}

			if (msg.Type == MessageType.LightSetColor) {
				msg.Payload.Advance();
				var color = msg.Payload.GetColor();
				
				_data.Color = color;
				Console.WriteLine("Input is " + color.ToHsbkString());
			}
			
			if (msg.Type == MessageType.LightSetWaveform) {
				msg.Payload.Advance(2); // Res, transient
				var color = msg.Payload.GetColor();
				_data.Color = color;
				Console.WriteLine("Input is " + color.ToHsbkString());
			}
			
			if (msg.Type == MessageType.LightSetWaveformOptional) {
				msg.Payload.Advance(2); // Res, transient
				var color = msg.Payload.GetColor();
				var period = msg.Payload.GetUInt32();
				var cycles = msg.Payload.GetFloat32();
				var skewRatio = msg.Payload.GetInt16();
				var waveform = msg.Payload.GetUint8();
				var setHue = msg.Payload.GetUint8() == 1;
				var setBright = msg.Payload.GetUint8() == 1;
				var setSat = msg.Payload.GetUint8() == 1;
				var setK = msg.Payload.GetUint8() == 1;
				if (setHue && setSat && setBright && setK) {
					_data.Color = color;	
				}

				if (setHue) {
					//_data.Color.Hue = color.Hue;
				}
				
				if (setBright) {
					//_data.Color.Brightness = color.Brightness;
				}
				
				if (setSat) {
					//_data.Color.Saturation = color.Saturation;
				}
				
				if (setK) {
					_data.Color.Kelvin = color.Kelvin;
				}
				
				Console.WriteLine("Input is " + color.ToHsbkString());
			}

			if (msg.Type == MessageType.LightSetPower || msg.Type == MessageType.DeviceSetPower) {
				var lvl = msg.Payload.GetUInt16();
				_data.PowerLevel = lvl;
				var bString = string.Join(",", (from a in msg.Payload.ToArray() select a.ToString("X2")).ToArray());
				Console.WriteLine($"Power payload {lvl}: " + bString);
			}

			if (msg.Type == MessageType.DeviceSetLabel) {
				_data.Label = msg.Payload.GetString(32);
				//Console.WriteLine("New label: " + _data.Label);
			}
			
			if (msg.Type == MessageType.DeviceSetLocation) {
				_data.Location = msg.Payload.GetBytes(16);
				_data.LocationLabel = msg.Payload.GetString(32);
				var updated = msg.Payload.GetDate();
				Console.WriteLine("Updated: " + updated);
				_data.LocationUpdated = DateTime.Now;
			}
			
			if (msg.Type == MessageType.DeviceSetGroup) {
				_data.Group = msg.Payload.GetBytes(16);
				_data.GroupLabel = msg.Payload.GetString(32);
				var updated = msg.Payload.GetDate();
				Console.WriteLine("Updated: " + updated);
				_data.GroupUpdated = DateTime.Now;
			}

			if (msg.Type == MessageType.DeviceSetOwner) {
				Console.WriteLine("Setting owner!");
				
			}

			if (msg.Type == MessageType.SetExtendedColorZones) {
				Console.WriteLine("Reading color zones: " + msg.Payload.ToString());
				
				var dur = msg.Payload.GetUInt32();
				Console.WriteLine("DUR: " + dur);
				var apply = msg.Payload.GetUint8();
				Console.WriteLine("apply: " + apply);
				var idx = msg.Payload.GetUInt16();
				Console.WriteLine("Idx: " + idx);
				var count = msg.Payload.GetUint8();
				Console.WriteLine("Count: " + count);
				var ColorArray = new LifxColor[count];
				for (var i = 0; i < count; i++) {
					ColorArray[i] = msg.Payload.GetColor();
				}
				Console.WriteLine("No, really, data:: " + JsonConvert.SerializeObject(ColorArray));
			}

			if (msg.Type == MessageType.MultiZoneSetEffect) {
				_data.MultizoneId = msg.Payload.GetUInt32();
				_data.EffectType = msg.Payload.GetUint8();
				msg.Payload.Advance(2); // Reserved
				_data.EffectSpeed = msg.Payload.GetUInt32();
				_data.EffectDur = msg.Payload.GetUInt64();
				msg.Payload.Advance(8); // Reserved 2
				_data.EffectParam = msg.Payload.GetBytes(32);
				Console.WriteLine("Here's some data: " + JsonConvert.SerializeObject(_data));
			}

			SaveData();

			// First send ack if required
			if (msg.AcknowledgeRequired) {
				var ack = LifxResponse.Create(MessageType.DeviceAcknowledgement, _productInfo, _data);
				ack.Target = msg.Target;
				await BroadcastMessageAsync(incoming, ack, client);
			}
			if (msg.ResponseRequired || msg.Type == MessageType.DeviceGetService) {
				
				var lr = LifxResponse.Create(msg.Type, _productInfo, _data);
				var res = lr;
				if (res.Type == MessageType.DeviceStateService) {
					res.Target = GetMacAddress();
				} else {
					res.Target = msg.Target;
				}
				Debug.WriteLine( $"LOCAL=>{incoming.Address}::{res.Type}: " + JsonConvert.SerializeObject(msg));
				if (res.Type == MessageType.LightStatePower) {
					var bString = string.Join(",", (from a in res.Payload.ToArray() select a.ToString("X2")).ToArray());
					Console.WriteLine($"Power payload (out): " + bString);
				}

				if (res.Type == MessageType.LightState) {
					var pl = res.Payload;
					var color = pl.GetColor();
					pl.Advance(2);
					var power = pl.GetUInt16();
					var label = pl.GetString(32);
					var bString = string.Join(",", (from a in res.Payload.ToArray() select a.ToString("X2")).ToArray());
					//Console.WriteLine($"Lightstate payload: " + bString);
					//Console.Write($"Light state for {label}: {power}", color);
				}

				if (res.Type == MessageType.DeviceStateLocation) {
					Console.WriteLine("Ougoing location payload: " + res.Payload.ToString());
					
				} 
				await BroadcastMessageAsync(incoming, res, client);
			}
		}

		/// <summary>
		/// Gets the mac address
		/// </summary>
		/// <returns>The mac</returns>
		private static byte[] GetMacAddress() {
			var mac = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
			var interfaces = NetworkInterface.GetAllNetworkInterfaces();
			if (interfaces.Length < 1) return mac;

			foreach (var adapter in interfaces) {
				if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet) {
					continue;
				}

				var address = adapter.GetPhysicalAddress();
				var bytes = address.GetAddressBytes().ToList();
				bytes.Add(0);
				bytes.Add(0); // Pad bytes
				return bytes.ToArray();
			}

			return mac;
		}
	}
}