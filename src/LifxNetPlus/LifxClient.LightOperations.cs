using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LifxNetPlus {
	public partial class LifxClient {
		
		public enum WaveFormType : ushort {
			//Device Messages
			Saw = 0,
			Sine = 1,
			HalfSine = 2,
			Triangle = 3,
			Pulse = 4
		}

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

			var packet = new LifxPacket(MessageType.LightSetPower, (ushort) (isOn ? 65535 : 0), b);
			packet.ResponseRequired = true;
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
			packet.ResponseRequired = true;
			return (await BroadcastMessageAsync<LightPowerResponse>(
				bulb, packet)).IsOn;
		}

		/// <summary>
		/// Sets color and temperature of bulb
		/// </summary>
		/// <param name="bulb">The bulb to set</param>
		/// <param name="color">The LifxColor to set the bulb to</param>
		/// <param name="duration">An optional transition duration, in milliseconds.</param>
		/// <returns></returns>
		public async Task<LightStateResponse> SetColorAsync(LightBulb bulb, LifxColor color, int duration = 0) {
			if (bulb == null) throw new ArgumentNullException(nameof(bulb));
			if (duration > uint.MaxValue || duration < 0) throw new ArgumentOutOfRangeException(nameof(duration));
			var dur = (uint) duration;
			var packet = new LifxPacket(MessageType.LightSetColor);
			packet.ResponseRequired = true;
			packet.Payload = new Payload(new object[] {(byte) 0, color.ToBytes(), dur});
			return await BroadcastMessageAsync<LightStateResponse>(bulb, packet);
		}

		/// <summary>
		/// Sets color and temperature of bulb
		/// </summary>
		/// <param name="bulb">The bulb to set</param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// /// <param name="w"></param>
		/// <param name="duration">An optional transition duration, in milliseconds.</param>
		/// <returns>LightStateResponse</returns>
		public async Task<LightStateResponse> SetRgbwAsync(LightBulb bulb, int r, int g, int b, int w = 0, int duration = 0) {
			if (bulb == null) throw new ArgumentNullException(nameof(bulb));
			if (duration > uint.MaxValue || duration < 0) throw new ArgumentOutOfRangeException(nameof(duration));
			var dur = (uint) duration;
			var packet = new LifxPacket(MessageType.LightSetRgbw);
			packet.ResponseRequired = true;
			packet.Payload = new Payload(new object[] {(short) r,(short) g,(short) b,(short) w, dur});
			return await BroadcastMessageAsync<LightStateResponse>(bulb, packet);
		}

		public async Task<LightStateResponse> SetWaveForm(Device d, bool transient, LifxColor color, uint period,
			float cycles, short skewRatio, WaveFormType type) {
			if (d == null) throw new ArgumentNullException(nameof(d));
			var packet = new LifxPacket(MessageType.LightSetWaveform) {
				ResponseRequired = true,
				Payload = new Payload(new object[] {
					(byte) 0, //reserved
					transient, color, period, cycles, skewRatio, type
				})
			};
			return await BroadcastMessageAsync<LightStateResponse>(d, packet);
		}
		
		public async Task<LightStateResponse> SetWaveFormOptional(Device d, bool transient, LifxColor color, uint period,
			float cycles, short skewRatio, WaveFormType type, bool setHue, bool setSat, bool setBri, bool setK) {
			if (d == null) throw new ArgumentNullException(nameof(d));
			var packet = new LifxPacket(MessageType.LightSetWaveform) {
				ResponseRequired = true,
				Payload = new Payload(new object[] {
					(byte) 0, //reserved
					transient, color, period, cycles, skewRatio, type, setHue, setSat, setBri, setK
				})
			};
			return await BroadcastMessageAsync<LightStateResponse>(d, packet);
		}


		
		/// <summary>
		/// Set Light Brightness
		/// </summary>
		/// <param name="bulb"></param>
		/// <param name="brightness">0 - 255</param>
		/// <param name="duration"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
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