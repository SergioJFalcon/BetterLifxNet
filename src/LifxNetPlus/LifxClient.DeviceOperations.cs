using System;
using System.Threading.Tasks;

namespace LifxNetPlus {
	/// <summary>
	/// The lifx client class
	/// </summary>
	public partial class LifxClient {
		/// <summary>
		///     Turns the device on
		/// </summary>
		public Task TurnDeviceOnAsync(Device device) {
			return SetDevicePowerStateAsync(device, true);
		}

		/// <summary>
		///     Turns the device off
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public Task TurnDeviceOffAsync(Device device) {
			return SetDevicePowerStateAsync(device, false);
		}

		/// <summary>
		///     Sets the device power state
		/// </summary>
		/// <param name="device"></param>
		/// <param name="isOn"></param>
		/// <returns></returns>
		public async Task SetDevicePowerStateAsync(Device device, bool isOn) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.DeviceSetPower, isOn ? 65535 : 0);

			_ = await BroadcastMessageAsync<AcknowledgementResponse>(device, packet);
		}

		/// <summary>
		///     Gets the label for the device
		/// </summary>
		/// <param name="device"></param>
		/// <returns>The device label</returns>
		public async Task<string?> GetDeviceLabelAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var resp = await BroadcastMessageAsync<StateLabelResponse>(device,
				new LifxPacket(MessageType.DeviceGetLabel));
			return resp?.Label ?? "";
		}


		/// <summary>
		///     Gets the label for the device
		/// </summary>
		/// <param name="device"></param>
		/// <returns>The device label</returns>
		public async Task<StateOwnerResponse?> GetDeviceOwnerAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.DeviceGetOwner);
			var resp = await BroadcastMessageAsync<StateOwnerResponse>(device, packet);
			return resp;
		}

		/// <summary>
		///     Gets the label for the device
		/// </summary>
		/// <param name="device"></param>
		/// <returns>The device label</returns>
		public async Task<StateWanResponse?> GetWanAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.WanGet) {Target = device.MacAddress};
			var resp = await BroadcastMessageAsync<StateWanResponse>(device, packet);
			//Debug.WriteLine("Response: " + resp.Payload);
			return resp;
		}

		/// <summary>
		///     Sets the label on the device
		/// </summary>
		/// <param name="device"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		public async Task SetDeviceLabelAsync(Device device, string label) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			await BroadcastMessageAsync<AcknowledgementResponse>(
				device, new LifxPacket(MessageType.DeviceSetLabel, label));
		}

		/// <summary>
		///     Gets the device version
		/// </summary>
		public Task<StateVersionResponse?> GetDeviceVersionAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			return BroadcastMessageAsync<StateVersionResponse>(device, new LifxPacket(MessageType.DeviceGetVersion));
		}

		/// <summary>
		///     Gets Host MCU firmware information.
		/// </summary>
		/// <param name="device"></param>
		/// <returns>
		///     <see cref="StateHostFirmwareResponse" />
		/// </returns>
		public Task<StateHostFirmwareResponse?> GetDeviceHostFirmwareAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			return BroadcastMessageAsync<StateHostFirmwareResponse>(device,
				new LifxPacket(MessageType.DeviceGetHostFirmware));
		}

		/// <summary>
		///     Get Host MCU information.
		/// </summary>
		/// <param name="device"></param>
		/// <returns>
		///     <see cref="StateHostInfoResponse" />
		/// </returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<StateHostInfoResponse?> GetHostInfoAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			return await BroadcastMessageAsync<StateHostInfoResponse>(device,
				new LifxPacket(MessageType.DeviceGetHostInfo));
		}

		/// <summary>
		///     Get Host Wifi information.
		/// </summary>
		/// <param name="device"></param>
		/// <returns>
		///     <see cref="StateWifiInfoResponse" />
		/// </returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<StateWifiInfoResponse?> GetWifiInfoAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			return await BroadcastMessageAsync<StateWifiInfoResponse>(device,
				new LifxPacket(MessageType.DeviceGetWifiInfo));
		}

		/// <summary>
		///     Get Host Wifi firmware information.
		/// </summary>
		/// <param name="device"></param>
		/// <returns>
		///     <see cref="StateWifiFirmwareResponse" />
		/// </returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<StateWifiFirmwareResponse?> GetWifiFirmwareAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			return await BroadcastMessageAsync<StateWifiFirmwareResponse>(device,
				new LifxPacket(MessageType.DeviceGetWifiFirmware));
		}

		/// <summary>
		///     Get device power level
		///     Zero implies standby and non-zero sets a corresponding power draw level. Currently only 0 and 65535 are supported.
		/// </summary>
		/// <param name="device"></param>
		/// <returns>0 for off, 1 for on</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<int> GetPowerAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var level = await BroadcastMessageAsync<StatePowerResponse>(device,
				new LifxPacket(MessageType.DeviceGetPower));
			return level?.Level == 0 ? 0 : 1;
		}

		/// <summary>
		///     Set Device power level.
		///     Internally, Lifx offers a range from 0-65535, but actually only responds to 0 and 65535.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="level">0 for off, 1 for on</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task SetPowerAsync(Device device, int level) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			if (level != 0) {
				level = 65535;
			}

			await BroadcastMessageAsync<AcknowledgementResponse>(device,
				new LifxPacket(MessageType.DeviceSetPower, level));
		}

		/// <summary>
		///     Get run-time information.
		/// </summary>
		/// <param name="device"></param>
		/// <returns>
		///     <see cref="StateInfoResponse" />
		/// </returns>
		/// <exception cref="ArrayTypeMismatchException"></exception>
		public async Task<StateInfoResponse?> GetInfoAsync(Device device) {
			if (device == null) {
				throw new ArrayTypeMismatchException(nameof(device));
			}

			return await BroadcastMessageAsync<StateInfoResponse>(device, new LifxPacket(MessageType.DeviceGetInfo));
		}

		/// <summary>
		///     Set the device location label
		/// </summary>
		/// <param name="device"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task SetLocationAsync(Device device, string label) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var rand = new Random();
			var location = new byte[16];
			rand.NextBytes(location);
			var updated = DateTimeOffset.Now.ToUnixTimeSeconds();
			var packet = new LifxPacket(MessageType.DeviceSetLocation, location, label, updated);
			await BroadcastMessageAsync<StatePowerResponse>(device, packet);
		}

		/// <summary>
		///     Ask the device to return its location information.
		/// </summary>
		/// <param name="device"></param>
		/// <returns>
		///     <see cref="StateLocationResponse" />
		/// </returns>
		/// <exception cref="ArrayTypeMismatchException"></exception>
		public async Task<StateLocationResponse?> GetLocationAsync(Device device) {
			if (device == null) {
				throw new ArrayTypeMismatchException(nameof(device));
			}

			return await BroadcastMessageAsync<StateLocationResponse>(device,
				new LifxPacket(MessageType.DeviceGetLocation));
		}

		/// <summary>
		///     Set the device group.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="label">The new group name</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task SetGroupAsync(Device device, string label) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var rand = new Random();
			var group = new byte[16];
			rand.NextBytes(group);
			var updated = DateTimeOffset.Now.ToUnixTimeSeconds();
			var packet = new LifxPacket(MessageType.DeviceSetGroup, group, label, updated);
			await BroadcastMessageAsync<StatePowerResponse>(device, packet);
		}

		/// <summary>
		///     Get the device group.
		/// </summary>
		/// <param name="device"></param>
		/// <returns>
		///     <see cref="StateGroupResponse" />
		/// </returns>
		/// <exception cref="ArrayTypeMismatchException"></exception>
		public async Task<StateGroupResponse?> GetGroupAsync(Device device) {
			if (device == null) {
				throw new ArrayTypeMismatchException(nameof(device));
			}

			return await BroadcastMessageAsync<StateGroupResponse>(device, new LifxPacket(MessageType.DeviceGetGroup));
		}

		/// <summary>
		///     Request an arbitrary payload be echoed back.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="payload"></param>
		/// <returns>
		///     <see cref="EchoResponse" />
		/// </returns>
		/// <exception cref="ArrayTypeMismatchException"></exception>
		public async Task<EchoResponse?> RequestEcho(Device device, byte[] payload) {
			if (device == null) {
				throw new ArrayTypeMismatchException(nameof(device));
			}

			// Truncate our input payload to be 64 bits exactly
			var realPayload = new byte[64];
			for (var i = 0; i < realPayload.Length; i++) {
				if (i < payload.Length) {
					realPayload[i] = payload[i];
				} else {
					realPayload[i] = 0;
				}
			}

			var packet = new LifxPacket(MessageType.DeviceEchoRequest, realPayload);
			return await BroadcastMessageAsync<EchoResponse>(device, packet);
		}
	}
}