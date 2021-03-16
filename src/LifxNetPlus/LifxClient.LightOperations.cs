using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LifxNetPlus {
	public partial class LifxClient {
		private readonly Dictionary<uint, Action<LifxResponse>> _taskCompletions =
			new Dictionary<uint, Action<LifxResponse>>();

		/// <summary>
		/// Turns a bulb on using the provided transition time
		/// </summary>
		/// <param name="bulb"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		/// <seealso cref="TurnBulbOffAsync(LightBulb, int)"/>
		/// <seealso cref="TurnDeviceOnAsync(Device)"/>
		/// <seealso cref="TurnDeviceOffAsync(Device)"/>
		/// <seealso cref="SetLightPowerAsync(LightBulb, bool, int)"/>
		/// <seealso cref="SetDevicePowerStateAsync(Device, bool)"/>
		/// <seealso cref="GetLightPowerAsync(LightBulb)"/>
		public Task TurnBulbOnAsync(LightBulb bulb, int duration) =>
			SetLightPowerAsync(bulb, true, duration);

		/// <summary>
		/// Turns a bulb off using the provided transition time
		/// </summary>
		/// <seealso cref="TurnBulbOnAsync(LightBulb, int)"/>
		/// <seealso cref="TurnDeviceOnAsync(Device)"/>
		/// <seealso cref="TurnDeviceOffAsync(Device)"/>
		/// <seealso cref="SetLightPowerAsync(LightBulb, bool, int)"/>
		/// <seealso cref="SetDevicePowerStateAsync(Device, bool)"/>
		/// <seealso cref="GetLightPowerAsync(LightBulb)"/>
		public Task TurnBulbOffAsync(LightBulb bulb, int duration) =>
			SetLightPowerAsync(bulb, false, duration);

		/// <summary>
		/// Turns a bulb on or off using the provided transition time
		/// </summary>
		/// <param name="bulb"></param>
		/// <param name="isOn">True to turn on, false to turn off</param>
		/// <param name="duration">Optional transition duration, in ms.</param>
		/// <returns></returns>
		/// <seealso cref="TurnBulbOffAsync(LightBulb, int)"/>
		/// <seealso cref="TurnBulbOnAsync(LightBulb, int)"/>
		/// <seealso cref="TurnDeviceOnAsync(Device)"/>
		/// <seealso cref="TurnDeviceOffAsync(Device)"/>
		/// <seealso cref="SetDevicePowerStateAsync(Device, bool)"/>
		/// <seealso cref="GetLightPowerAsync(LightBulb)"/>
		public async Task SetLightPowerAsync(LightBulb bulb, bool isOn, int duration = 0) {
			if (bulb == null)
				throw new ArgumentNullException(nameof(bulb));
			if (duration > uint.MaxValue ||
			    duration < 0)
				throw new ArgumentOutOfRangeException(nameof(duration));


			var b = BitConverter.GetBytes((ushort) duration);

			Debug.WriteLine(
				$"Sending LightSetPower(on={isOn},duration={duration}ms) to {bulb}");
			var packet = new LifxPacket(MessageType.LightSetPower, (ushort) (isOn ? 65535 : 0), b);
			await BroadcastMessageAsync<AcknowledgementResponse>(bulb, packet).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the current power state for a light bulb
		/// </summary>
		/// <param name="bulb"></param>
		/// <returns></returns>
		public async Task<bool> GetLightPowerAsync(LightBulb bulb) {
			if (bulb == null)
				throw new ArgumentNullException(nameof(bulb));

			var packet = new LifxPacket(MessageType.LightGetPower);
			return (await BroadcastMessageAsync<LightPowerResponse>(
				bulb, packet).ConfigureAwait(false)).IsOn;
		}

		/// <summary>
		/// Sets color and temperature of bulb
		/// </summary>
		/// <param name="bulb">The bulb to set</param>
		/// <param name="color">The LifxColor to set the bulb to</param>
		/// <param name="duration">An optional transition duration, in milliseconds.</param>
		/// <returns></returns>
		public Task SetColorAsync(LightBulb bulb, LifxColor color, int duration = 0) {
			return SetColorAsync(bulb, (ushort) color.LifxHue, (ushort) color.LifxSaturation,
				(ushort) color.LifxBrightness, (ushort) color.K, duration);
		}


		/// <summary>
		/// Sets color and temperature for a bulb and uses a transition time to the provided state
		/// </summary>
		/// <param name="bulb">Light bulb</param>
		/// <param name="hue">0..65535</param>
		/// <param name="saturation">0..65535</param>
		/// <param name="brightness">0..65535</param>
		/// <param name="kelvin">2700..9000</param>
		/// <param name="duration">Transition duration, in ms.</param>
		/// <returns></returns>
		public async Task SetColorAsync(LightBulb bulb,
			ushort hue,
			ushort saturation,
			ushort brightness,
			ushort kelvin,
			int duration = 0) {
			if (bulb == null)
				throw new ArgumentNullException(nameof(bulb));
			if (duration > uint.MaxValue ||
			    duration < 0)
				throw new ArgumentOutOfRangeException("transitionDuration");
			if (kelvin < 2500 || kelvin > 9000) {
				throw new ArgumentOutOfRangeException("kelvin", "Kelvin must be between 2500 and 9000");
			}

			Debug.WriteLine("Setting color to {0}", bulb);
			var packet = new LifxPacket(MessageType.LightSetColor);
			packet.Payload = new Payload(new object[] {0x00, hue, saturation, brightness, kelvin, duration});
			await BroadcastMessageAsync<AcknowledgementResponse>(bulb, packet);
		}


		public async Task SetBrightnessAsync(LightBulb bulb,
			ushort brightness,
			int duration = 0) {
			if (duration > UInt32.MaxValue ||
			    duration < 0)
				throw new ArgumentOutOfRangeException(nameof(duration));

			var packet = new LifxPacket(MessageType.SetLightBrightness, brightness, duration);
			await BroadcastMessageAsync<AcknowledgementResponse>(bulb, packet);
		}

		/// <summary>
		/// Gets the current state of the bulb
		/// </summary>
		/// <param name="bulb"></param>
		/// <returns></returns>
		public async Task<LightStateResponse> GetLightStateAsync(LightBulb bulb) {
			if (bulb == null) throw new ArgumentNullException(nameof(bulb));
			return await BroadcastMessageAsync<LightStateResponse>(bulb, new LifxPacket(MessageType.LightGet));
		}


		/// <summary>
		/// Gets the current maximum power level of the Infrared channel
		/// </summary>
		/// <param name="bulb"></param>
		/// <returns></returns>
		public async Task<ushort> GetInfraredAsync(LightBulb bulb) {
			if (bulb == null) throw new ArgumentNullException(nameof(bulb));
			var packet = new LifxPacket(MessageType.LightGetInfrared);
			return (await BroadcastMessageAsync<InfraredStateResponse>(
				bulb, packet).ConfigureAwait(false)).Brightness;
		}

		/// <summary>
		/// Sets the infrared brightness level
		/// </summary>
		/// <param name="device"></param>
		/// <param name="brightness"></param>
		/// <returns></returns>
		public async Task SetInfraredAsync(Device device, ushort brightness) {
			if (device == null) throw new ArgumentNullException(nameof(device));
			var packet = new LifxPacket(MessageType.LightSetInfrared, brightness);
			await BroadcastMessageAsync<AcknowledgementResponse>(device, packet).ConfigureAwait(false);
		}
	}
}