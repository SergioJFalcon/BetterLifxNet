using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace LifxNetPlus {
	/// <summary>
	/// Base class for LIFX response types
	/// </summary>
	public abstract class LifxResponse : LifxPacket {
		internal LifxPacket Packet { get; }

		internal LifxResponse(LifxPacket packet) : base(packet) {
			Packet = packet;
		}

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
	/// Response to any message sent with ack_required set to 1. 
	/// </summary>
	internal class AcknowledgementResponse : LifxResponse {
		internal AcknowledgementResponse(LifxPacket packet) : base(
			packet) {
		}
	}

	/// <summary>
	/// State Tile Tap Config
	/// </summary>
	internal class StateTileTapConfigResponse : LifxResponse {
		internal StateTileTapConfigResponse(LifxPacket packet) : base(
			packet) {
		}
	}

	/// <summary>
	/// Response to a state tile get request 
	/// </summary>
	public class StateTileEffectResponse : LifxResponse {
		internal StateTileEffectResponse(LifxPacket packet) : base(
			packet) {
		}
	}

	/// <summary>
	/// The StateZone message represents the state of a single zone with the index field indicating which zone is represented. The count field contains the count of the total number of zones available on the device.
	/// </summary>
	public class StateZoneResponse : LifxResponse {
		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public LifxColor Color { get; }

		/// <summary>
		/// Count - total number of zones on the device
		/// </summary>
		public ushort Count { get; }

		/// <summary>
		/// Index - Zone the message starts from
		/// </summary>
		public ushort Index { get; }

		internal StateZoneResponse(LifxPacket packet) : base(packet) {
			Count = packet.Payload.GetUInt16();
			Index = packet.Payload.GetUInt16();
			Color = new Payload().GetColor();
		}
	}


	/// <summary>
	/// Response to GetHostInfo message.
	/// Provides host MCU information.
	/// </summary>
	public class StateHostInfoResponse : LifxResponse {
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

		internal StateHostInfoResponse(LifxPacket packet) : base(packet) {
			Signal = packet.Payload.GetFloat32();
			Tx = packet.Payload.GetUInt32();
			Rx = packet.Payload.GetUInt32();
		}
	}


	/// <summary>
	/// Response to GetWifiInfo message.
	/// Provides host Wifi information.
	/// </summary>
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

		internal StateWifiInfoResponse(LifxPacket packet) : base(packet) {
			Signal = packet.Payload.GetFloat32();
			Tx = packet.Payload.GetUInt32();
			Rx = packet.Payload.GetUInt32();
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
		public ulong Build { get; set; }

		/// <summary>
		/// Major firmware version number
		/// </summary>
		public ushort VersionMajor { get; set; }

		/// <summary>
		/// Minor firmware version number
		/// </summary>
		public ushort VersionMinor { get; set; }

		internal StateWifiFirmwareResponse(LifxPacket packet) : base(packet) {
			Build = packet.Payload.GetUInt64();
			// Skip 64-bit reserved
			packet.Payload.Advance(8);
			VersionMinor = packet.Payload.GetUInt16();
			VersionMajor = packet.Payload.GetUInt16();
		}
	}


	/// <summary>
	/// Provides device power level.
	/// </summary>
	public class StatePowerResponse : LifxResponse {
		/// <summary>
		/// Zero implies standby and non-zero sets a corresponding power draw level. Currently only 0 and 65535 are supported.
		/// </summary>
		public ulong Level { get; set; }

		internal StatePowerResponse(LifxPacket packet) : base(packet) {
			Level = packet.Payload.GetUInt16();
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

		internal StateInfoResponse(LifxPacket packet) : base(packet) {
			Time = DateTimeOffset.FromUnixTimeSeconds(packet.Payload.GetInt64()).DateTime;
			Uptime = packet.Payload.GetInt64();
			Downtime = packet.Payload.GetInt64();
		}
	}


	/// <summary>
	/// Device location.
	/// </summary>
	public class StateLocationResponse : LifxResponse {
		public byte[] Location { get; set; }

		public string Label { get; set; }

		public ulong Updated { get; set; }

		internal StateLocationResponse(LifxPacket packet) : base(packet) {
			Location = packet.Payload.GetBytes(16);
			Label = packet.Payload.GetString(32);
			Updated = packet.Payload.GetUInt64();
		}
	}

	/// <summary>
	/// Device group.
	/// </summary>
	public class StateGroupResponse : LifxResponse {
		public byte[] Group { get; set; }

		public string Label { get; set; }

		public ulong Updated { get; set; }

		internal StateGroupResponse(LifxPacket packet) : base(packet) {
			Group = packet.Payload.GetBytes(16);
			Label = packet.Payload.GetString(32);
			Updated = packet.Payload.GetUInt64();
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

		internal EchoResponse(LifxPacket packet) : base(packet) {
			RequestPayload = packet.Payload.ToArray();
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
		public int TotalCount { get; }

		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public List<Tile> Tiles { get; }

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

		internal StateMultiZoneResponse(LifxPacket packet) : base(
			packet) {
			Colors = new LifxColor[8];
			Count = packet.Payload.GetUint8();
			Index = packet.Payload.GetUint8();
			for (var i = 0; i < 8; i++) {
				Debug.WriteLine($"Reading color {i}.");
				Colors[i] = packet.Payload.GetColor();
			}

			Debug.WriteLine("Colors read.");
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

		internal StateExtendedColorZonesResponse(LifxPacket packet) :
			base(packet) {
			Colors = new List<LifxColor>();
			Count = packet.Payload.GetUInt16();
			Index = packet.Payload.GetUInt16();
			while (packet.Payload.HasContent()) {
				Colors.Add(packet.Payload.GetColor());
			}
		}
	}

	/// <summary>
	/// Response to GetService message.
	/// Provides the device Service and port.
	/// If the Service is temporarily unavailable, then the port value will be 0.
	/// </summary>
	[Serializable]
	public class StateServiceResponse : LifxResponse {
		[JsonProperty] public byte Service { get; }
		[JsonProperty] public uint Port { get; }

		public StateServiceResponse(LifxPacket packet) : base(
			packet) {
			Service = packet.Payload.GetUint8();
			Port = packet.Payload.GetUInt32();
		}
	}

	/// <summary>
	/// Response to any message sent with ack_required set to 1. 
	/// </summary>
	public class StateTileState16Response : LifxResponse {
		public LifxColor[] Colors { get; }

		public uint TileIndex { get; }
		public uint Width { get; }
		public uint X { get; }
		public uint Y { get; }

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
				} else {
					Debug.WriteLine($"Content size mismatch fetching colors: {i}/64: ");
				}
			}
		}
	}
	
	/// <summary>
	/// Response to any message sent with ack_required set to 1. 
	/// </summary>
	public class StateTileState64Response : LifxResponse {
		public LifxColor[] Colors { get; }

		public uint TileIndex { get; }
		public uint Width { get; }
		public uint X { get; }
		public uint Y { get; }

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
				} else {
					Debug.WriteLine($"Content size mismatch fetching colors: {i}/64: ");
				}
			}
		}
	}

	/// <summary>
	/// Response to GetLabel message. Provides device label.
	/// </summary>
	internal class StateLabelResponse : LifxResponse {
		public string? Label { get; }

		internal StateLabelResponse(LifxPacket packet) : base(packet) {
			Label = packet.Payload.GetString().Replace("\0", "");
		}
	}

	/// <summary>
	/// Response to GetLabel message. Provides device label.
	/// </summary>
	public class StateOwnerResponse : LifxResponse {
		public byte[] Label { get; }

		public byte[] Owner { get; }
		public ulong Updated { get; }

		internal StateOwnerResponse(LifxPacket packet) : base(packet) {
			packet.Payload.Advance(2); // Reserved 1
			Owner = packet.Payload.GetBytes(16);
			Label = packet.Payload.GetBytes(16);
			packet.Payload.Advance(2); // Reserved 2
			Updated = packet.Payload.GetUInt64();
		}
	}

	/// <summary>
	/// Response to GetWAN message.
	/// public enum Status {
	// OFF(0),
	// CONNECTED(1),
	// ERROR_UNAUTHORIZED(2),
	// ERROR_OVER_CAPACITY(3),
	// ERROR_OVER_RATE(4),
	// ERROR_NO_ROUTE(5),
	// ERROR_INTERNAL_CLIENT(6),
	// ERROR_INTERNAL_SERVER(7),
	// ERROR_DNS_FAILURE(8),
	// ERROR_SSL_FAILURE(9),
	// ERROR_CONNECTION_REFUSED(10),
	// CONNECTING(11),
	// WIFI_STOPPED(12),
	// WIFI_SUSPENDED(13),
	// WIFI_IP_CHANGED(14),
	// CONFIG_CHANGED(15),
	// ERROR_EMPTY_BROKER_IP(20),
	// ERROR_CREATE_SSL_CONTEXT(21),
	// ERROR_SET_X509_OBJECT(22),
	// ERROR_SET_CA_CERTIFICATE(23),
	// ERROR_CREATE_SSL_CONNECTION(24),
	// ERROR_NOT_INITIALIZED(25),
	// ERROR_REMOTE_CLOSED(26),
	// ERROR_PROTOCOL_FRAMING(27),
	// ERROR_PROTOCOL_TIMEOUT(28),
	// ERROR_MISSING_KEEPALIVE(29),
	// ERROR_AUTHORIZATION_SEND(30),
	// ERROR_AUTHORIZATION_CHECK(31),
	// ERROR_KEEPALIVE_SEND(32),
	// ERROR_INCOMPLETE_FRAME_TIMEOUT(33),
	// ERROR_MEMORY_ALLOC(34),
	// ERROR_LX_TCP_STREAM_APPEND(35),
	// ERROR_TX_ERRORS_EXCEEDED(36),
	// ERROR_NOT_STATION_MODE(37),
	// ERROR_SOCKET_EBADF(40),
	// ERROR_SOCKET_ENOMEM(41),
	// ERROR_SOCKET_EFAULT(42),
	// ERROR_SOCKET_EINVAL(43),
	// ERROR_SOCKET_ENFILE(44),
	// ERROR_SOCKET_ENOSYS(45),
	// ERROR_SOCKET_EOPNOTSUPP(46),
	// ERROR_SOCKET_EPFNOSUPPORT(47),
	// ERROR_SOCKET_ECONNRESET(48),
	// ERROR_SOCKET_ENOBUFS(49),
	// ERROR_SOCKET_EAFNOSUPPORT(50),
	// ERROR_SOCKET_EPROTOTYPE(51),
	// ERROR_SOCKET_ENOTSOCK(52),
	// ERROR_SOCKET_ENOPROTOOPT(53),
	// ERROR_SOCKET_ESHUTDOWN(54),
	// ERROR_SOCKET_ECONNREFUSED(55),
	// ERROR_SOCKET_EADDRINUSE(56),
	// ERROR_SOCKET_ECONNABORTED(57),
	// ERROR_SOCKET_ENETUNREACH(58),
	// ERROR_SOCKET_ENETDOWN(59),
	// ERROR_SOCKET_ETIMEDOUT(60),
	// ERROR_SOCKET_EHOSTDOWN(61),
	// ERROR_SOCKET_EHOSTUNREACH(62),
	// ERROR_SOCKET_EINPROGRESS(63),
	// ERROR_SOCKET_EALREADY(64),
	// ERROR_SOCKET_EDESTADDRREQ(65),
	// ERROR_SOCKET_EMSGSIZE(66),
	// ERROR_SOCKET_EPROTONOSUPPORT(67),
	// ERROR_SOCKET_ESOCKTNOSUPPORT(68),
	// ERROR_SOCKET_EADDRNOTAVAIL(69),
	// ERROR_SOCKET_ENETRESET(70),
	// ERROR_SOCKET_EISCONN(71),
	// ERROR_SOCKET_ENOTCONN(72),
	// ERROR_DNS_HOST_NOT_FOUND(80),
	// ERROR_DNS_NO_DATA(81),
	// ERROR_DNS_NO_RECOVERY(82),
	// ERROR_DNS_TRY_AGAIN(83),
	// ERROR_X509_FEATURE_UNAVAILABLE(90),
	// ERROR_X509_UNKNOWN_OID(91),
	// ERROR_X509_INVALID_FORMAT(92),
	// ERROR_X509_INVALID_VERSION(93),
	// ERROR_X509_INVALID_SERIAL(94),
	// ERROR_X509_INVALID_ALG(95),
	// ERROR_X509_INVALID_NAME(96),
	// ERROR_X509_INVALID_DATE(97),
	// ERROR_X509_INVALID_SIGNATURE(98),
	// ERROR_X509_INVALID_EXTENSIONS(99),
	// ERROR_X509_UNKNOWN_VERSION(100),
	// ERROR_X509_UNKNOWN_SIG_ALG(101),
	// ERROR_X509_SIG_MISMATCH(102),
	// ERROR_X509_CERT_VERIFY_FAILED(103),
	// ERROR_X509_CERT_UNKNOWN_FORMAT(104),
	// ERROR_X509_BAD_INPUT_DATA(105),
	// ERROR_X509_ALLOC_FAILED(106),
	// ERROR_X509_FILE_IO_ERROR(107),
	// ERROR_X509_BUFFER_TOO_SMALL(108),
	// ERROR_X509_FATAL_ERROR(109),
	// ERROR_SSL_FEATURE_UNAVAILABLE(120),
	// ERROR_SSL_BAD_INPUT_DATA(121),
	// ERROR_SSL_INVALID_MAC(122),
	// ERROR_SSL_INVALID_RECORD(123),
	// ERROR_SSL_CONN_EOF(124),
	// ERROR_SSL_UNKNOWN_CIPHER(125),
	// ERROR_SSL_NO_CIPHER_CHOSEN(126),
	// ERROR_SSL_NO_RNG(127),
	// ERROR_SSL_NO_CLIENT_CERTIFICATE(128),
	// ERROR_SSL_CERTIFICATE_TOO_LARGE(129),
	// ERROR_SSL_CERTIFICATE_REQUIRED(130),
	// ERROR_SSL_PRIVATE_KEY_REQUIRED(131),
	// ERROR_SSL_CA_CHAIN_REQUIRED(132),
	// ERROR_SSL_UNEXPECTED_MESSAGE(133),
	// ERROR_SSL_FATAL_ALERT_MESSAGE(134),
	// ERROR_SSL_PEER_VERIFY_FAILED(135),
	// ERROR_SSL_PEER_CLOSE_NOTIFY(136),
	// ERROR_SSL_BAD_HS_CLIENT_HELLO(137),
	// ERROR_SSL_BAD_HS_SERVER_HELLO(138),
	// ERROR_SSL_BAD_HS_CERTIFICATE(139),
	// ERROR_SSL_BAD_HS_CERTIFICATE_REQUEST(140),
	// ERROR_SSL_BAD_HS_SERVER_KEY_EXCHANGE(141),
	// ERROR_SSL_BAD_HS_SERVER_HELLO_DONE(142),
	// ERROR_SSL_BAD_HS_CLIENT_KEY_EXCHANGE(143),
	// ERROR_SSL_BAD_HS_CLIENT_KEY_EXCHANGE_RP(144),
	// ERROR_SSL_BAD_HS_CLIENT_KEY_EXCHANGE_CS(145),
	// ERROR_SSL_BAD_HS_CERTIFICATE_VERIFY(146),
	// ERROR_SSL_BAD_HS_CHANGE_CIPHER_SPEC(147),
	// ERROR_SSL_BAD_HS_FINISHED(148),
	// ERROR_SSL_ALLOC_FAILED(149),
	// ERROR_SSL_HW_ACCEL_FAILED(LifxOTAService.StatusUpdateError),
	// ERROR_SSL_HW_ACCEL_FALLTHROUGH(151),
	// ERROR_SSL_COMPRESSION_FAILED(152),
	// ERROR_SSL_BAD_HS_PROTOCOL_VERSION(153),
	// ERROR_SSL_BAD_HS_NEW_SESSION_TICKET(154),
	// ERROR_SSL_SESSION_TICKET_EXPIRED(155),
	// ERROR_SSL_PK_TYPE_MISMATCH(156),
	// ERROR_SSL_UNKNOWN_IDENTITY(157),
	// ERROR_SSL_INTERNAL_ERROR(158),
	// ERROR_SSL_COUNTER_WRAPPING(159),
	// ERROR_SSL_WAITING_SERVER_HELLO_RENEGO(160),
	// ERROR_SSL_HELLO_VERIFY_REQUIRED(161),
	// ERROR_SSL_BUFFER_TOO_SMALL(162),
	// ERROR_SSL_NO_USABLE_CIPHERSUITE(163),
	// ERROR_SSL_WANT_READ(164),
	// ERROR_SSL_WANT_WRITE(165),
	// ERROR_SSL_TIMEOUT(166),
	// ERROR_SSL_CLIENT_RECONNECT(167),
	// ERROR_SSL_UNEXPECTED_RECORD(168),
	// ERROR_SSL_NON_FATAL(169),
	// ERROR_SSL_INVALID_VERIFY_HASH(170),
	// ERROR_NET_SOCKET_FAILED(180),
	// ERROR_NET_CONNECT_FAILED(181),
	// ERROR_NET_BIND_FAILED(182),
	// ERROR_NET_LISTEN_FAILED(183),
	// ERROR_NET_ACCEPT_FAILED(184),
	// ERROR_NET_RECV_FAILED(185),
	// ERROR_NET_SEND_FAILED(186),
	// ERROR_NET_CONN_RESET(187),
	// ERROR_NET_UNKNOWN_HOST(188),
	// ERROR_NET_BUFFER_TOO_SMALL(189),
	// ERROR_NET_INVALID_CONTEXT(190),
	// ERROR_NET_SELECT_FAILED(191),
	// ERROR_NET_SETSOCKOPT_FAILED(192),
	// ERROR_ESSL_INVAL(LifxOTAService.StatusSuccessThreshold),
	// ERROR_ESSL_NOSOCKET(LifxOTAService.StatusUpdateSuccessful),
	// ERROR_ESSL_HSNOTDONE(202),
	// ERROR_ESSL_HSDONE(203),
	// ERROR_ESSL_NOMEM(204),
	// ERROR_ESSL_CONN(205),
	// ERROR_ESSL_CERT(206),
	// ERROR_ESSL_ALERTRECV(207),
	// ERROR_ESSL_ALERTFATAL(208),
	// ERROR_ESSL_TIMEOUT(209),
	// ERROR_ESSL_NOT_SSL(210),
	// ERROR_ESSL_TRUST_CERTCN(211),
	// ERROR_ESSL_TRUST_CERTTIME(212),
	// ERROR_ESSL_TRUST_CERT(213),
	// ERROR_ESSL_TRUST_NONE(214);
	/// </summary>
	public class StateWanResponse : LifxResponse {
		public byte State { get; }

		internal StateWanResponse(LifxPacket packet) : base(packet) {
			State = packet.Payload.GetUint8();
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
		public ushort Hue { get; }

		/// <summary>
		/// Bulb color temperature
		/// </summary>
		public ushort Kelvin { get; }

		/// <summary>
		/// Saturation (0=desaturated, 65535 = fully saturated)
		/// </summary>
		public ushort Saturation { get; }

		internal LightStateResponse(LifxPacket packet) : base(packet) {
			Hue = packet.Payload.GetUInt16();
			Saturation = packet.Payload.GetUInt16();
			Brightness = packet.Payload.GetUInt16();
			Kelvin = packet.Payload.GetUInt16();
			IsOn = packet.Payload.GetUInt16() > 0;
			Label = packet.Payload.GetString(32).Replace("\\0", "");
		}
	}

	internal class LightPowerResponse : LifxResponse {
		public bool IsOn { get; }

		internal LightPowerResponse(LifxPacket packet) : base(packet) {
			IsOn = packet.Payload.GetUInt16() > 0;
		}
	}

	internal class InfraredStateResponse : LifxResponse {
		public ushort Brightness { get; }

		internal InfraredStateResponse(LifxPacket packet) : base(
			packet) {
			Brightness = packet.Payload.GetUInt16();
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

		internal StateVersionResponse(LifxPacket packet) : base(
			packet) {
			Vendor = packet.Payload.GetUInt32();
			Product = packet.Payload.GetUInt32();
			Version = packet.Payload.GetUInt32();
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

		/// <summary>
		/// Firmware version
		/// </summary>
		public uint Version { get; }

		internal StateHostFirmwareResponse(LifxPacket packet) : base(
			packet) {
			var nanoseconds = packet.Payload.GetUInt64();
			Build = Utilities.Epoch.AddMilliseconds(nanoseconds * 0.000001);
			//8..15 UInt64 is reserved
			Version = packet.Payload.GetUInt32();
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

		internal StateRelayPowerResponse(LifxPacket packet) : base(
			packet) {
			RelayIndex = packet.Payload.GetUint8();
			Level = packet.Payload.GetUInt16();
		}
	}

	public class UnknownResponse : LifxResponse {
		public Payload MsgPayload;

		internal UnknownResponse(LifxPacket packet) : base(packet) {
			MsgPayload = packet.Payload;
		}
	}
}