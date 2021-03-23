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
	internal static class Program {
		private static int _deviceVersion;
		private static bool _quitFlag;
		private static DeviceData _data;
		private static Product _productInfo;
		private static List<string> _messages;
		public static void Main() {
			_messages = new List<string>();
			var tr1 = new TextWriterTraceListener(Console.Out);
			Trace.Listeners.Add(tr1);
			Console.CancelKeyPress += HandleClose;
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
			
			Console.WriteLine("Emulating product: " + _productInfo.name);
			StartListener().Wait();
		}

		private static void HandleClose(object sender, ConsoleCancelEventArgs args) {
			_quitFlag = true;
		}

		private static async Task StartListener() {
			try {
				_data = LoadData();
			} catch (Exception e) {
				Console.WriteLine("Exception loading data: " + e.Message, e.StackTrace);
			}

			_data.ProductData = _productInfo;
			var end = new IPEndPoint(IPAddress.Any, 56700);
			var client = new UdpClient(end) {Client = {Blocking = false}};
			client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			client.Client.SendBufferSize = 4096;
			client.Client.ReceiveBufferSize = 4096;
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


		private static async Task HandleIncomingMessages(byte[] data, IPEndPoint endpoint, UdpClient client) {
			var remote = endpoint;
			await ParseMessage(data, remote, client);

			
		}


		private static async Task BroadcastMessageAsync(IPEndPoint target, LifxPacket packet, UdpClient client) {
			var msg = packet.Encode();
			await client.SendAsync(msg, msg.Length, target);
		}

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


		private static async Task ParseMessage(byte[] packet, IPEndPoint incoming, UdpClient client) {
			var msg = new LifxPacket(packet);
			if (!_messages.Contains(msg.Type.ToString())) {
				Debug.WriteLine( $"{incoming.Address}=>LOCAL::{msg.Type}: " + JsonConvert.SerializeObject(msg));
				_messages.Add(msg.Type.ToString());
			}
			var colors = new List<LifxColor>();
			if (msg.Type == MessageType.SetColorZones) {
				var start = msg.Payload.GetUint8();
				var end = msg.Payload.GetUint8();
				var color = msg.Payload.GetColor();
				Console.WriteLine($"Setting zones {start} - {end} to {color.ToHsbkString()}", color.Color);
			}

			if (msg.Type == MessageType.SetTileState64) {
				msg.Payload.Advance(9);
				Console.WriteLine("Colors: ");
				for (var i = 0; i < 64; i++) {
					var color = msg.Payload.GetColor();
					colors.Add(color);
					Console.WriteLine(i + " is " + color.ToHsbkString(), color.Color);
				}

				_data.ColorArray = colors.ToArray();
				Console.WriteLine("");
			}

			if (msg.Type == MessageType.LightSetColor) {
				msg.Payload.Advance();
				var color = msg.Payload.GetColor();
				
				_data.Color = color;
				Console.WriteLine("Input is " + color.ToHsbkString(), color.Color);
			}
			
			if (msg.Type == MessageType.LightSetWaveform) {
				msg.Payload.Advance(2); // Res, transient
				var color = msg.Payload.GetColor();
				_data.Color = color;
				Console.WriteLine("Input is " + color.ToHsbkString(), color.Color);
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
					_data.Color.Hue = color.Hue;
				}
				
				if (setBright) {
					_data.Color.Brightness = color.Brightness;
				}
				
				if (setSat) {
					_data.Color.Saturation = color.Saturation;
				}
				
				if (setK) {
					_data.Color.Kelvin = color.Kelvin;
				}
				
				Console.WriteLine("Input is " + color.ToHsbkString(), color.Color);
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
				_data.LocationUpdated = msg.Payload.GetUInt64();
				Console.WriteLine("Incoming location payload: " + msg.Payload.ToString());
			}
			
			if (msg.Type == MessageType.DeviceSetGroup) {
				_data.Group = msg.Payload.GetBytes(16);
				_data.GroupLabel = msg.Payload.GetString(32);
				_data.GroupUpdated = msg.Payload.GetUInt64();
				//Console.WriteLine("New group payload: " + msg.Payload);
			}

			if (msg.Type == MessageType.DeviceSetOwner) {
				Console.WriteLine("Setting owner!");
				
			}

			if (msg.Type == MessageType.MultiZoneSetEffect) {
				_data.MultizoneId = msg.Payload.GetUInt32();
				_data.EffectType = msg.Payload.GetUint8();
				msg.Payload.Advance(2); // Reserved
				_data.EffectSpeed = msg.Payload.GetUInt32();
				_data.EffectDur = msg.Payload.GetUInt64();
				msg.Payload.Advance(8); // Reserved 2
				_data.EffectParam = msg.Payload.GetBytes(32);
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
				//Debug.WriteLine( $"LOCAL=>{incoming.Address}::{res.Type}: " + JsonConvert.SerializeObject(msg));
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