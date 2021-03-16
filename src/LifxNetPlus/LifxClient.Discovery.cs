using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LifxNetPlus {
	public partial class LifxClient {
		
		private uint _discoverSourceId;
		private CancellationTokenSource? _discoverCancellationSource;
		private readonly Dictionary<string, Device> _discoveredBulbs = new Dictionary<string, Device>();
		private readonly int[] _stripIds = {31, 32, 38};
		private readonly int[] _tileIds = {55};
		private readonly int[] _switchIds = {70};

		

		/// <summary>
		/// Event fired when a LIFX bulb is discovered on the network
		/// </summary>
		public event EventHandler<DeviceDiscoveryEventArgs>? DeviceDiscovered;

		/// <summary>
		/// Event fired when a LIFX bulb hasn't been seen on the network for a while (for more than 5 minutes)
		/// </summary>
		public event EventHandler<DeviceDiscoveryEventArgs>? DeviceLost;

		private IList<Device> devices = new List<Device>();

		/// <summary>
		/// Gets a list of currently known devices
		/// </summary>
		public IEnumerable<Device> Devices => devices;

		/// <summary>
		/// Event args for <see cref="LifxClient.DeviceDiscovered"/> and <see cref="LifxClient.DeviceLost"/> events.
		/// </summary>
		public sealed class DeviceDiscoveryEventArgs : EventArgs {
			internal DeviceDiscoveryEventArgs(Device device) => Device = device;

			/// <summary>
			/// The device the event relates to
			/// </summary>
			public Device Device { get; }
		}

		private void ProcessDeviceDiscoveryMessage(IPAddress remoteAddress, StateServiceResponse msg) {
			string id = msg.TargetMacAddressName;
			Debug.WriteLine($"Processing device discovery message for {remoteAddress}: {id}");

			if (_discoveredBulbs.ContainsKey(id)) {
				_discoveredBulbs[id].LastSeen = DateTime.UtcNow; //Update datestamp
				_discoveredBulbs[id].HostName = remoteAddress.ToString(); //Update hostname in case IP changed
				Debug.WriteLine("Device already discovered, skipping.");
				return;
			}

			if (msg.Source != _discoverSourceId || //did we request the discovery?
			    _discoverCancellationSource == null ||
			    _discoverCancellationSource.IsCancellationRequested) {
				Debug.WriteLine("Source mismatch or cancellation...");
				return;	
			}

			var address = remoteAddress.ToString();
			var mac = msg.Target;
			msg.Payload.Reset();
			var svc = msg.Service;
			var port = msg.Port;
			var lastSeen = DateTime.UtcNow;
			Debug.WriteLine("Creating generic device: " + address + " and " + port);
			var device = new LightBulb(address, mac, svc, port) {
				LastSeen = lastSeen
			};

			_discoveredBulbs[id] = device;
			devices.Add(device);
			DeviceDiscovered?.Invoke(this, new DeviceDiscoveryEventArgs(device));
		}

		/// <summary>
		/// Begins searching for bulbs.
		/// </summary>
		/// <seealso cref="DeviceDiscovered"/>
		/// <seealso cref="DeviceLost"/>
		/// <seealso cref="StopDeviceDiscovery"/>
		public void StartDeviceDiscovery() {
			// Reset our list of devices on discovery start
			devices = new List<Device>();
			if (_discoverCancellationSource != null && !_discoverCancellationSource.IsCancellationRequested)
				return;
			_discoverCancellationSource = new CancellationTokenSource();
			var token = _discoverCancellationSource.Token;
			_discoverSourceId = MessageId.GetNextIdentifier();
			var discoPacket = new LifxPacket(_discoverSourceId, MessageType.DeviceGetService);
			//Start discovery thread
			Task.Run(async () => {
				Debug.WriteLine("Sending GetServices...");
				await BroadcastMessageAsync<UnknownResponse>(discoPacket);
				while (!token.IsCancellationRequested) {
					try {
						  //await BroadcastMessageAsync<UnknownResponse>(null, header,
						  //MessageType.DeviceGetService);
						
					} catch (Exception e) {
						Debug.WriteLine("Broadcast exception: " + e.Message + e.StackTrace);
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
		/// Stops device discovery
		/// </summary>
		/// <seealso cref="StartDeviceDiscovery"/>
		public void StopDeviceDiscovery() {
			if (_discoverCancellationSource == null || _discoverCancellationSource.IsCancellationRequested)
				return;
			_discoverCancellationSource.Cancel();
			_discoverCancellationSource = null;
		}
	}
}