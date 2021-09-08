using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LifxNetPlus {
	/// <summary>
	///     Base class for LIFX response types
	/// </summary>
	public abstract class LifxResponse : LifxPacket {
		/// <summary>
		/// Gets the value of the packet
		/// </summary>
		internal LifxPacket Packet { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LifxResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal LifxResponse(LifxPacket packet) : base(packet) {
			Packet = packet;
		}

		/// <summary>
		/// Creates the packet
		/// </summary>
		/// <param name="packet">The packet</param>
		/// <returns>The lifx response</returns>
		internal static LifxResponse Create(LifxPacket packet) {
			return packet.Type switch {
				MessageType.DeviceAcknowledgement => new AcknowledgementResponse(packet),
				MessageType.DeviceStateLabel => new StateLabelResponse(packet),
				MessageType.LightState => new LightStateResponse(packet),
				MessageType.LightSetColor => new LightStateResponse(packet),
				MessageType.LightStatePower => new LightPowerResponse(packet),
				MessageType.LightStateInfrared => new InfraredStateResponse(packet),
				MessageType.DeviceStateVersion => new StateVersionResponse(packet),
				MessageType.DeviceStateHostFirmware => new StateHostFirmwareResponse(packet),
				MessageType.DeviceStateService => new StateServiceResponse(packet),
				MessageType.StateExtendedColorZones => new StateExtendedColorZonesResponse(packet),
				MessageType.SetExtendedColorZones => new SetExtendedColorZonesResponse(packet),
				MessageType.StateZone => new StateZoneResponse(packet),
				MessageType.StateMultiZone => new StateMultiZoneResponse(packet),
				MessageType.StateDeviceChain => new StateDeviceChainResponse(packet),
				MessageType.StateTileState16 => new StateTileState16Response(packet),
				MessageType.StateTileState64 => new StateTileState64Response(packet),
				MessageType.StateRelayPower => new StateRelayPowerResponse(packet),
				MessageType.DeviceStateHostInfo => new StateHostInfoResponse(packet),
				MessageType.DeviceStateWifiInfo => new StateWifiInfoResponse(packet),
				MessageType.DeviceStateWifiFirmware => new StateWifiFirmwareResponse(packet),
				MessageType.DeviceStatePower => new StatePowerResponse(packet),
				MessageType.DeviceStateInfo => new StateInfoResponse(packet),
				MessageType.DeviceStateLocation => new StateLocationResponse(packet),
				MessageType.DeviceStateGroup => new StateGroupResponse(packet),
				MessageType.DeviceEchoResponse => new EchoResponse(packet),
				MessageType.DeviceStateOwner => new StateOwnerResponse(packet),
				MessageType.WanState => new StateWanResponse(packet),
				MessageType.StateTileTapConfig => new StateTileTapConfigResponse(packet),
				MessageType.TileStateEffect => new StateTileEffectResponse(packet),
				_ => new UnknownResponse(packet)
			};
		}
	}

	/// <summary>
	///     Response to any message sent with ack_required set to 1.
	/// </summary>
	internal class AcknowledgementResponse : LifxResponse {
		/// <summary>
		/// Initializes a new instance of the <see cref="AcknowledgementResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal AcknowledgementResponse(LifxPacket packet) : base(
			packet) {
		}
	}

	/// <summary>
	///     State Tile Tap Config
	/// </summary>
	internal class StateTileTapConfigResponse : LifxResponse {
		/// <summary>
		/// Initializes a new instance of the <see cref="StateTileTapConfigResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateTileTapConfigResponse(LifxPacket packet) : base(
			packet) {
		}
	}

	/// <summary>
	///     Response to a state tile get request
	/// </summary>
	public class StateTileEffectResponse : LifxResponse {
		/// <summary>
		/// Initializes a new instance of the <see cref="StateTileEffectResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateTileEffectResponse(LifxPacket packet) : base(
			packet) {
		}
	}

	/// <summary>
	///     The StateZone message represents the state of a single zone with the index field indicating which zone is
	///     represented. The count field contains the count of the total number of zones available on the device.
	/// </summary>
	public class StateZoneResponse : LifxResponse {
		/// <summary>
		///     The list of colors returned by the message
		/// </summary>
		public LifxColor Color { get; }

		/// <summary>
		///     Count - total number of zones on the device
		/// </summary>
		public ushort Count { get; }

		/// <summary>
		///     Index - Zone the message starts from
		/// </summary>
		public ushort Index { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateZoneResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateZoneResponse(LifxPacket packet) : base(packet) {
			Count = packet.Payload.GetUInt16();
			Index = packet.Payload.GetUInt16();
			Color = new Payload().GetColor();
		}
	}


	/// <summary>
	///     Response to GetHostInfo message.
	///     Provides host MCU information.
	/// </summary>
	public class StateHostInfoResponse : LifxResponse {
		/// <summary>
		///     Radio receive signal strength in milliWatts
		/// </summary>
		public float Signal { get; set; }

		/// <summary>
		///     Bytes received since power on
		/// </summary>
		public uint Rx { get; set; }

		/// <summary>
		///     Bytes transmitted since power on
		/// </summary>
		public uint Tx { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateHostInfoResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateHostInfoResponse(LifxPacket packet) : base(packet) {
			Signal = packet.Payload.GetFloat32();
			Tx = packet.Payload.GetUInt32();
			Rx = packet.Payload.GetUInt32();
		}
	}


	/// <summary>
	///     Response to GetWifiInfo message.
	///     Provides host Wifi information.
	/// </summary>
	public class StateWifiInfoResponse : LifxResponse {
		/// <summary>
		///     Radio receive signal strength in milliWatts
		/// </summary>
		public float Signal { get; set; }

		/// <summary>
		///     Bytes received since power on
		/// </summary>
		public uint Rx { get; set; }

		/// <summary>
		///     Bytes transmitted since power on
		/// </summary>
		public uint Tx { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateWifiInfoResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateWifiInfoResponse(LifxPacket packet) : base(packet) {
			Signal = packet.Payload.GetFloat32();
			Tx = packet.Payload.GetUInt32();
			Rx = packet.Payload.GetUInt32();
		}
	}

	/// <summary>
	///     Response to GetWifiFirmware message.
	///     Provides Wifi subsystem information.
	/// </summary>
	public class StateWifiFirmwareResponse : LifxResponse {
		/// <summary>
		///     Firmware build time (epoch time)
		/// </summary>
		public ulong Build { get; set; }

		/// <summary>
		///     Major firmware version number
		/// </summary>
		public ushort VersionMajor { get; set; }

		/// <summary>
		///     Minor firmware version number
		/// </summary>
		public ushort VersionMinor { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateWifiFirmwareResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateWifiFirmwareResponse(LifxPacket packet) : base(packet) {
			Build = packet.Payload.GetUInt64();
			// Skip 64-bit reserved
			packet.Payload.Advance(8);
			VersionMinor = packet.Payload.GetUInt16();
			VersionMajor = packet.Payload.GetUInt16();
		}
	}


	/// <summary>
	///     Provides device power level.
	/// </summary>
	public class StatePowerResponse : LifxResponse {
		/// <summary>
		///     Zero implies standby and non-zero sets a corresponding power draw level. Currently only 0 and 65535 are supported.
		/// </summary>
		public ulong Level { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StatePowerResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StatePowerResponse(LifxPacket packet) : base(packet) {
			Level = packet.Payload.GetUInt16();
		}
	}


	/// <summary>
	///     Provides run-time information of device.
	/// </summary>
	public class StateInfoResponse : LifxResponse {
		/// <summary>
		///     Current time
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		///     Last power off period, 5 second accuracy (in nanoseconds)
		/// </summary>
		public long Downtime { get; set; }

		/// <summary>
		///     Time since last power on (relative time in nanoseconds)
		/// </summary>
		public long Uptime { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateInfoResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateInfoResponse(LifxPacket packet) : base(packet) {
			Time = DateTimeOffset.FromUnixTimeSeconds(packet.Payload.GetInt64()).DateTime;
			Uptime = packet.Payload.GetInt64();
			Downtime = packet.Payload.GetInt64();
		}
	}


	/// <summary>
	/// Location Response
	/// </summary>
	public class StateLocationResponse : LifxResponse {
		/// <summary>
		/// The location
		/// </summary>
		public byte[] Location { get; set; }
		/// <summary>
		/// Device Label
		/// </summary>
		public string Label { get; set; }
		/// <summary>
		/// Last Updated
		/// </summary>

		public ulong Updated { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateLocationResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateLocationResponse(LifxPacket packet) : base(packet) {
			Location = packet.Payload.GetBytes(16);
			Label = packet.Payload.GetString(32);
			Updated = packet.Payload.GetUInt64();
		}
	}

	/// <summary>
	///     Device group.
	/// </summary>
	public class StateGroupResponse : LifxResponse {
		/// <summary>
		/// Device Group
		/// </summary>
		public byte[] Group { get; set; }
		/// <summary>
		/// Group Label
		/// </summary>

		public string Label { get; set; }
		/// <summary>
		/// Last Updated
		/// </summary>

		public ulong Updated { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateGroupResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateGroupResponse(LifxPacket packet) : base(packet) {
			Group = packet.Payload.GetBytes(16);
			Label = packet.Payload.GetString(32);
			Updated = packet.Payload.GetUInt64();
		}
	}

	/// <summary>
	///     Echo response with payload sent in the EchoRequest.
	/// </summary>
	public class EchoResponse : LifxResponse {
		/// <summary>
		///     Payload sent in the EchoRequest.
		/// </summary>
		public byte[] RequestPayload { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="EchoResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal EchoResponse(LifxPacket packet) : base(packet) {
			RequestPayload = packet.Payload.ToArray();
		}
	}


	/// <summary>
	///     The StateZone message represents the state of a single zone with the index field indicating which zone is
	///     represented. The count field contains the count of the total number of zones available on the device.
	/// </summary>
	public class StateDeviceChainResponse : LifxResponse {
		/// <summary>
		///     Start Index - Zone the message starts from
		/// </summary>
		public byte StartIndex { get; }

		/// <summary>
		///     Count - total number of zones on the device
		/// </summary>
		public int TotalCount { get; }

		/// <summary>
		///     The list of colors returned by the message
		/// </summary>
		public List<Tile> Tiles { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateDeviceChainResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateDeviceChainResponse(LifxPacket packet) : base(
			packet) {
			Tiles = new List<Tile>();
			StartIndex = packet.Payload.GetUint8();
			for (var i = 0; i < 16; i++) {
				var tile = new Tile();
				tile.LoadBytes(packet.Payload);
				Tiles.Add(tile);
			}

			TotalCount = packet.Payload.GetUint8();
		}
	}

	/// <summary>
	///     Get the list of colors currently being displayed by zones
	/// </summary>
	public class StateMultiZoneResponse : LifxResponse {
		/// <summary>
		///     The list of colors returned by the message
		/// </summary>
		[JsonProperty]
		public LifxColor[] Colors { get; }

		/// <summary>
		///     Count - total number of zones on the device
		/// </summary>

		[JsonProperty]
		public ushort Count { get; }

		/// <summary>
		///     Index - Zone the message starts from
		/// </summary>
		[JsonProperty]
		public ushort Index { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateMultiZoneResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateMultiZoneResponse(LifxPacket packet) : base(
			packet) {
			Colors = new LifxColor[8];
			Count = packet.Payload.GetUint8();
			Index = packet.Payload.GetUint8();
			for (var i = 0; i < 8; i++) {
				Colors[i] = packet.Payload.GetColor();
			}
		}
	}


	/// <summary>
	///     Get the list of colors currently being displayed by zones
	/// </summary>
	public class StateExtendedColorZonesResponse : LifxResponse {
		/// <summary>
		///     The list of colors returned by the message
		/// </summary>
		[JsonProperty]
		public List<LifxColor> Colors { get; private set; }

		/// <summary>
		///     Number of colors
		/// </summary>
		[JsonProperty]
		public ushort ColorsCount { get; private set; }

		/// <summary>
		///     Index - Zone the message starts from
		/// </summary>
		[JsonProperty]
		public ushort Index { get; private set; }

		/// <summary>
		///     Count - total number of zones on the device
		/// </summary>
		[JsonProperty]
		public ushort ZonesCount { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateExtendedColorZonesResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateExtendedColorZonesResponse(LifxPacket packet) :
			base(packet) {
			Colors = new List<LifxColor>();
			ZonesCount = packet.Payload.GetUInt16();
			Index = packet.Payload.GetUInt16();
			ColorsCount = packet.Payload.GetUint8();
			for (var i = 0; i < ColorsCount; i++) {
				Colors.Add(packet.Payload.GetColor());
			}
		}
	}

	/// <summary>
	///     Get the list of colors currently being displayed by zones
	/// </summary>
	public class SetExtendedColorZonesResponse : LifxResponse {
		/// <summary>
		///     The list of colors returned by the message
		/// </summary>
		[JsonProperty]
		public List<LifxColor> Colors { get; private set; }

		/// <summary>
		///     Should the effect be applied?
		///     0 - No
		///     1 - Apply
		///     2 - ApplyOnly
		/// </summary>
		public uint Apply { get; }

		/// <summary>
		/// Gets or sets the value of the color count
		/// </summary>
		public uint ColorCount { get; set; }

		/// <summary>
		///     How long to transition to new color
		/// </summary>
		[JsonProperty]
		public uint Duration { get; private set; }

		/// <summary>
		///     Count - total number of zones on the device
		/// </summary>
		[JsonProperty]
		public ushort ZoneIndex { get; private set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="SetExtendedColorZonesResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal SetExtendedColorZonesResponse(LifxPacket packet) :
			base(packet) {
			Colors = new List<LifxColor>();
			Duration = packet.Payload.GetUInt32();
			Apply = packet.Payload.GetUint8();
			ZoneIndex = packet.Payload.GetUInt16();
			ColorCount = packet.Payload.GetUint8();
			for (var i = 0; i < ColorCount; i++) {
				Colors.Add(packet.Payload.GetColor());
			}
		}
	}

	/// <summary>
	///     Response to GetService message.
	///     Provides the device Service and port.
	///     If the Service is temporarily unavailable, then the port value will be 0.
	/// </summary>
	[Serializable]
	public class StateServiceResponse : LifxResponse {
		/// <summary>
		/// Gets the value of the service
		/// </summary>
		[JsonProperty] public byte Service { get; }
		/// <summary>
		/// Gets the value of the port
		/// </summary>
		[JsonProperty] public uint Port { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateServiceResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		public StateServiceResponse(LifxPacket packet) : base(
			packet) {
			Service = packet.Payload.GetUint8();
			Port = packet.Payload.GetUInt32();
		}
	}

	/// <summary>
	///     Response to any message sent with ack_required set to 1.
	/// </summary>
	public class StateTileState16Response : LifxResponse {
		/// <summary>
		/// Gets the value of the colors
		/// </summary>
		public LifxColor[] Colors { get; }

		/// <summary>
		/// Gets the value of the tile index
		/// </summary>
		public uint TileIndex { get; }
		/// <summary>
		/// Gets the value of the width
		/// </summary>
		public uint Width { get; }
		/// <summary>
		/// Gets the value of the x
		/// </summary>
		public uint X { get; }
		/// <summary>
		/// Gets the value of the y
		/// </summary>
		public uint Y { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateTileState16Response"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateTileState16Response(LifxPacket packet) : base(
			packet) {
			TileIndex = packet.Payload.GetUint8();
			// Skip one byte for reserved
			packet.Payload.Advance();
			X = packet.Payload.GetUint8();
			Y = packet.Payload.GetUint8();
			Width = packet.Payload.GetUint8();
			Colors = new LifxColor[16];
			for (var i = 0; i < Colors.Length; i++) {
				if (packet.Payload.HasContent()) {
					Colors[i] = packet.Payload.GetColor();
				}
			}
		}
	}

	/// <summary>
	///     Response to any message sent with ack_required set to 1.
	/// </summary>
	public class StateTileState64Response : LifxResponse {
		/// <summary>
		/// Gets the value of the colors
		/// </summary>
		public LifxColor[] Colors { get; }

		/// <summary>
		/// Gets the value of the tile index
		/// </summary>
		public uint TileIndex { get; }
		/// <summary>
		/// Gets the value of the width
		/// </summary>
		public uint Width { get; }
		/// <summary>
		/// Gets the value of the x
		/// </summary>
		public uint X { get; }
		/// <summary>
		/// Gets the value of the y
		/// </summary>
		public uint Y { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateTileState64Response"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateTileState64Response(LifxPacket packet) : base(
			packet) {
			TileIndex = packet.Payload.GetUint8();
			// Skip one byte for reserved
			packet.Payload.Advance();
			X = packet.Payload.GetUint8();
			Y = packet.Payload.GetUint8();
			Width = packet.Payload.GetUint8();
			Colors = new LifxColor[64];
			for (var i = 0; i < Colors.Length; i++) {
				if (packet.Payload.HasContent()) {
					Colors[i] = packet.Payload.GetColor();
				}
			}
		}
	}

	/// <summary>
	///     Response to GetLabel message. Provides device label.
	/// </summary>
	internal class StateLabelResponse : LifxResponse {
		/// <summary>
		/// Gets the value of the label
		/// </summary>
		public string? Label { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateLabelResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateLabelResponse(LifxPacket packet) : base(packet) {
			Label = packet.Payload.GetString(32).Replace("\0", "");
		}
	}

	/// <summary>
	///     Response to GetLabel message. Provides device label.
	/// </summary>
	public class StateOwnerResponse : LifxResponse {
		/// <summary>
		/// Gets the value of the label
		/// </summary>
		public string Label { get; }

		/// <summary>
		/// Gets the value of the owner
		/// </summary>
		public string Owner { get; }
		/// <summary>
		/// Gets the value of the updated
		/// </summary>
		public ulong Updated { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateOwnerResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateOwnerResponse(LifxPacket packet) : base(packet) {
			packet.Payload.Advance(2); // Reserved 1
			Owner = packet.Payload.GetString(16);
			Label = packet.Payload.GetString(16);
			packet.Payload.Advance(2); // Reserved 2
			Updated = packet.Payload.GetUInt64();
		}
	}

	/// <summary>
	/// Response to GetWAN message.
	/// </summary>
	
	public class StateWanResponse : LifxResponse {
		/// <summary>
		/// Gets the value of the state
		/// </summary>
		public byte State { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateWanResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateWanResponse(LifxPacket packet) : base(packet) {
			State = packet.Payload.GetUint8();
		}
	}

	/// <summary>
	///     Sent by a device to provide the current light state
	/// </summary>
	public class LightStateResponse : LifxResponse {
		/// <summary>
		///     Power state
		/// </summary>
		public bool IsOn { get; }

		/// <summary>
		///     Light label
		/// </summary>
		public string Label { get; }

		/// <summary>
		///     Brightness (0=off, 65535=full brightness)
		/// </summary>
		public ushort Brightness { get; }

		/// <summary>
		///     Hue
		/// </summary>
		public ushort Hue { get; }

		/// <summary>
		///     Bulb color temperature
		/// </summary>
		public ushort Kelvin { get; }

		/// <summary>
		///     Saturation (0=desaturated, 65535 = fully saturated)
		/// </summary>
		public ushort Saturation { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LightStateResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal LightStateResponse(LifxPacket packet) : base(packet) {
			Hue = packet.Payload.GetUInt16();
			Saturation = packet.Payload.GetUInt16();
			Brightness = packet.Payload.GetUInt16();
			Kelvin = packet.Payload.GetUInt16();
			IsOn = packet.Payload.GetUInt16() > 0;
			Label = packet.Payload.GetString(32).Replace("\\0", "");
		}
	}

	/// <summary>
	/// The light power response class
	/// </summary>
	/// <seealso cref="LifxResponse"/>
	internal class LightPowerResponse : LifxResponse {
		/// <summary>
		/// Gets the value of the is on
		/// </summary>
		public bool IsOn { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LightPowerResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal LightPowerResponse(LifxPacket packet) : base(packet) {
			//var bString = string.Join(",", (from a in packet.Payload.ToArray() select a.ToString("X2")).ToArray());
			//Debug.WriteLine("Power payload: " + bString);
			IsOn = packet.Payload.GetUInt16() > 0;
		}
	}

	/// <summary>
	/// The infrared state response class
	/// </summary>
	/// <seealso cref="LifxResponse"/>
	internal class InfraredStateResponse : LifxResponse {
		/// <summary>
		/// Gets the value of the brightness
		/// </summary>
		public ushort Brightness { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="InfraredStateResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal InfraredStateResponse(LifxPacket packet) : base(
			packet) {
			Brightness = packet.Payload.GetUInt16();
		}
	}

	/// <summary>
	///     Response to GetVersion message.	Provides the hardware version of the device.
	/// </summary>
	public class StateVersionResponse : LifxResponse {
		/// <summary>
		///     Product ID
		/// </summary>
		public uint Product { get; }

		/// <summary>
		///     Vendor ID
		/// </summary>
		public uint Vendor { get; }

		/// <summary>
		///     Hardware version
		/// </summary>
		public uint Version { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateVersionResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateVersionResponse(LifxPacket packet) : base(
			packet) {
			Vendor = packet.Payload.GetUInt32();
			Product = packet.Payload.GetUInt32();
			Version = packet.Payload.GetUInt32();
		}
	}

	/// <summary>
	///     Response to GetHostFirmware message. Provides host firmware information.
	/// </summary>
	public class StateHostFirmwareResponse : LifxResponse {
		/// <summary>
		///     Firmware build time
		/// </summary>
		public DateTime Build { get; }

		/// <summary>
		///     Firmware version
		/// </summary>
		public uint Version { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateHostFirmwareResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateHostFirmwareResponse(LifxPacket packet) : base(
			packet) {
			var nanoseconds = packet.Payload.GetUInt64();
			Build = Utilities.Epoch.AddMilliseconds(nanoseconds * 0.000001);
			//8..15 UInt64 is reserved
			Version = packet.Payload.GetUInt32();
		}
	}

	/// <summary>
	///     Response to GetVersion message.	Provides the hardware version of the device.
	/// </summary>
	public class StateRelayPowerResponse : LifxResponse {
		/// <summary>
		///     The value of the relay
		/// </summary>
		public int Level { get; }

		/// <summary>
		///     The relay on the switch starting from 0
		/// </summary>
		public int RelayIndex { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateRelayPowerResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal StateRelayPowerResponse(LifxPacket packet) : base(
			packet) {
			RelayIndex = packet.Payload.GetUint8();
			Level = packet.Payload.GetUInt16();
		}
	}

	/// <summary>
	/// The unknown response class
	/// </summary>
	/// <seealso cref="LifxResponse"/>
	public class UnknownResponse : LifxResponse {
		/// <summary>
		/// The msg payload
		/// </summary>
		public Payload MsgPayload;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnknownResponse"/> class
		/// </summary>
		/// <param name="packet">The packet</param>
		internal UnknownResponse(LifxPacket packet) : base(packet) {
			MsgPayload = packet.Payload;
		}
	}
}