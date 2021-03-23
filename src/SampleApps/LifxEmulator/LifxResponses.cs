using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using LifxNetPlus;
using Console = Colorful.Console;

namespace LifxEmulator {
	/// <summary>
	/// Base class for LIFX response types
	/// </summary>
	public abstract class LifxResponse : LifxPacket {
		internal LifxPacket Packet { get; }
		
		internal LifxResponse(LifxPacket packet) : base(packet) {
			Packet = packet;
		}

		internal static LifxPacket Create(MessageType type, Product productInfo, DeviceData devData) {
			var newPacket = new LifxPacket(MessageType.DeviceAcknowledgement);
			switch (type) {
				case MessageType.DeviceGetService:
					newPacket.Type = MessageType.DeviceStateService;
					devData.OutPacket = new StateServiceResponse(newPacket);
					break;
				case MessageType.DeviceEchoRequest:
					newPacket.Type = MessageType.DeviceEchoResponse;
					devData.OutPacket = new EchoResponse(newPacket);
					break;
				case MessageType.DeviceGetInfo:
					newPacket.Type = MessageType.DeviceStateInfo;
					devData.OutPacket = new StateInfoResponse(newPacket);
					break;
				case MessageType.LightGetLightRestore:
					newPacket.Type = MessageType.LightStateLightRestore;
					devData.OutPacket = new LightStateLightRestoreResponse(newPacket, devData);
					break;
				case MessageType.LightGet:
				case MessageType.LightSetColor:
				case MessageType.LightSetWaveform:
				case MessageType.LightSetWaveformOptional:
					newPacket.Type = MessageType.LightState;
					devData.OutPacket = new LightStateResponse(newPacket, devData);
					break;
				case MessageType.LightSetPower:
				case MessageType.LightGetPower:
					newPacket.Type = MessageType.LightStatePower;
					devData.OutPacket = new StateLightPowerResponse(newPacket, devData);
					break;
				case MessageType.DeviceGetVersion:
					newPacket.Type = MessageType.DeviceStateVersion;
					devData.OutPacket = new StateVersionResponse(newPacket, productInfo);
					break;
				case MessageType.DeviceGetHostFirmware:
					newPacket.Type = MessageType.DeviceStateHostFirmware;
					devData.OutPacket = new StateHostFirmwareResponse(newPacket);
					break;
				case MessageType.DeviceGetWifiFirmware:
					newPacket.Type = MessageType.DeviceStateWifiFirmware;
					devData.OutPacket = new StateWifiFirmwareResponse(newPacket);
					break;
				case MessageType.GetExtendedColorZones:
					newPacket.Type = MessageType.StateExtendedColorZones;
					devData.OutPacket = new StateExtendedColorZonesResponse(newPacket, devData);
					break;
				case MessageType.SetColorZones:
				case MessageType.GetColorZones:
					newPacket.Type = MessageType.StateMultiZone;
					devData.OutPacket = new StateMultiZoneResponse(newPacket, devData);
					break;
				case MessageType.GetDeviceChain:
					newPacket.Type = MessageType.StateDeviceChain;
					devData.OutPacket = new StateDeviceChainResponse(newPacket);
					break;
				case MessageType.SetRelayPower:
				case MessageType.GetRelayPower:
					newPacket.Type = MessageType.StateRelayPower;
					devData.OutPacket = new StateRelayPowerResponse(newPacket);
					break;
				case MessageType.DeviceGetPower:
				case MessageType.DeviceSetPower:
					newPacket.Type = MessageType.DeviceStatePower;
					devData.OutPacket = new StatePowerResponse(newPacket, devData);
					break;
				case MessageType.DeviceGetLabel:
				case MessageType.DeviceSetLabel:
					newPacket.Type = MessageType.DeviceStateLabel;
					devData.OutPacket = new StateLabelResponse(newPacket, devData);
					break;
				case MessageType.DeviceGetLocation:
				case MessageType.DeviceSetLocation:
					newPacket.Type = MessageType.DeviceStateLocation;
					devData.OutPacket = new StateLocationResponse(newPacket, devData);
					break;
				case MessageType.DeviceGetGroup:
				case MessageType.DeviceSetGroup:
					newPacket.Type = MessageType.DeviceStateGroup;
					devData.OutPacket = new StateGroupResponse(newPacket, devData);
					break;
				case MessageType.DeviceGetWifiInfo:
					newPacket.Type = MessageType.DeviceStateWifiInfo;
					devData.OutPacket = new StateWifiInfoResponse(newPacket);
					break;
				case MessageType.DeviceGetOwner:
				case MessageType.DeviceSetOwner:
					newPacket.Type = MessageType.DeviceStateOwner;
					devData.OutPacket = new StateOwnerResponse(newPacket, devData);
					break;
				case MessageType.WanGet:
					newPacket.Type = MessageType.WanState;
					devData.OutPacket = new StateWanResponse(newPacket);
					break;
				case MessageType.TileGetEffect:
				case MessageType.tileSetEffect:
					newPacket.Type = MessageType.TileStateEffect;
					devData.OutPacket = new StateTileEffectResponse(newPacket);
					break;
				case MessageType.GetTileTapConfig:
					newPacket.Type = MessageType.StateTileTapConfig;
					devData.OutPacket = new StateTileTapConfigResponse(newPacket);
					break;
				case MessageType.MultiZoneGetEffect:
				case MessageType.MultiZoneSetEffect:
					newPacket.Type = MessageType.MultizoneStateEffect;
					devData.OutPacket = new StateMultizoneStateResponse(newPacket, devData);
					break;
				default:
					newPacket.Type = MessageType.DeviceAcknowledgement;
					devData.OutPacket = new AcknowledgementResponse(newPacket);
					break;
			}

			return devData.OutPacket;
		}
	}

	/// <summary>
	/// Response to GetService message.
	/// Provides the device Service and port.
	/// If the Service is temporarily unavailable, then the port value will be 0.
	/// </summary>
	internal class StateServiceResponse : LifxResponse {
		private byte Service { get; }
		private ulong Port { get; }

		internal StateServiceResponse(LifxPacket newPacket) : base(
			newPacket) {
			Service = 1;
			Port = 56700;
			Payload = new Payload(new object[] {Service, Port});
		}
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
	/// Response to GetWAN message.
	/// public enum Status {
	/// </summary>
	public class StateMultizoneStateResponse : LifxResponse {
		public UInt32 Instance { get; }
		public byte EffectType { get; }
		public UInt32 Speed { get; }
		public UInt64 Duration { get; }
		public Byte[] Parameter { get; }

		internal StateMultizoneStateResponse(LifxPacket newPacket, DeviceData devData) : base(newPacket) {
			Instance = devData.MultizoneId;
			EffectType = devData.EffectType;
			Speed = devData.EffectSpeed;
			Duration = devData.EffectDur;
			Parameter = devData.EffectParam;
			
			Payload = new Payload(new object[] {
				Instance, EffectType, (byte) 0, (byte) 0, Speed, Duration,
				(byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0,
				Parameter
			});
		}
	}



	/// <summary>
	/// Response to GetLabel message. Provides device label.
	/// </summary>
	public class StateOwnerResponse : LifxResponse {
		public DateTime Updated { get; }
		public string? Label { get; }

		public string? Owner { get; }

		internal StateOwnerResponse(LifxPacket newPacket, DeviceData devData) : base(newPacket) {
			var oBytes = devData.OwnerPacket;
			Payload = new Payload(new object[] {oBytes});
		}
	}

	/// <summary>
	/// Response to GetWAN message.
	/// public enum Status {
	/// </summary>
	public class StateWanResponse : LifxResponse {
		public byte State { get; }

		internal StateWanResponse(LifxPacket newPacket) : base(newPacket) {
			State = 1;
			Payload = new Payload(new object[] {State});
		}
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
		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public LifxColor[] Colors { get; }

		/// <summary>
		/// Count - total number of zones on the device
		/// </summary>
		public ushort Count { get; }

		/// <summary>
		/// Index - Zone the message starts from
		/// </summary>
		public ushort Index { get; }

		internal StateMultiZoneResponse(LifxPacket newPacket, DeviceData devData) : base(
			newPacket) {
			Count = 8;
			Index = 0;
			Colors = new LifxColor[Count];
			for (var i = Index; i < Count; i++) {
				Colors[i] = new LifxColor(255, 0, 0);
			}

			if (devData.ColorArray != null && devData.ColorArray.Length == Count) {
				Colors = devData.ColorArray;
			}

			var args = new List<object> {(byte) Count, (byte) Index};
			args.AddRange(Colors);
			Payload = new Payload(args.ToArray());
		}
	}

	public class StateWifiInfoResponse : LifxResponse {
		/// <summary>
		/// Radio receive signal strength in milliWatts
		/// </summary>
		public float Signal { get; set; }

		/// <summary>
		/// Bytes received since power on
		/// </summary>
		public uint Rx { get; set; }

		/// <summary>
		/// Bytes transmitted since power on
		/// </summary>
		public uint Tx { get; set; }

		internal StateWifiInfoResponse(LifxPacket newPacket) : base(newPacket) {
			Signal = 20;
			Tx = 666;
			Rx = 777;
			ushort reserved = 0;
			Payload = new Payload(new object[] {Signal, Tx, Rx, reserved});
		}
	}


	/// <summary>
	/// Response to GetWifiFirmware message.
	/// Provides Wifi subsystem information.
	/// </summary>
	public class StateWifiFirmwareResponse : LifxResponse {
		/// <summary>
		/// Firmware build time (epoch time)
		/// </summary>
		public DateTime Build { get; set; }

		/// <summary>
		/// Major firmware version number
		/// </summary>
		public ushort VersionMajor { get; set; }

		/// <summary>
		/// Minor firmware version number
		/// </summary>
		public ushort VersionMinor { get; set; }

		internal StateWifiFirmwareResponse(LifxPacket newPacket) : base(newPacket) {
			Build = DateTime.Now;
			ulong reserved = 0;
			VersionMinor = 3;
			VersionMajor = 6;
			Payload = new Payload(new object[] {Build, reserved, VersionMajor, VersionMinor});
		}
	}

	/// <summary>
	/// Response to GetLabel message. Provides device label.
	/// </summary>
	internal class StateLabelResponse : LifxResponse {
		public string? Label { get; }

		internal StateLabelResponse(LifxPacket newPacket, DeviceData devData) : base(newPacket) {
			Label = devData.Label;	
			Payload = new Payload(new object[] {Label});
		}
	}

	/// <summary>
	/// Provides run-time information of device.
	/// </summary>
	public class StateInfoResponse : LifxResponse {
		/// <summary>
		/// Current time
		/// </summary>
		public DateTime Time { get; set; }

		/// <summary>
		/// Last power off period, 5 second accuracy (in nanoseconds)
		/// </summary>
		public long Downtime { get; set; }

		/// <summary>
		/// Time since last power on (relative time in nanoseconds)
		/// </summary>
		public long Uptime { get; set; }

		internal StateInfoResponse(LifxPacket newPacket) : base(newPacket) {
			Time = DateTime.Now;
			Uptime = 5000;
			Downtime = 100000;
			var args = new List<object> {Time, Uptime, Downtime};
			Payload = new Payload(args.ToArray());
		}
	}


	/// <summary>
	/// Echo response with payload sent in the EchoRequest.
	/// </summary>
	public class EchoResponse : LifxResponse {
		/// <summary>
		/// Payload sent in the EchoRequest.
		/// </summary>
		public byte[] RequestPayload { get; set; }

		internal EchoResponse(LifxPacket newPacket) : base(newPacket) {
			RequestPayload = Payload.ToArray();
		}
	}


	/// <summary>
	/// The StateZone message represents the state of a single zone with the index field indicating which zone is represented. The count field contains the count of the total number of zones available on the device.
	/// </summary>
	public class StateDeviceChainResponse : LifxResponse {
		/// <summary>
		/// Start Index - Zone the message starts from
		/// </summary>
		public byte StartIndex { get; }

		/// <summary>
		/// Count - total number of zones on the device
		/// </summary>
		public byte TotalCount { get; }

		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public List<Tile> Tiles { get; }

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
	}
	
	public class StateLightPowerResponse : LifxResponse {
		/// <summary>
		/// Zero implies standby and non-zero sets a corresponding power draw level. Currently only 0 and 65535 are supported.
		/// </summary>
		public ushort Level { get; set; }

		internal StateLightPowerResponse(LifxPacket packet, DeviceData devData) : base(packet) {
			Level = devData.PowerLevel;
			
			Payload = new Payload(new object[]{Level});
		}
	}

	public class StatePowerResponse : LifxResponse {
		/// <summary>
		/// Zero implies standby and non-zero sets a corresponding power draw level. Currently only 0 and 65535 are supported.
		/// </summary>
		public ushort Level { get; set; }

		internal StatePowerResponse(LifxPacket packet, DeviceData devData) : base(packet) {
			Level = devData.PowerLevel;
			
			var args = new List<object> {Level};
			Payload = new Payload(args.ToArray());
		}
	}

	public class StateLocationResponse : LifxResponse {
		public byte[] Location { get; set; }

		public ulong Updated { get; set; }

		public string Label { get; set; }

		internal StateLocationResponse(LifxPacket newPacket, DeviceData devData) : base(newPacket) {
			Location = devData.Location;
			Label = devData.LocationLabel;
			Updated = devData.LocationUpdated;

			Payload = new Payload(new object[] {Location, Label, Updated});
		}
	}

	/// <summary>
	/// Device group.
	/// </summary>
	public class StateGroupResponse : LifxResponse {
		public byte[] Group { get; set; }

		public ulong Updated { get; set; }

		public string Label { get; set; }

		internal StateGroupResponse(LifxPacket newPacket, DeviceData devData) : base(newPacket) {
			
			Group = devData.Group;
			Label = devData.GroupLabel;
			Updated = devData.GroupUpdated;
			Payload = new Payload(new object[] {Group, Label, Updated});
		}
	}

	/// <summary>
	/// Get the list of colors currently being displayed by zones
	/// </summary>
	public class StateExtendedColorZonesResponse : LifxResponse {
		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public List<LifxColor> Colors { get; private set; }

		/// <summary>
		/// Count - total number of zones on the device
		/// </summary>
		public ushort Count { get; private set; }

		/// <summary>
		/// Index - Zone the message starts from
		/// </summary>
		public ushort Index { get; private set; }

		internal StateExtendedColorZonesResponse(LifxPacket newPacket, DeviceData devData) :
			base(newPacket) {
			Colors = new List<LifxColor>();
			Count = 8;
			Index = 0;
			for (var i = Index; i < Count; i++) {
				Colors.Add(new LifxColor(255, 0, 0));
			}

			if (devData.ColorArray != null && devData.ColorArray.Length == 64) {
				Colors = devData.ColorArray.ToList();
			}
			var args = new List<object> {Count, Index, (byte) Colors.Count};
			args.AddRange(Colors);
			Payload = new Payload(args.ToArray());
		}
	}

	/// <summary>
	/// Sent by a device to provide the current light state
	/// </summary>
	public class LightStateResponse : LifxResponse {
		/// <summary>
		/// Power state
		/// </summary>
		public bool IsOn { get; }

		/// <summary>
		/// Light label
		/// </summary>
		public string Label { get; }

		/// <summary>
		/// Brightness (0=off, 65535=full brightness)
		/// </summary>
		public ushort Brightness { get; }

		/// <summary>
		/// Hue
		/// </summary>
		public LifxColor Color { get; }

		internal LightStateResponse(LifxPacket newPacket, DeviceData devData) : base(newPacket) {
				Label = devData.Label;
				Brightness = devData.PowerLevel;
				Color = devData.Color;
				var args = new List<object> {
				Color,
				(byte)0, // Reserved
				(byte)0, // Reserved
				(byte)0, // Reserved
				Brightness,
				Label,
				(byte)0, // Reserved
				(byte)0, // Reserved
				(byte)0, // Reserved
				(byte)0, // Reserved
				(byte)0, // Reserved
				(byte)0, // Reserved
				(byte)0, // Reserved
				(byte)0 // Reserved
			};
			Payload = new Payload(args.ToArray());
		}
	}
	
	/// <summary>
	/// Sent by a device to provide the current light state
	/// </summary>
	public class LightStateLightRestoreResponse : LifxResponse {
		/// <summary>
		/// Power state
		/// </summary>
		public LifxColor Color { get; }
		
		public bool RestoreWhite { get; }
		
		public bool RestoreState { get; }

		
		
		internal LightStateLightRestoreResponse(LifxPacket newPacket, DeviceData devData) : base(newPacket) {
			Color = devData.Color;
			var args = new List<object> {
				Color,
				(byte)0,
				(byte)0,
				(byte)0 // Reserved
			};
			Payload = new Payload(args.ToArray());
		}
	}

	/// <summary>
	/// Response to GetVersion message.	Provides the hardware version of the device.
	/// </summary>
	public class StateVersionResponse : LifxResponse {
		/// <summary>
		/// Product ID
		/// </summary>
		public uint Product { get; }

		/// <summary>
		/// Vendor ID
		/// </summary>
		public uint Vendor { get; }

		/// <summary>
		/// Hardware version
		/// </summary>
		public uint Version { get; }

		internal StateVersionResponse(LifxPacket newPacket, Product productData) : base(
			newPacket) {
			Product = 32;

			Product = (uint) productData.pid;
			Vendor = 1;
			Version = 117506305;
			var args = new List<object> {Vendor, Product, Version};
			Payload = new Payload(args.ToArray());
		}
	}

	/// <summary>
	/// Response to GetHostFirmware message. Provides host firmware information.
	/// </summary>
	public class StateHostFirmwareResponse : LifxResponse {
		/// <summary>
		/// Firmware build time
		/// </summary>
		public DateTime Build { get; }

		public uint VersionMajor { get; }

		/// <summary>
		/// Firmware version
		/// </summary>
		public uint VersionMinor { get; }

		internal StateHostFirmwareResponse(LifxPacket newPacket) : base(
			newPacket) {
			Build = DateTime.Now;
			ulong reserved = 0;
			ulong version = 1532997580;
			var args = new List<object> {Build, reserved, version};
			Payload = new Payload(args.ToArray());
		}
	}

	/// <summary>
	/// Response to GetVersion message.	Provides the hardware version of the device.
	/// </summary>
	public class StateRelayPowerResponse : LifxResponse {
		/// <summary>
		/// The value of the relay
		/// </summary>
		public int Level { get; }

		/// <summary>
		/// The relay on the switch starting from 0
		/// </summary>
		public int RelayIndex { get; }

		internal StateRelayPowerResponse(LifxPacket newPacket) : base(
			newPacket) {
			RelayIndex = 0;
			Level = 65536;
			Payload = new Payload(new object[] {RelayIndex, Level});
		}
	}
}