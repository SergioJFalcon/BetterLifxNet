using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LifxNetPlus {
	/// <summary>
	/// Base class for LIFX response types
	/// </summary>
	public abstract class LifxResponse {
		internal static LifxResponse Create(FrameHeader header, MessageType type, uint source, Payload payload) {
			return type switch {
				MessageType.DeviceAcknowledgement => new AcknowledgementResponse(header, type, payload, source),
				MessageType.DeviceStateLabel => new StateLabelResponse(header, type, payload, source),
				MessageType.LightState => new LightStateResponse(header, type, payload, source),
				MessageType.LightStatePower => new LightPowerResponse(header, type, payload, source),
				MessageType.LightStateInfrared => new InfraredStateResponse(header, type, payload, source),
				MessageType.DeviceStateVersion => new StateVersionResponse(header, type, payload, source),
				MessageType.DeviceStateHostFirmware => new StateHostFirmwareResponse(header, type, payload, source),
				MessageType.DeviceStateService => new StateServiceResponse(header, type, payload, source),
				MessageType.StateExtendedColorZones => new StateExtendedColorZonesResponse(header, type, payload,
					source),
				MessageType.StateZone => new StateZoneResponse(header, type, payload, source),
				MessageType.StateMultiZone => new StateMultiZoneResponse(header, type, payload, source),
				MessageType.StateDeviceChain => new StateDeviceChainResponse(header, type, payload, source),
				MessageType.StateTileState64 => new StateTileState64Response(header, type, payload, source),
				MessageType.StateRelayPower => new StateRelayPowerResponse(header, type, payload, source),
				MessageType.DeviceStateHostInfo => new StateHostInfoResponse(header, type, payload, source),
				MessageType.DeviceStateWifiInfo => new StateWifiInfoResponse(header, type, payload, source),
				MessageType.DeviceStateWifiFirmware => new StateWifiFirmwareResponse(header, type, payload, source),
				MessageType.DeviceStatePower => new StatePowerResponse(header, type, payload, source),
				MessageType.DeviceStateInfo => new StateInfoResponse(header, type, payload, source),
				MessageType.DeviceStateLocation => new StateLocationResponse(header, type, payload, source),
				MessageType.DeviceStateGroup => new StateGroupResponse(header, type, payload, source),
				MessageType.DeviceEchoResponse => new EchoResponse(header, type, payload, source),
				MessageType.DeviceStateOwner => new StateOwnerResponse(header, type, payload, source),
				MessageType.WanState => new StateWanResponse(header, type, payload, source),
				MessageType.StateTileTapConfig => new StateTileTapConfigResponse(header,type,payload,source),
				MessageType.TileStateEffect => new StateTileEffectResponse(header, type, payload, source),
				_ => new UnknownResponse(header, type, payload, source)
			};
		}
		
		internal static LifxResponse CreateFromPacket(LifxPacket packet) {
			return packet.LifxProtocolHeader.Type switch {
				MessageType.DeviceAcknowledgement => new AcknowledgementResponse(header, type, payload, source),
				MessageType.DeviceStateLabel => new StateLabelResponse(header, type, payload, source),
				MessageType.LightState => new LightStateResponse(header, type, payload, source),
				MessageType.LightStatePower => new LightPowerResponse(header, type, payload, source),
				MessageType.LightStateInfrared => new InfraredStateResponse(header, type, payload, source),
				MessageType.DeviceStateVersion => new StateVersionResponse(header, type, payload, source),
				MessageType.DeviceStateHostFirmware => new StateHostFirmwareResponse(header, type, payload, source),
				MessageType.DeviceStateService => new StateServiceResponse(header, type, payload, source),
				MessageType.StateExtendedColorZones => new StateExtendedColorZonesResponse(header, type, payload,
					source),
				MessageType.StateZone => new StateZoneResponse(header, type, payload, source),
				MessageType.StateMultiZone => new StateMultiZoneResponse(header, type, payload, source),
				MessageType.StateDeviceChain => new StateDeviceChainResponse(header, type, payload, source),
				MessageType.StateTileState64 => new StateTileState64Response(header, type, payload, source),
				MessageType.StateRelayPower => new StateRelayPowerResponse(header, type, payload, source),
				MessageType.DeviceStateHostInfo => new StateHostInfoResponse(header, type, payload, source),
				MessageType.DeviceStateWifiInfo => new StateWifiInfoResponse(header, type, payload, source),
				MessageType.DeviceStateWifiFirmware => new StateWifiFirmwareResponse(header, type, payload, source),
				MessageType.DeviceStatePower => new StatePowerResponse(header, type, payload, source),
				MessageType.DeviceStateInfo => new StateInfoResponse(header, type, payload, source),
				MessageType.DeviceStateLocation => new StateLocationResponse(header, type, payload, source),
				MessageType.DeviceStateGroup => new StateGroupResponse(header, type, payload, source),
				MessageType.DeviceEchoResponse => new EchoResponse(header, type, payload, source),
				MessageType.DeviceStateOwner => new StateOwnerResponse(header, type, payload, source),
				MessageType.WanState => new StateWanResponse(header, type, payload, source),
				MessageType.StateTileTapConfig => new StateTileTapConfigResponse(header,type,payload,source),
				MessageType.TileStateEffect => new StateTileEffectResponse(header, type, payload, source),
				_ => new UnknownResponse(header, type, payload, source)
			};
		}

		internal LifxResponse(FrameHeader header, MessageType type, Payload payload, uint source) {
			Header = header;
			Type = type;
			Payload = payload;
			Source = source;
		}

		internal FrameHeader Header { get; }
		internal Payload Payload { get; }
		internal MessageType Type { get; }
		internal uint Source { get; }
	}

	/// <summary>
	/// Response to any message sent with ack_required set to 1. 
	/// </summary>
	internal class AcknowledgementResponse : LifxResponse {
		internal AcknowledgementResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
		}
	}
	
	/// <summary>
	/// State Tile Tap Config
	/// </summary>
	internal class StateTileTapConfigResponse : LifxResponse {
		internal StateTileTapConfigResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
		}
	}
	
	/// <summary>
	/// Response to a state tile get request 
	/// </summary>
	internal class StateTileEffectResponse : LifxResponse {
		internal StateTileEffectResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
		}
	}

	/// <summary>
	/// The StateZone message represents the state of a single zone with the index field indicating which zone is represented. The count field contains the count of the total number of zones available on the device.
	/// </summary>
	public class StateZoneResponse : LifxResponse {
		internal StateZoneResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Count = payload.GetUInt16();
			Index = payload.GetUInt16();
			Color = new Payload().GetColor();
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
		public LifxColor Color { get; }
	}
	
	
	/// <summary>
	/// Response to GetHostInfo message.
	/// Provides host MCU information.
	/// </summary>
	public class StateHostInfoResponse : LifxResponse {
		internal StateHostInfoResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Signal = payload.GetFloat32();
			Tx = payload.GetUInt32();
			Rx = payload.GetUInt32();
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
	/// Response to GetWifiInfo message.
	/// Provides host Wifi information.
	/// </summary>
	public class StateWifiInfoResponse : LifxResponse {
		internal StateWifiInfoResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Signal = payload.GetFloat32();
			Tx = payload.GetUInt32();
			Rx = payload.GetUInt32();
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
		internal StateWifiFirmwareResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Build = payload.GetUInt64();
			// Skip 64-bit reserved
			payload.Advance(8);
			VersionMinor = payload.GetUInt16();
			VersionMajor = payload.GetUInt16();
		}
		
		/// <summary>
		/// Firmware build time (epoch time)
		/// </summary>
		public ulong Build { get; set; }
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
	/// Provides device power level.
	/// </summary>
	public class StatePowerResponse : LifxResponse {
		internal StatePowerResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Level = payload.GetUInt16();
		}
		
		/// <summary>
		/// Zero implies standby and non-zero sets a corresponding power draw level. Currently only 0 and 65535 are supported.
		/// </summary>
		public ulong Level { get; set; }
		
	}
	
	
	/// <summary>
	/// Provides run-time information of device.
	/// </summary>
	public class StateInfoResponse : LifxResponse {
		internal StateInfoResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Time = DateTimeOffset.FromUnixTimeSeconds(payload.GetInt64()).DateTime;
			Uptime = payload.GetInt64();
			Downtime = payload.GetInt64();
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
	/// Device location.
	/// </summary>
	public class StateLocationResponse : LifxResponse {
		internal StateLocationResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Location = payload.GetBytes(16);
			Label = payload.GetString(32);
			Updated = payload.GetUInt64();
		}

		public byte[] Location { get; set; }

		public string Label { get; set; }

		public ulong Updated { get; set; }
	}
	
	/// <summary>
	/// Device group.
	/// </summary>
	public class StateGroupResponse : LifxResponse {
		internal StateGroupResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Group = payload.GetBytes(16);
			Label = payload.GetString(32);
			Updated = payload.GetUInt64();
		}

		public byte[] Group { get; set; }

		public string Label { get; set; }

		public ulong Updated { get; set; }
	}
	
	/// <summary>
	/// Echo response with payload sent in the EchoRequest.
	/// </summary>
	public class EchoResponse : LifxResponse {
		internal EchoResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			RequestPayload = payload.ToArray();
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
		internal StateDeviceChainResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header,
			type, payload, source) {
			Tiles = new List<Tile>();
			StartIndex = payload.GetUint8();
			for (var i = 0; i < 16; i++) {
				var tile = new Tile();
				tile.LoadBytes(payload);
				Tiles.Add(tile);
			}
			TotalCount = payload.GetUint8();
		}

		/// <summary>
		/// Count - total number of zones on the device
		/// </summary>
		public int TotalCount { get; }

		/// <summary>
		/// Start Index - Zone the message starts from
		/// </summary>
		public byte StartIndex { get; }

		/// <summary>
		/// The list of colors returned by the message
		/// </summary>
		public List<Tile> Tiles { get; }
	}

	/// <summary>
	/// Get the list of colors currently being displayed by zones
	/// </summary>
	public class StateMultiZoneResponse : LifxResponse {
		internal StateMultiZoneResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
			Colors = new LifxColor[8];
			Count = payload.GetUint8();
			Index = payload.GetUint8();
			for (var i = 0; i < 8; i++) {
				Debug.WriteLine($"Reading color {i}.");
				Colors[i] = payload.GetColor();
			}
			Debug.WriteLine("Colors read.");
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


	/// <summary>
	/// Get the list of colors currently being displayed by zones
	/// </summary>
	public class StateExtendedColorZonesResponse : LifxResponse {
		internal StateExtendedColorZonesResponse(FrameHeader header, MessageType type, Payload payload, uint source) :
			base(header, type, payload, source) {
			Colors = new List<LifxColor>();
			Count = payload.GetUInt16();
			Index = payload.GetUInt16();
			while (payload.HasContent()) {
				Colors.Add(payload.GetColor());
			}
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
	/// Response to GetService message.
	/// Provides the device Service and port.
	/// If the Service is temporarily unavailable, then the port value will be 0.
	/// </summary>
	internal class StateServiceResponse : LifxResponse {
		internal StateServiceResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
			Service = payload.GetUint8();
			Port = payload.GetUInt32();
		}

		private byte Service { get; }
		private uint Port { get; }
	}

	/// <summary>
	/// Response to any message sent with ack_required set to 1. 
	/// </summary>
	public class StateTileState64Response : LifxResponse {
		internal StateTileState64Response(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
			TileIndex = payload.GetUint8();
			// Skip one byte for reserved
			payload.Advance();
			X = payload.GetUint8();
			Y = payload.GetUint8();
			Width = payload.GetUint8();
			Colors = new LifxColor[64];
			for (var i = 0; i < Colors.Length; i++) {
				if (payload.HasContent()) {
					Colors[i] = payload.GetColor();
				} else {
					Debug.WriteLine($"Content size mismatch fetching colors: {i}/64: ");
				}
			}
		}

		public uint TileIndex { get; }
		public uint X { get; }
		public uint Y { get; }
		public uint Width { get; }
		public LifxColor[] Colors { get; }
	}

	/// <summary>
	/// Response to GetLabel message. Provides device label.
	/// </summary>
	internal class StateLabelResponse : LifxResponse {
		internal StateLabelResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Label = payload.GetString().Replace("\0", "");
		}

		public string? Label { get; }
	}
	
	/// <summary>
	/// Response to GetLabel message. Provides device label.
	/// </summary>
	public class StateOwnerResponse : LifxResponse {
		internal StateOwnerResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Owner = payload.GetString(16);
			Label = payload.GetString(16);
			Updated = payload.GetUInt64();
		}

		public string Owner { get; }
		public string Label { get; }
		public ulong Updated { get; }
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
		internal StateWanResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			State = payload.GetUint8();
		}

		public byte State { get; }
	}

	/// <summary>
	/// Sent by a device to provide the current light state
	/// </summary>
	public class LightStateResponse : LifxResponse {
		internal LightStateResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			Hue = payload.GetUInt16();
			Saturation = payload.GetUInt16();
			Brightness = payload.GetUInt16();
			Kelvin = payload.GetUInt16();
			IsOn = payload.GetUInt16() > 0;
			Label = payload.GetString(32).Replace("\\0", "");
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

	internal class LightPowerResponse : LifxResponse {
		internal LightPowerResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			IsOn = payload.GetUInt16() > 0;
		}

		public bool IsOn { get; }
	}

	internal class InfraredStateResponse : LifxResponse {
		internal InfraredStateResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
			Brightness = payload.GetUInt16();
		}

		public ushort Brightness { get; }
	}

	/// <summary>
	/// Response to GetVersion message.	Provides the hardware version of the device.
	/// </summary>
	public class StateVersionResponse : LifxResponse {
		internal StateVersionResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
			Vendor = Payload.GetUInt32();
			Product = Payload.GetUInt32();
			Version = Payload.GetUInt32();
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
		internal StateHostFirmwareResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
			var nanoseconds = payload.GetUInt64();
			Build = Utilities.Epoch.AddMilliseconds(nanoseconds * 0.000001);
			//8..15 UInt64 is reserved
			Version = payload.GetUInt32();
		}

		/// <summary>
		/// Firmware build time
		/// </summary>
		public DateTime Build { get; }

		/// <summary>
		/// Firmware version
		/// </summary>
		public uint Version { get; }
	}

	/// <summary>
	/// Response to GetVersion message.	Provides the hardware version of the device.
	/// </summary>
	public class StateRelayPowerResponse : LifxResponse {
		internal StateRelayPowerResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(
			header, type, payload, source) {
			RelayIndex = payload.GetUint8();
			Level = payload.GetUInt16();
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

	public class UnknownResponse : LifxResponse {
		internal UnknownResponse(FrameHeader header, MessageType type, Payload payload, uint source) : base(header,
			type, payload, source) {
			MsgPayload = payload;
		}

		public Payload MsgPayload;
	}
}