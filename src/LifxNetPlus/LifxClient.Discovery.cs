using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LifxNetPlus {
	public partial class LifxClient {
		/// <summary>
		///     Gets a list of currently known devices
		/// </summary>
		public IEnumerable<Device> Devices => devices;

		private readonly Dictionary<string, Device> _discoveredBulbs = new Dictionary<string, Device>();
		private readonly int[] _stripIds = {31, 32, 38};
		private readonly int[] _switchIds = {70};
		private readonly int[] _tileIds = {55};
		private CancellationTokenSource? _discoverCancellationSource;
		private uint _discoverSourceId;

		private IList<Device> devices = new List<Device>();


		/// <summary>
		///     Event fired when a LIFX bulb is discovered on the network
		/// </summary>
		public event EventHandler<DeviceDiscoveryEventArgs>? DeviceDiscovered;

		/// <summary>
		///     Event fired when a LIFX bulb hasn't been seen on the network for a while (for more than 5 minutes)
		/// </summary>
		public event EventHandler<DeviceDiscoveryEventArgs>? DeviceLost;

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
			var device = new LightBulb(address, mac, svc, port) {
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
					} catch (Exception e) {
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

			internal DeviceDiscoveryEventArgs(Device device) {
				Device = device;
			}
		}
	}
}