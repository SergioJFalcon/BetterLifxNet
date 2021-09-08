using System;
using System.Threading.Tasks;

namespace LifxNetPlus {
	/// <summary>
	/// The lifx client class
	/// </summary>
	public partial class LifxClient {
		/// <summary>
		///     Get the power state of a relay on a switch device.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="relayIndex">The relay on the switch starting from 0</param>
		/// <returns>A StateRelayPower message.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<StateRelayPowerResponse?> GetRelayPowerAsync(Device device, int relayIndex = 0) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.GetRelayPower, (byte) relayIndex);
			return await BroadcastMessageAsync<StateRelayPowerResponse>(
				device, packet);
		}

		/// <summary>
		///     Set the power state of a relay on a switch device.
		///     Current models of the LIFX switch do not have dimming capability,
		///     so the two valid values are 0 for off and 65535 for on.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="relayIndex">The relay on the switch starting from 0</param>
		/// <param name="enable">Whether to turn the device on or not.</param>
		/// <returns>A StateRelayPower message.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<StateRelayPowerResponse?> SetRelayPowerAsync(Device device, int relayIndex, bool enable) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var level = enable ? 65535 : 0;

			var packet = new LifxPacket(MessageType.SetRelayPower, (byte) relayIndex, (ushort) level);
			return await BroadcastMessageAsync<StateRelayPowerResponse>(device, packet);
		}
	}
}