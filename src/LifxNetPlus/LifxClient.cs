using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LifxNetPlus {
	/// <summary>
	///     LIFX Client for communicating with bulbs
	/// </summary>
	public partial class LifxClient {
		/// <summary>
		/// The port
		/// </summary>
		private const int Port = 56700;
		/// <summary>
		/// The socket
		/// </summary>
		private readonly UdpClient _socket;
		/// <summary>
		/// The is running
		/// </summary>
		private bool _isRunning;

		/// <summary>
		/// Initializes a new instance of the <see cref="LifxClient"/> class
		/// </summary>
		private LifxClient() {
			IPEndPoint end = new IPEndPoint(IPAddress.Any, Port);
			_socket = new UdpClient(end) {Client = {Blocking = false}};
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				_socket.Client.DontFragment = true;
			}

			_socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		}

		/// <summary>
		///     Disposes the client
		/// </summary>
		public void Dispose() {
			_isRunning = false;
			_socket.Dispose();
		}

		/// <summary>
		///     Creates a new LIFX client.
		/// </summary>
		/// <returns>client</returns>
		public static Task<LifxClient> CreateAsync() {
			LifxClient client = new LifxClient();
			client.Initialize();
			return Task.FromResult(client);
		}

		/// <summary>
		/// Initializes this instance
		/// </summary>
		private void Initialize() {
			_isRunning = true;
			StartReceiveLoop();
		}


		/// <summary>
		/// Starts the receive loop
		/// </summary>
		private void StartReceiveLoop() {
			Task.Run(async () => {
				while (_isRunning) {
					try {
						var result = await _socket.ReceiveAsync();
						if (result.Buffer.Length > 0) {
							HandleIncomingMessages(result.Buffer, result.RemoteEndPoint);
						}
					} catch {
						// ignored
					}
				}
			});
		}

		/// <summary>
		/// Handles the incoming messages using the specified data
		/// </summary>
		/// <param name="data">The data</param>
		/// <param name="endpoint">The endpoint</param>
		private void HandleIncomingMessages(byte[] data, IPEndPoint endpoint) {
			var remote = endpoint;
			var msg = ParseMessage(data);
			switch (msg.Packet.Type) {
				case MessageType.DeviceStateService:
					ProcessDeviceDiscoveryMessage(remote.Address, (StateServiceResponse) msg);
					break;
				default:
					if (_taskCompletions.ContainsKey(msg.Packet.Sequence)) {
						var tcs = _taskCompletions[msg.Packet.Sequence];
						tcs(msg);
					}

					break;
			}

			// if (remote.Port == 56700)
			// 	Debug.WriteLine("Received {0} from {1}:{2}", msg.Type, remote,
			// 		string.Join(",", (from a in data select a.ToString("X2")).ToArray()));
		}

		/// <summary>
		/// Broadcasts the message using the specified packet
		/// </summary>
		/// <typeparam name="T">The </typeparam>
		/// <param name="packet">The packet</param>
		/// <returns>A task containing the</returns>
		private Task<T?> BroadcastMessageAsync<T>(LifxPacket packet) where T : LifxResponse {
			return BroadcastPayloadAsync<T>("255.255.255.255", packet);
		}

		/// <summary>
		/// Create a hex string from a byte array
		/// </summary>
		/// <param name="ba"></param>
		/// <returns></returns>
		public static string HexString(byte[] ba) {
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (var b in ba) {
				hex.AppendFormat("{0:x2}", b);
			}

			return hex.ToString();
		}

		/// <summary>
		/// Broadcasts the message using the specified device
		/// </summary>
		/// <typeparam name="T">The </typeparam>
		/// <param name="device">The device</param>
		/// <param name="packet">The packet</param>
		/// <returns>The response</returns>
		private Task<T?> BroadcastMessageAsync<T>(Device device, LifxPacket packet)
			where T : LifxResponse {
			var hostname = device.HostName;
			packet.Target = device.MacAddress;
			var response = BroadcastPayloadAsync<T>(hostname, packet);
			return response;
		}


		/// <summary>
		/// Broadcasts the message using the specified device
		/// </summary>
		/// <param name="device">The device</param>
		/// <param name="packet">The packet</param>
		/// <exception cref="InvalidOperationException">No valid socket</exception>
		private async Task BroadcastMessageAsync(Device device, LifxPacket packet) {
			var hostname = device.HostName;
			packet.Target = device.MacAddress;
			if (_socket == null) {
				throw new InvalidOperationException("No valid socket");
			}

			var msg = packet.Encode();
			await _socket.SendAsync(msg, msg.Length, hostname, Port);
		}

		/// <summary>
		/// Broadcasts the payload using the specified host
		/// </summary>
		/// <typeparam name="T">The </typeparam>
		/// <param name="host">The host</param>
		/// <param name="packet">The packet</param>
		/// <exception cref="InvalidOperationException">No valid socket</exception>
		/// <returns>The result</returns>
		private async Task<T?> BroadcastPayloadAsync<T>(string host, LifxPacket packet)
			where T : LifxResponse {
			if (_socket == null) {
				throw new InvalidOperationException("No valid socket");
			}


			TaskCompletionSource<T>? tcs = null;
			if (packet.Sequence > 0 &&
			    typeof(T) != typeof(UnknownResponse)) {
				tcs = new TaskCompletionSource<T>();
				Action<LifxResponse> action = r => {
					if (r.GetType() == typeof(T)) {
						tcs.TrySetResult((T) r);
					}
				};
				_taskCompletions[packet.Sequence] = action;
			}

			var msg = packet.Encode();
			await _socket.SendAsync(msg, msg.Length, host, Port);
			
			T? result = default;
			if (tcs == null) {
				return result;
			}

			var _ = Task.Delay(1000).ContinueWith(t => {
				if (!t.IsCompleted) {
					tcs.TrySetException(new TimeoutException());
				}
			});
			try {
				result = await tcs.Task;
			} finally {
				_taskCompletions.Remove(packet.Sequence);
			}

			return result;
		}


		/// <summary>
		/// Parse a Lifx Message to a response
		/// </summary>
		/// <param name="packet">The incoming bytes to parse.</param>
		/// <returns>A <see cref="LifxResponse"/></returns>
		public static LifxResponse ParseMessage(byte[] packet) {
			var fh = new LifxPacket(packet);
			var res = LifxResponse.Create(fh);
			return res;
		}
	}
}