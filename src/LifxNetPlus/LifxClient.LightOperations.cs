using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace LifxNetPlus {
	/// <summary>
	/// The lifx client class
	/// </summary>
	public partial class LifxClient {
		/// <summary>
		/// Waveform Types
		/// </summary>
		public enum WaveFormType : ushort {
			//Device Messages
			/// <summary>
			/// The saw wave form type
			/// </summary>
			Saw = 0, // Saw wave
			/// <summary>
			/// The sine wave form type
			/// </summary>
			Sine = 1, // Sine wave
			/// <summary>
			/// The half sine wave form type
			/// </summary>
			HalfSine = 2, // Half sine wave
			/// <summary>
			/// The triangle wave form type
			/// </summary>
			Triangle = 3, // Triangle
			/// <summary>
			/// The pulse wave form type
			/// </summary>
			Pulse = 4 // Pulse
		}

		/// <summary>
		/// The lifx response
		/// </summary>
		private readonly Dictionary<uint, Action<LifxResponse>> _taskCompletions =
			new Dictionary<uint, Action<LifxResponse>>();

		/// <summary>
		///     Turns a device on using the provided transition time
		/// </summary>
		/// <param name="device"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		/// <seealso cref="TurnDeviceOffAsync(LifxNetPlus.Device,int)" />
		/// <seealso cref="TurnDeviceOnAsync(Device)" />
		/// <seealso cref="TurnDeviceOffAsync(Device)" />
		/// <seealso cref="SetDevicePowerStateAsync(Device, bool)" />
		/// <seealso cref="GetLightPowerAsync(Device)" />
		public Task TurnDeviceOnAsync(Device device, int duration) {
			return SetLightPowerAsync(device, true, duration);
		}

		/// <summary>
		///     Turns a device off using the provided transition time
		/// </summary>
		/// <seealso cref="TurnDeviceOnAsync(LifxNetPlus.Device,int)" />
		/// <seealso cref="TurnDeviceOnAsync(Device)" />
		/// <seealso cref="TurnDeviceOffAsync(Device)" />
		/// <seealso cref="SetDevicePowerStateAsync(Device, bool)" />
		/// <seealso cref="GetLightPowerAsync(Device)" />
		public Task TurnDeviceOffAsync(Device device, int duration) {
			return SetLightPowerAsync(device, false, duration);
		}

		/// <summary>
		///     Turns a device on or off using the provided transition time
		/// </summary>
		/// <param name="device"></param>
		/// <param name="isOn">True to turn on, false to turn off</param>
		/// <param name="duration">Optional transition duration, in ms.</param>
		/// <param name="ackRequired">Whether or not to await a response</param>
		/// <returns></returns>
		/// <seealso cref="TurnDeviceOffAsync(LifxNetPlus.Device,int)" />
		/// <seealso cref="TurnDeviceOnAsync(LifxNetPlus.Device,int)" />
		/// <seealso cref="TurnDeviceOnAsync(Device)" />
		/// <seealso cref="TurnDeviceOffAsync(Device)" />
		/// <seealso cref="SetDevicePowerStateAsync(Device, bool)" />
		/// <seealso cref="GetLightPowerAsync(Device)" />
		public async Task SetLightPowerAsync(
			Device device,
			bool isOn,
			int duration = 0,
			bool ackRequired = false)
		{
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			if (duration > int.MaxValue ||
					duration < 0) {
				throw new ArgumentOutOfRangeException(nameof(duration));
			}


			var b = BitConverter.GetBytes((ushort) duration);

			var packet =
				new LifxPacket(MessageType.LightSetPower, (ushort) (isOn ? 65535 : 0), b) {ResponseRequired = ackRequired};
			if (ackRequired) {
				await BroadcastMessageAsync<AcknowledgementResponse>(device, packet);	
			} else {
				await BroadcastMessageAsync(device, packet);
			}
			
			
		}

		/// <summary>
		///     Gets the current power state for a light device
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public async Task<bool> GetLightPowerAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.LightGetPower) {ResponseRequired = true};
			var res = await BroadcastMessageAsync<LightPowerResponse>(
				device, packet);
			if (res == null) return false;
			return res.IsOn;
		}

		/// <summary>
		///     Sets color and temperature of device
		/// </summary>
		/// <param name="device">The device to set</param>
		/// <param name="color">The LifxColor to set the device to</param>
		/// <param name="duration">An optional transition duration, in milliseconds.</param>
		/// <param name="ackRequired">Whether or not to await a response</param>
		/// <returns></returns>
		public async Task<LightStateResponse?> SetColorAsync(Device device, LifxColor color, int duration = 0, bool ackRequired = false) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			if (duration > int.MaxValue || duration < 0) {
				throw new ArgumentOutOfRangeException(nameof(duration));
			}

			var dur = (uint) duration;
			var packet = new LifxPacket(MessageType.LightSetColor) {
				ResponseRequired = true, Payload = new Payload(new object[] {(byte) 0, color.ToBytes(), dur})
			};
			if (ackRequired) {
				return await BroadcastMessageAsync<LightStateResponse>(device, packet);	
			} 

			await BroadcastMessageAsync(device, packet);
			return null;
		}

		/// <summary>
		///     Sets color and temperature of device
		/// </summary>
		/// <param name="device">The device to set</param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// ///
		/// <param name="w"></param>
		/// <param name="duration">An optional transition duration, in milliseconds.</param>
		/// <returns>LightStateResponse</returns>
		public async Task<LightStateResponse?> SetRgbwAsync(
			Device device, 
			int r,
			int g,
			int b,
			int w = 0,
			int duration = 0)
		{
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			if (duration > int.MaxValue || duration < 0) {
				throw new ArgumentOutOfRangeException(nameof(duration));
			}

			var dur = (uint) duration;
			var packet = new LifxPacket(MessageType.LightSetRgbw) {
				ResponseRequired = true,
				Payload = new Payload(new object[] {(short) r, (short) g, (short) b, (short) w, dur})
			};
			return await BroadcastMessageAsync<LightStateResponse>(device, packet);
		}

		/// <summary>
		/// Set Light Waveform
		/// </summary>
		/// <param name="d">Device</param>
		/// <param name="transient">Set to TRUE to set the device back to it's original color after the effect if the waveform is SINE or triangle</param>
		/// <param name="color"></param>
		/// <param name="period">Length of the waveform in milliseconds</param>
		/// <param name="cycles">How many cycles of the effect to through, the duration of a cycle will last for period milliseconds</param>
		/// <param name="skewRatio"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<LightStateResponse?> SetWaveForm(
			Device d,
			bool transient,
			LifxColor color,
			uint period,
			float cycles,
			short skewRatio,
			WaveFormType type
			)
		{
			if (d == null) {
				throw new ArgumentNullException(nameof(d));
			}

			var packet = new LifxPacket(MessageType.LightSetWaveform) {
				ResponseRequired = true,
				Payload = new Payload(new object[] {
					(byte) 0, //reserved
					transient, color, period, cycles, skewRatio, type
				})
			};
			
			return await BroadcastMessageAsync<LightStateResponse>(d, packet);
		}

		/// <summary>
		/// Set the light waveform
		/// </summary>
		/// <param name="d"></param>
		/// <param name="transient"></param>
		/// <param name="color"></param>
		/// <param name="period"></param>
		/// <param name="cycles"></param>
		/// <param name="skewRatio"></param>
		/// <param name="type"></param>
		/// <param name="setHue"></param>
		/// <param name="setSat"></param>
		/// <param name="setBri"></param>
		/// <param name="setK"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<LightStateResponse?> SetWaveFormOptional(
			Device d,
			bool transient,
			LifxColor color,
			uint period,
			float cycles,
			short skewRatio,
			WaveFormType type,
			bool setHue,
			bool setSat,
			bool setBri,
			bool setK)
		{
			if (d == null) {
				throw new ArgumentNullException(nameof(d));
			}

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
		/// 	 Set the light to a warning state
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task SetWarning(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var warning_color = new LifxColor(
				color: Color.Yellow, 
				brightness: 0.3
			);

			var packet = new LifxPacket(MessageType.LightSetWaveform) {
				ResponseRequired = true,
				Payload = new Payload(new object[] {
					(byte) 0, //reserved
					true, warning_color, 1000, 10000F, 0, WaveFormType.Pulse
				})
			};

			await BroadcastMessageAsync<AcknowledgementResponse>(device, packet);
		}

		/// <summary>
		///     Set Light Brightness
		/// </summary>
		/// <param name="device"></param>
		/// <param name="brightness">0 - 255</param>
		/// <param name="duration"></param>
		/// <param name="ackRequired">Whether or not to await acknowledgement</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public async Task SetBrightnessAsync(
			Device device,
			ushort brightness,
			int duration = 0,
			bool ackRequired = false)
		{
			if (duration > int.MaxValue || duration < 0) {
				throw new ArgumentOutOfRangeException(nameof(duration));
			}

			var packet = new LifxPacket(MessageType.SetLightBrightness, brightness, duration);
			if (ackRequired) {
				await BroadcastMessageAsync<AcknowledgementResponse>(device, packet);	
			} else {
				await BroadcastMessageAsync(device, packet);
			}
		}

		/// <summary>
		///     Gets the current state of the device
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public async Task<LightStateResponse?> GetLightStateAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			return await BroadcastMessageAsync<LightStateResponse>(device, new LifxPacket(MessageType.LightGet));
		}


		/// <summary>
		///     Gets the current maximum power level of the Infrared channel
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public async Task<ushort> GetInfraredAsync(Device device) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.LightGetInfrared);
			var res = await BroadcastMessageAsync<InfraredStateResponse>(
				device, packet);
			if (res == null) return 0;
			return res.Brightness;
		}

		/// <summary>
		///     Sets the infrared brightness level
		/// </summary>
		/// <param name="device"></param>
		/// <param name="brightness"></param>
		/// <returns></returns>
		public async Task SetInfraredAsync(Device device, ushort brightness) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.LightSetInfrared, brightness);
			await BroadcastMessageAsync<AcknowledgementResponse>(device, packet);
		}
	}
}