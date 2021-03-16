using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LifxNetPlus {
	/// <summary>
	/// LIFX Client for communicating with bulbs
	/// </summary>
	public partial class LifxClient {
		private const int Port = 56700;
		private readonly UdpClient _socket;
		private bool _isRunning;
		private byte[] _macAddress;

		private LifxClient() {
			IPEndPoint end = new IPEndPoint(IPAddress.Any, Port);
			_socket = new UdpClient(end) {Client = {Blocking = false}, DontFragment = true};
			_socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			_macAddress = GetMacAddress();
		}

		/// <summary>
		/// Creates a new LIFX client.
		/// </summary>
		/// <returns>client</returns>
		public static Task<LifxClient> CreateAsync() {
			LifxClient client = new LifxClient();
			client.Initialize();
			return Task.FromResult(client);
		}

		private void Initialize() {
			_isRunning = true;
			StartReceiveLoop();
		}


		private void StartReceiveLoop() {
			Task.Run(async () => {
				while (_isRunning)
					try {
						var result = await _socket.ReceiveAsync();
						if (result.Buffer.Length > 0) {
							HandleIncomingMessages(result.Buffer, result.RemoteEndPoint);
						}
					} catch {
						// ignored
					}
			});
		}

		private void HandleIncomingMessages(byte[] data, IPEndPoint endpoint) {
			var remote = endpoint;
			var msg = ParseMessage(data);
			switch (msg.Packet.Type) {
				case MessageType.DeviceStateService:
					ProcessDeviceDiscoveryMessage(remote.Address, (StateServiceResponse) msg);
					break;
				default:
					if (_taskCompletions.ContainsKey(msg.Packet.Source)) {
						var tcs = _taskCompletions[msg.Packet.Source];
						tcs(msg);
					}

					break;
			}

			if (remote.Port == 56700)
				Debug.WriteLine("Received {0} from {1}:{2}", msg.Type, remote,
					string.Join(",", (from a in data select a.ToString("X2")).ToArray()));
		}

		/// <summary>
		/// Disposes the client
		/// </summary>
		public void Dispose() {
			_isRunning = false;
			_socket.Dispose();
		}
		
		private Task<T> BroadcastMessageAsync<T>(LifxPacket packet) where T : LifxResponse {
			
			
			Debug.WriteLine("Broadcasting discovery packet.");
			return BroadcastPayloadAsync<T>("255.255.255.255", packet);
		}

		private Task<T> BroadcastMessageAsync<T>(Device device, LifxPacket packet) where T : LifxResponse {
			
			var hostname = "255.255.255.255";
			
			if (device != null) {
				hostname = device.HostName;
				if (packet.Type != MessageType.DeviceGetService) {
					packet.Target = device.MacAddress;
				}
			}
		
			Debug.WriteLine("Broadcasting " + packet.Type + " to " + hostname);
			return BroadcastPayloadAsync<T>(hostname, packet);
		}
		
		private async Task<T> BroadcastPayloadAsync<T>(string host, LifxPacket packet)
			where T : LifxResponse {
			if (_socket == null)
				throw new InvalidOperationException("No valid socket");
			var data = packet.Encode();
			Debug.WriteLine(
				string.Join(",", (from a in data select a.ToString("X2")).ToArray()));


			TaskCompletionSource<T>? tcs = null;
			if (packet.Identifier > 0 &&
			    typeof(T) != typeof(UnknownResponse)) {
				tcs = new TaskCompletionSource<T>();
				Action<LifxResponse> action = (r) => {
					if (r.GetType() == typeof(T))
						tcs.TrySetResult((T) r);
				};
				_taskCompletions[packet.Identifier] = action;
			}

			var msg = packet.Encode();
			await _socket.SendAsync(msg, msg.Length, host, Port);

			//{
			//	await WritePacketToStreamAsync(stream, header, (UInt16)type, payload).ConfigureAwait(false);
			//}
			T result = default(T);
			if (tcs != null) {
				var _ = Task.Delay(1000).ContinueWith((t) => {
					if (!t.IsCompleted)
						tcs.TrySetException(new TimeoutException());
				});
				try {
					result = await tcs.Task.ConfigureAwait(false);
				} finally {
					_taskCompletions.Remove(packet.Identifier);
				}
			}

			return result;
		}
		
	
		private static LifxResponse ParseMessage(byte[] packet) {
			var fh = new LifxPacket(packet);
			Debug.WriteLine("Incoming message: " + JsonConvert.SerializeObject(fh));
			return LifxResponse.Create(fh);
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

				PhysicalAddress address = adapter.GetPhysicalAddress();
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

		public FrameHeader(bool acknowledgeRequired = false) {
			Identifier = MessageId.GetNextIdentifier();
			AcknowledgeRequired = acknowledgeRequired;
		}

		public string TargetMacAddressName {
			get { return string.Join(":", TargetMacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray()); }
		}
	}
}