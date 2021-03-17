using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

		private LifxClient() {
			IPEndPoint end = new IPEndPoint(IPAddress.Any, Port);
			_socket = new UdpClient(end) {Client = {Blocking = false}};
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				_socket.Client.DontFragment = true;
			}
			_socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		}

		/// <summary>
		/// Disposes the client
		/// </summary>
		public void Dispose() {
			_isRunning = false;
			_socket.Dispose();
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
			var msg = ParseMessage(data, endpoint);
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

		private Task<T> BroadcastMessageAsync<T>(LifxPacket packet) where T : LifxResponse {
			Debug.WriteLine("Broadcasting discovery packet.");
			return BroadcastPayloadAsync<T>("255.255.255.255", packet);
		}

		private Task<T> BroadcastMessageAsync<T>(Device device, LifxPacket packet) where T : LifxResponse {
			var hostname = device.HostName;
			packet.Target = device.MacAddress;
			Debug.WriteLine($"LOCAL=>{hostname}::{packet.Type}: " + JsonConvert.SerializeObject(packet));
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


		private static LifxResponse ParseMessage(byte[] packet, IPEndPoint ep) {
			var fh = new LifxPacket(packet);
			Debug.WriteLine($"{ep.Address}L=>LOCAL::{fh.Type}: " + JsonConvert.SerializeObject(packet));
			return LifxResponse.Create(fh);
		}
	}
}