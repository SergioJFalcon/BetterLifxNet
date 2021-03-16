using System;
using System.Collections.Generic;
using LifxNetPlus;

namespace LifxEmulator {
	/// <summary>
	/// Base class for LIFX response types
	/// </summary>
	public abstract class LifxResponse : LifxPacket {
		internal static LifxResponse Create(LifxPacket packet, int deviceVersion) {
			packet.Payload.Reset();
			var newPacket = new LifxPacket(MessageType.DeviceAcknowledgement);
			switch (packet.Type) {
				case MessageType.DeviceGetService:
					newPacket.Type = MessageType.DeviceStateService;
					newPacket.Addressable = true;
					return new StateServiceResponse(newPacket);
				case MessageType.DeviceEchoRequest:
					newPacket.Type = MessageType.DeviceEchoResponse;
					return new EchoResponse(newPacket);
				case MessageType.DeviceGetInfo:
					newPacket.Type = MessageType.DeviceStateInfo;
					return new StateInfoResponse(newPacket);
				case MessageType.LightGet:
					newPacket.Type = MessageType.LightState;
					return new LightStateResponse(newPacket);
				case MessageType.DeviceGetVersion:
					newPacket.Type = MessageType.DeviceStateVersion;
					return new StateVersionResponse(newPacket, deviceVersion);
				case MessageType.DeviceGetHostFirmware:
					newPacket.Type = MessageType.DeviceStateHostFirmware;
					return new StateHostFirmwareResponse(newPacket, deviceVersion);
				case MessageType.DeviceGetWifiFirmware:
					newPacket.Type = MessageType.DeviceStateWifiFirmware;
					return new StateWifiFirmwareResponse(newPacket);
				case MessageType.GetExtendedColorZones:
					newPacket.Type = MessageType.StateExtendedColorZones;
					return new StateExtendedColorZonesResponse(newPacket);
				case MessageType.GetColorZones:
					newPacket.Type = MessageType.StateMultiZone;
					return new StateMultiZoneResponse(newPacket);
				case MessageType.GetDeviceChain:
					newPacket.Type = MessageType.StateDeviceChain;
					return new StateDeviceChainResponse(newPacket);
				case MessageType.GetRelayPower:
					newPacket.Type = MessageType.StateRelayPower;
					return new StateRelayPowerResponse(newPacket);
				case MessageType.DeviceGetPower:
					newPacket.Type = MessageType.DeviceStatePower;
					return new StatePowerResponse(newPacket);
				case MessageType.DeviceSetPower:
					newPacket.Type = MessageType.DeviceAcknowledgement;
					return new AcknowledgementResponse(newPacket);
				case MessageType.DeviceGetLabel:
					newPacket.Type = MessageType.DeviceStateLabel;
					return new StateLabelResponse(newPacket);
				case MessageType.DeviceGetLocation:
					newPacket.Type = MessageType.DeviceStateLocation;
					return new StateLocationResponse(newPacket);
				case MessageType.DeviceGetGroup:
					newPacket.Type = MessageType.DeviceStateGroup;
					return new StateGroupResponse(newPacket);
				case MessageType.DeviceGetWifiInfo:
					newPacket.Type = MessageType.DeviceStateWifiInfo;
					return new StateWifiInfoResponse(newPacket);
				case MessageType.DeviceGetOwner:
					newPacket.Type = MessageType.DeviceStateOwner;
					return new StateOwnerResponse(newPacket);
				case MessageType.WanGet:
					newPacket.Type = MessageType.WanState;
					return new StateWanResponse(newPacket);
				case MessageType.TileGetEffect:
					newPacket.Type = MessageType.TileStateEffect;
					return new StateTileEffectResponse(newPacket);
				case MessageType.GetTileTapConfig:
					newPacket.Type = MessageType.StateTileTapConfig;
					return new StateTileTapConfigResponse(newPacket);
				default:
					newPacket.Type = MessageType.DeviceAcknowledgement;
					return new AcknowledgementResponse(newPacket);
			}
		}

		internal LifxResponse(LifxPacket packet) : base(packet) {
			Packet = packet;
		}

		internal LifxPacket Packet { get; }
	}
	
	/// <summary>
	/// Response to GetService message.
	/// Provides the device Service and port.
	/// If the Service is temporarily unavailable, then the port value will be 0.
	/// </summary>
	internal class StateServiceResponse : LifxResponse {
		internal StateServiceResponse(LifxPacket newPacket) : base(
			newPacket) {
			Service = 1;
			Port = 56700;
			Payload = new Payload(new object[]{Service, Port});
		}

		private byte Service { get; }
		private ulong Port { get; }
	}
	
	/// <summary>
	/// State Tile Tap Config
	/// </summary>
	internal class StateTileTapConfigResponse : LifxResponse {
		internal StateTileTapConfigResponse(LifxPacket newPacket) : base(
			newPacket) {
		}
	}
	
	/// <summary>
	/// Response to a state tile get request 
	/// </summary>
	internal class StateTileEffectResponse : LifxResponse {
		internal StateTileEffectResponse(LifxPacket newPacket) : base(
			newPacket) {
		}
	}

	
	/// <summary>
	/// Response to GetLabel message. Provides device label.
	/// </summary>
	public class StateOwnerResponse : LifxResponse {
		internal StateOwnerResponse(LifxPacket newPacket) : base(newPacket) {
			var oBytes = new byte[] {0xEC, 0x30, 0x7E, 0x9A, 0xAE, 0xC9, 0x4F, 0x72, 0xB3, 0xF8, 0xE7, 
				0x38, 0x44, 0x04, 0x4A, 0x4A};
			var lBytes = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00};
			Updated = DateTime.Now;
			var res = (ushort) 0;
			Payload = new Payload(new object[] {(byte) 0, oBytes, lBytes, res, Updated});
		}

		public string? Owner { get; }
		public string? Label { get; }
		public DateTime Updated { get; }
	}
	
	/// <summary>
	/// Response to GetWAN message.
	/// public enum Status {
	/// </summary>
	public class StateWanResponse : LifxResponse {
		internal StateWanResponse(LifxPacket newPacket) : base(newPacket) {
			State = 1;
			Payload = new Payload(new object[] {State});
		}

		public byte State { get; }
	}


	/// <summary>
	/// Response to any message sent with ack_required set to 1. 
	/// </summary>
	internal class AcknowledgementResponse : LifxResponse {
		internal AcknowledgementResponse(LifxPacket newPacket) : base(
			newPacket) {
		}
	}

	/// <summary>
	/// Get the list of colors currently being displayed by zones
	/// </summary>
	public class StateMultiZoneResponse : LifxResponse {
		internal StateMultiZoneResponse(LifxPacket newPacket) : base(
			newPacket) {
			Count = 8;
			Index = 0;
			Colors = new LifxColor[Count];
			for (var i = Index; i < Count; i++) {
				Colors[i] = new LifxColor(255,0,0);
			}

			var args = new List<object> {(byte)Count, (byte)Index};
			args.AddRange(Colors);
			Payload = new Payload(args.ToArray());
		}

		/// <summary>
		/// Count - total number of zones on the device
		/// </summary>
		public ushort Count { get; }

		/// <summary>
		/// Index - Zone the message starts from
		/// </summary>
		public ushort Index { get; }

		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public LifxColor[] Colors { get; }
	}
	
	public class StateWifiInfoResponse : LifxResponse {
		internal StateWifiInfoResponse(LifxPacket newPacket) : base(newPacket) {
			Signal = 20;
			Tx = 666;
			Rx = 777;
			ushort reserved = 0; 
			Payload = new Payload(new object[] {Signal, Tx, Rx, reserved });
		}

		/// <summary>
		/// Bytes received since power on
		/// </summary>
		public uint Rx { get; set; }

		/// <summary>
		/// Bytes transmitted since power on
		/// </summary>
		public uint Tx { get; set; }

		/// <summary>
		/// Radio receive signal strength in milliWatts
		/// </summary>
		public float Signal { get; set; }
	}



	/// <summary>
	/// Response to GetWifiFirmware message.
	/// Provides Wifi subsystem information.
	/// </summary>
	public class StateWifiFirmwareResponse : LifxResponse {
		internal StateWifiFirmwareResponse(LifxPacket newPacket) : base(newPacket) {
			Build = DateTime.Now;
			ulong reserved = 0;
			VersionMinor = 3;
			VersionMajor = 6;
			Payload = new Payload(new object[] {Build, reserved, VersionMajor, VersionMinor});
		}

		/// <summary>
		/// Firmware build time (epoch time)
		/// </summary>
		public DateTime Build { get; set; }

		/// <summary>
		/// Minor firmware version number
		/// </summary>
		public ushort VersionMinor { get; set; }

		/// <summary>
		/// Major firmware version number
		/// </summary>
		public ushort VersionMajor { get; set; }
	}
	
	/// <summary>
	/// Response to GetLabel message. Provides device label.
	/// </summary>
	internal class StateLabelResponse : LifxResponse {
		internal StateLabelResponse(LifxPacket newPacket) : base(newPacket) {
			Label = "Emulator";
			Payload = new Payload(new object[] {Label});
		}

		public string? Label { get; }
	}

	/// <summary>
	/// Provides run-time information of device.
	/// </summary>
	public class StateInfoResponse : LifxResponse {
		internal StateInfoResponse(LifxPacket newPacket) : base(newPacket) {
			Time = DateTime.Now;
			Uptime = 5000;
			Downtime = 100000;
			var args = new List<object> {Time, Uptime, Downtime};
			Payload = new Payload(args.ToArray() );
		}

		/// <summary>
		/// Current time
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		/// Time since last power on (relative time in nanoseconds)
		/// </summary>
		public long Uptime { get; set; }

		/// <summary>
		/// Last power off period, 5 second accuracy (in nanoseconds)
		/// </summary>
		public long Downtime { get; set; }
	}
	

	/// <summary>
	/// Echo response with payload sent in the EchoRequest.
	/// </summary>
	public class EchoResponse : LifxResponse {
		internal EchoResponse(LifxPacket newPacket) : base(newPacket) {
			RequestPayload = Payload.ToArray();
		}

		/// <summary>
		/// Payload sent in the EchoRequest.
		/// </summary>
		public byte[] RequestPayload { get; set; }
	}
	

	/// <summary>
	/// The StateZone message represents the state of a single zone with the index field indicating which zone is represented. The count field contains the count of the total number of zones available on the device.
	/// </summary>
	public class StateDeviceChainResponse : LifxResponse {
		internal StateDeviceChainResponse(LifxPacket newPacket) : base(newPacket) {
			Tiles = new List<Tile>();
			TotalCount = 16;
			StartIndex = 0;
			var args = new List<object>();
			args.Add(StartIndex);
			for (var i = StartIndex; i < TotalCount; i++) {
				var tile = new Tile();
				tile.CreateDefault(i);
				Tiles.Add(tile);
				args.Add(tile.ToBytes());
			}
			args.Add(TotalCount);
			Payload = new Payload(args.ToArray());
			Payload.Rewind();
		}

		/// <summary>
		/// Count - total number of zones on the device
		/// </summary>
		public byte TotalCount { get; }

		/// <summary>
		/// Start Index - Zone the message starts from
		/// </summary>
		public byte StartIndex { get; }

		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public List<Tile> Tiles { get; }
	}
	
	public class StatePowerResponse : LifxResponse {
		internal StatePowerResponse(LifxPacket packet) : base(packet) {
			Level = 65535;
			var args = new List<object> {Level};
			Payload = new Payload(args.ToArray());
		}
		
		/// <summary>
		/// Zero implies standby and non-zero sets a corresponding power draw level. Currently only 0 and 65535 are supported.
		/// </summary>
		public ulong Level { get; set; }
		
	}
	
	public class StateLocationResponse : LifxResponse {
		internal StateLocationResponse(LifxPacket newPacket) : base(newPacket) {
			var rand = new Random();
			var location = new byte[16];
			rand.NextBytes(location);
			Location = location;
			Label = "Emu room";
			Updated = DateTime.Now;
			Payload = new Payload(new object[] {Location, Label, Updated});
		}

		public byte[] Location { get; set; }

		public string Label { get; set; }

		public DateTime Updated { get; set; }
	}
	
	/// <summary>
	/// Device group.
	/// </summary>
	public class StateGroupResponse : LifxResponse {
		internal StateGroupResponse(LifxPacket newPacket) : base(newPacket) {
			var rand = new Random();
			var location = new byte[16];
			rand.NextBytes(location);
			Group = location;
			Label = "Emu group";
			Updated = DateTime.Now;
			Payload = new Payload(new object[] {Group, Label, Updated});
		}

		public byte[] Group { get; set; }

		public string Label { get; set; }

		public DateTime Updated { get; set; }
	}

	/// <summary>
	/// Get the list of colors currently being displayed by zones
	/// </summary>
	public class StateExtendedColorZonesResponse : LifxResponse {
		internal StateExtendedColorZonesResponse(LifxPacket newPacket) :
			base(newPacket) {
			Colors = new List<LifxColor>();
			Count = 8;
			Index = 0;
			for (var i = Index; i < Count; i++) {
				Colors.Add(new LifxColor(255,0,0));
			}

			var args = new List<object> {Count, Index, (byte) Colors.Count};
			args.AddRange(Colors);
			Payload = new Payload(args.ToArray());
		}

		/// <summary>
		/// Count - total number of zones on the device
		/// </summary>
		public ushort Count { get; private set; }

		/// <summary>
		/// Index - Zone the message starts from
		/// </summary>
		public ushort Index { get; private set; }

		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public List<LifxColor> Colors { get; private set; }
	}

	/// <summary>
	/// Sent by a device to provide the current light state
	/// </summary>
	public class LightStateResponse : LifxResponse {
		internal LightStateResponse(LifxPacket newPacket) : base(newPacket) {

			var args = new List<object> {
				new LifxColor(255, 0, 0),
				0,
				(uint) 65535,
				"Test Light",
				(ulong) 0
			};
			Payload = new Payload(args.ToArray());
		}

		/// <summary>
		/// Hue
		/// </summary>
		public ushort Hue { get; }

		/// <summary>
		/// Saturation (0=desaturated, 65535 = fully saturated)
		/// </summary>
		public ushort Saturation { get; }

		/// <summary>
		/// Brightness (0=off, 65535=full brightness)
		/// </summary>
		public ushort Brightness { get; }

		/// <summary>
		/// Bulb color temperature
		/// </summary>
		public ushort Kelvin { get; }

		/// <summary>
		/// Power state
		/// </summary>
		public bool IsOn { get; }

		/// <summary>
		/// Light label
		/// </summary>
		public string Label { get; }
	}

	/// <summary>
	/// Response to GetVersion message.	Provides the hardware version of the device.
	/// </summary>
	public class StateVersionResponse : LifxResponse {
		internal StateVersionResponse(LifxPacket newPacket, int deviceVersion) : base(
			newPacket) {
			Product = 32;

			switch (deviceVersion) {
				case 0:
					Product = 1;
					break;
				case 1:
					Product = 31;
					break;
				case 2:
					Product = 32;
					break;
				case 3:
					Product = 38;
					break;
				case 4:
					Product = 55;
					break;
				case 5:
					Product = 70;
					break;
			}
			Vendor = 1;
			Version = 117506305;
			var args = new List<object> {Vendor, Product, Version};
			Payload = new Payload(args.ToArray());
		}

		/// <summary>
		/// Vendor ID
		/// </summary>
		public uint Vendor { get; }

		/// <summary>
		/// Product ID
		/// </summary>
		public uint Product { get; }

		/// <summary>
		/// Hardware version
		/// </summary>
		public uint Version { get; }
	}

	/// <summary>
	/// Response to GetHostFirmware message. Provides host firmware information.
	/// </summary>
	public class StateHostFirmwareResponse : LifxResponse {
		internal StateHostFirmwareResponse(LifxPacket newPacket, int deviceVersion) : base(
			newPacket) {
			Build = DateTime.Now;
			ulong reserved = 0;
			ulong version = 1532997580;
			var args = new List<object> {Build, reserved, version};
			Payload = new Payload(args.ToArray());
		}

		/// <summary>
		/// Firmware build time
		/// </summary>
		public DateTime Build { get; }

		/// <summary>
		/// Firmware version
		/// </summary>
		public uint VersionMinor { get; }
		public uint VersionMajor { get; }
		
	}

	/// <summary>
	/// Response to GetVersion message.	Provides the hardware version of the device.
	/// </summary>
	public class StateRelayPowerResponse : LifxResponse {
		internal StateRelayPowerResponse(LifxPacket newPacket) : base(
			newPacket) {
			RelayIndex = 0;
			Level = 65536;
			Payload = new Payload(new object[]{RelayIndex, Level});
		}

		/// <summary>
		/// The relay on the switch starting from 0
		/// </summary>
		public int RelayIndex { get; }

		/// <summary>
		/// The value of the relay
		/// </summary>
		public int Level { get; }
	}

	
}