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
using Console = Colorful.Console;

namespace LifxEmulator {
	internal static class Program {
		private static bool _quitFlag;
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private static int _deviceVersion;
		private static uint _identifier = 1;
		private static readonly object IdentifierLock = new object();

		
		public static void Main(string[] args) {
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
		
		private static uint GetNextIdentifier() {
			lock (IdentifierLock) {
				_identifier++;
			}

			return _identifier;
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
			var msg = await ParseMessage(data, endpoint);
			
			if (msg.GetType() != typeof(AcknowledgementResponse) || msg.Header.AcknowledgeRequired) {
				Debug.WriteLine($"Sending {msg.Type} to " + remote.Address);
				await BroadcastMessageAsync(remote, msg, client);
			}
		}
		
		private static async Task BroadcastMessageAsync(IPEndPoint target, LifxResponse message, UdpClient client) {
			
			using var stream = new MemoryStream();
			WritePacketToStream(stream, message.Header, (ushort) message.Type, message.Payload);
			var msg = stream.ToArray();
			var text = string.Join(",", (from a in msg select a.ToString("X2")).ToArray());
			Debug.WriteLine($"Sending message to {target.Address}: " + text);

			await client.SendAsync(msg, msg.Length, target);
		}
		
		private static void WritePacketToStream(Stream outStream, FrameHeader header, ushort type, Payload payload) {
			if (payload == null) {
				Console.WriteLine("No payload, creating new...");
				payload = new Payload();
			}
			using var dw = new BinaryWriter(outStream);

			#region Frame
			dw.Write((ushort) (payload.Length + 36));
			if (type == 2) {
				dw.Write((ushort)0x3400);
			} else {
				dw.Write((byte) 0);
				dw.Write((byte) 20);
			}

			if (type == 54 || type == 201) {
				dw.Write((byte)17);
				dw.Write((byte)0);
				dw.Write((byte)173);
				dw.Write((byte)176);	
				
			} else {
				dw.Write(header.Identifier); //source identifier - unique value set by the client, used by responses. If 0, responses are broadcast instead	
			}
			

			#endregion Frame

			#region Frame address

			//The target device address is 8 bytes long, when using the 6 byte MAC address then left - 
			//justify the value and zero-fill the last two bytes. A target device address of all zeroes effectively addresses all devices on the local network
			if (type == 2) {
				dw.Write(new byte[] {0, 0, 0, 0, 0, 0, 0, 0});
			} else {
				dw.Write(GetMacAddress());
			}
			
			dw.Write(new byte[] {0, 0, 0, 0, 0, 0}); //reserved 1

			//The client can use acknowledgements to determine that the LIFX device has received a message. 
			//However, when using acknowledgements to ensure reliability in an over-burdened lossy network ... 
			//causing additional network packets may make the problem worse. 
			//Client that don't need to track the updated state of a LIFX device can choose not to request a 
			//response, which will reduce the network burden and may provide some performance advantage. In
			//some cases, a device may choose to send a state update response independent of whether res_required is set.
			if (header.AcknowledgeRequired && header.ResponseRequired)
				dw.Write((byte) 0x03);
			else if (header.AcknowledgeRequired)
				dw.Write((byte) 0x02);
			else if (header.ResponseRequired)
				dw.Write((byte) 0x01);
			else
				dw.Write((byte) 0x00);
			//The sequence number allows the client to provide a unique value, which will be included by the LIFX 
			//device in any message that is sent in response to a message sent by the client. This allows the client
			//to distinguish between different messages sent with the same source identifier in the Frame. See
			//ack_required and res_required fields in the Frame Address.
			dw.Write(header.Sequence);

			#endregion Frame address

			#region Protocol Header

			//The at_time value should be zero for Set and Get messages sent by a client.
			//For State messages sent by a device, the at_time will either be the device
			//current time when the message was received or zero. StateColor is an example
			//of a message that will return a non-zero at_time value
			if (header.AtTime > DateTime.MinValue) {
				var time = header.AtTime.ToUniversalTime();
				dw.Write((ulong) (time - new DateTime(1970, 01, 01)).TotalMilliseconds * 10); //timestamp
			} else {
				dw.Write((ulong) 0);
			}

			#endregion Protocol Header

			dw.Write(type); //packet _type
			dw.Write((ushort) 0); //reserved
			dw.Write(payload.ToArray());
			dw.Flush();
		}
		
		private static async Task<LifxResponse> ParseMessage(byte[] packet, IPEndPoint ep) {
			using MemoryStream ms = new MemoryStream(packet);
			BinaryReader br = new BinaryReader(ms);
			//frame
			var size = br.ReadUInt16();
			if (packet.Length != size || size < 36)
				throw new Exception("Invalid packet");
			br.ReadUInt16(); //origin:2, reserved:1, addressable:1, protocol:12
			var source = br.ReadUInt32();
			//frame address
			byte[] target = br.ReadBytes(8);
			var header = new FrameHeader(source);
			header.TargetMacAddress = target;
			ms.Seek(6, SeekOrigin.Current); //skip reserved
			br.ReadByte(); //reserved:6, ack_required:1, res_required:1, 
			header.Sequence = br.ReadByte();
			//protocol header
			var nanoseconds = br.ReadUInt64();
			header.AtTime = Epoch.AddMilliseconds(nanoseconds * 0.000001);
			var type = (MessageType) br.ReadUInt16();
			Console.WriteLine($"Received {type} from {ep.Address}.");
			ms.Seek(2, SeekOrigin.Current); //skip reserved
			var bytes = size > 36 ? br.ReadBytes(size - 36) : new byte[] { };
			var payload = new Payload(bytes);
			var msg = string.Join(",", (from a in packet select a.ToString("X2")).ToArray());
			Console.WriteLine("Payload: " + msg);
		
			if (type == MessageType.SetColorZones) {
				var start = payload.GetUint8();
				var end = payload.GetUint8();
				var color = payload.GetColor();
				Debug.WriteLine($"Setting zones {start} - {end} to {color.ToHsbkString()}", color.Color);
			}

			if (type == MessageType.SetTileState64) {
				payload.Advance(9);
				Console.WriteLine("Colors: ");
				for (var i = 0; i < 64; i++) {
					var color = payload.GetColor();
					Console.WriteLine(i + " is " + color.ToHsbkString(), color.Color);
				}
				Console.WriteLine("");
			}

			var newHeader = new FrameHeader(GetNextIdentifier()) {TargetMacAddress = header.TargetMacAddress};
			var res = LifxResponse.Create(newHeader, type, source,
				payload,_deviceVersion);
			await Task.FromResult(true);
			return res;
		}
		
		private static byte[] GetMacAddress()
		{
			var mac = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			if (interfaces.Length < 1) return mac;

			foreach (NetworkInterface adapter in interfaces) {
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
	
	
	
	internal class FrameHeader {
		public uint Identifier;
		public byte Sequence;
		public bool AcknowledgeRequired;
		public bool ResponseRequired;
		public byte[] TargetMacAddress = {0, 0, 0, 0, 0, 0, 0, 0};
		public DateTime AtTime = DateTime.MinValue;

		public FrameHeader() {
		}

		public FrameHeader(uint id, bool acknowledgeRequired = false) {
			Identifier = id;
			AtTime = DateTime.Now;
			AcknowledgeRequired = acknowledgeRequired;
		}

		public string TargetMacAddressName {
			get { return string.Join(":", TargetMacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray()); }
		}
	}
	
	
}