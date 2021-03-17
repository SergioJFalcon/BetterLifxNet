using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
		
		public static void Main() {
			var tr1 = new TextWriterTraceListener(Console.Out);
			Trace.Listeners.Add(tr1);
			Console.CancelKeyPress += HandleClose;
			Console.WriteLine("What device would you like to emulate? 1-4");
			Console.WriteLine("(0) - Bulb");
			Console.WriteLine("(1) - Z-LED Gen 1");
			Console.WriteLine("(2) - Z-LED Gen 2");
			Console.WriteLine("(3) - Beam");
			Console.WriteLine("(4) - Tile");
			Console.WriteLine("(5) - Switch");

			_deviceVersion = int.Parse(Console.ReadLine() ?? "0");
			Console.WriteLine("Emulation mode: " + _deviceVersion);
			StartListener().Wait();
		}

		private static void HandleClose(object sender, ConsoleCancelEventArgs args) {
			_quitFlag = true;
		}

		private static async Task StartListener() {
			var end = new IPEndPoint(IPAddress.Any, 56700);
			var client = new UdpClient(end) {Client = {Blocking = false}};
			client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			client.Client.SendBufferSize = 4096;
			client.Client.ReceiveBufferSize = 4096;
			Console.WriteLine("Starting listener...");
			while (_quitFlag == false) {
				Console.WriteLine("Loop.");
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
			var msg = await ParseMessage(data, remote);

			if (msg.GetType() != typeof(AcknowledgementResponse) || msg.AcknowledgeRequired) {
				Debug.WriteLine( $"LOCAL=>{remote.Address}::{msg.Type}: " + JsonConvert.SerializeObject(msg));
				await BroadcastMessageAsync(remote, msg, client);
			}
		}


		private static async Task BroadcastMessageAsync(IPEndPoint target, LifxPacket packet, UdpClient client) {
			var msg = packet.Encode();
			await client.SendAsync(msg, msg.Length, target);
		}


		private static async Task<LifxResponse> ParseMessage(byte[] packet, IPEndPoint incoming) {
			var msg = new LifxPacket(packet);
			Debug.WriteLine( $"{incoming.Address}=>LOCAL::{msg.Type}: " + JsonConvert.SerializeObject(msg));
			if (msg.Type == MessageType.SetColorZones) {
				var start = msg.Payload.GetUint8();
				var end = msg.Payload.GetUint8();
				var color = msg.Payload.GetColor();
				Debug.WriteLine($"Setting zones {start} - {end} to {color.ToHsbkString()}", color.Color);
			}

			if (msg.Type == MessageType.SetTileState64) {
				msg.Payload.Advance(9);
				Console.WriteLine("Colors: ");
				for (var i = 0; i < 64; i++) {
					var color = msg.Payload.GetColor();
					Console.WriteLine(i + " is " + color.ToHsbkString(), color.Color);
				}

				Console.WriteLine("");
			}

			if (msg.Type == MessageType.LightSetColor) {
				
			}

			var res = LifxResponse.Create(msg, _deviceVersion);
			await Task.FromResult(true);
			if (res.Type == MessageType.DeviceStateService) {
				res.Target = GetMacAddress();
			} else {
				res.Target = msg.Target;
			}

			return res;
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