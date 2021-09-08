using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LifxNetPlus {
	/// <summary>
	/// The lifx client class
	/// </summary>
	public partial class LifxClient {
		/// <summary>
		///     Gets a list of currently known devices
		/// </summary>
		public IEnumerable<Device> Devices => devices;

		/// <summary>
		/// The device
		/// </summary>
		private readonly Dictionary<string, Device> _discoveredBulbs = new Dictionary<string, Device>();
		/// <summary>
		/// The strip ids
		/// </summary>
		private readonly int[] _stripIds = {31, 32, 38};
		/// <summary>
		/// The switch ids
		/// </summary>
		private readonly int[] _switchIds = {70};
		/// <summary>
		/// The tile ids
		/// </summary>
		private readonly int[] _tileIds = {55};
		/// <summary>
		/// The discover cancellation source
		/// </summary>
		private CancellationTokenSource? _discoverCancellationSource;

		/// <summary>
		/// The device
		/// </summary>
		private IList<Device> devices = new List<Device>();


		/// <summary>
		///     Event fired when a LIFX bulb is discovered on the network
		/// </summary>
		public event EventHandler<DeviceDiscoveryEventArgs>? DeviceDiscovered;

		/// <summary>
		///     Event fired when a LIFX bulb hasn't been seen on the network for a while (for more than 5 minutes)
		/// </summary>
		public event EventHandler<DeviceDiscoveryEventArgs>? DeviceLost;

		/// <summary>
		/// Processes the device discovery message using the specified remote address
		/// </summary>
		/// <param name="remoteAddress">The remote address</param>
		/// <param name="msg">The msg</param>
		private void ProcessDeviceDiscoveryMessage(IPAddress remoteAddress, StateServiceResponse msg) {
			string id = msg.TargetMacAddressName;

			if (_discoveredBulbs.ContainsKey(id)) {
				_discoveredBulbs[id].LastSeen = DateTime.UtcNow; //Update datestamp
				_discoveredBulbs[id].HostName = remoteAddress.ToString(); //Update hostname in case IP changed
				return;
			}

			if (msg.Source != MessageId.GetSource() || //did we request the discovery?
			    _discoverCancellationSource == null ||
			    _discoverCancellationSource.IsCancellationRequested) {
				return;
			}

			var address = remoteAddress.ToString();
			var mac = msg.Target;
			msg.Payload.Reset();
			var svc = msg.Service;
			var port = msg.Port;
			var lastSeen = DateTime.UtcNow;
			var device = new Device(address, mac, svc, port) {
				LastSeen = lastSeen
			};

			_discoveredBulbs[id] = device;
			devices.Add(device);
			DeviceDiscovered?.Invoke(this, new DeviceDiscoveryEventArgs(device));
		}

		/// <summary>
		///     Begins searching for bulbs.
		/// </summary>
		/// <seealso cref="DeviceDiscovered" />
		/// <seealso cref="DeviceLost" />
		/// <seealso cref="StopDeviceDiscovery" />
		public void StartDeviceDiscovery() {
			// Reset our list of devices on discovery start
			devices = new List<Device>();
			if (_discoverCancellationSource != null && !_discoverCancellationSource.IsCancellationRequested) {
				return;
			}

			_discoverCancellationSource = new CancellationTokenSource();
			var token = _discoverCancellationSource.Token;
			var discoPacket = new LifxPacket(MessageType.DeviceGetService);
			//Start discovery thread
			Task.Run(async () => {
				await BroadcastMessageAsync<UnknownResponse>(discoPacket);
				while (!token.IsCancellationRequested) {
					try {
						//await BroadcastMessageAsync<UnknownResponse>(null, header,
						//MessageType.DeviceGetService);
					} catch (Exception) {
						//Debug.WriteLine("Broadcast exception: " + e.Message + e.StackTrace);
					}

					await Task.Delay(1, token);
					var lostDevices = devices.Where(d => (DateTime.UtcNow - d.LastSeen).TotalMinutes > 5).ToArray();
					if (!lostDevices.Any()) {
						continue;
					}

					foreach (var device in lostDevices) {
						devices.Remove(device);
						_discoveredBulbs.Remove(device.MacAddressName);
						DeviceLost?.Invoke(this, new DeviceDiscoveryEventArgs(device));
					}
				}
			}, token);
		}

		/// <summary>
		///     Stops device discovery
		/// </summary>
		/// <seealso cref="StartDeviceDiscovery" />
		public void StopDeviceDiscovery() {
			if (_discoverCancellationSource == null || _discoverCancellationSource.IsCancellationRequested) {
				return;
			}

			_discoverCancellationSource.Cancel();
			_discoverCancellationSource = null;
		}

		/// <summary>
		///     Event args for <see cref="LifxClient.DeviceDiscovered" /> and <see cref="LifxClient.DeviceLost" /> events.
		/// </summary>
		public sealed class DeviceDiscoveryEventArgs : EventArgs {
			/// <summary>
			///     The device the event relates to
			/// </summary>
			public Device Device { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="DeviceDiscoveryEventArgs"/> class
			/// </summary>
			/// <param name="device">The device</param>
			internal DeviceDiscoveryEventArgs(Device device) {
				Device = device;
			}
		}
	}
}