using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LifxNetPlus {
	[Serializable]
	public class LifxPacket {
		public bool AcknowledgeRequired { get; set; }
		public bool Addressable { get; set; }
		public bool ResponseRequired { get; set; }
		public bool Tagged { get; set; }
		public byte Origin { get; set; }
		public byte Sequence { get; set; }
		public byte[] Target { get; set; }
		public DateTime AtTime { get; set; }
		public MessageType Type { get; set; }
		public Payload Payload { get; set; }

		public string TargetMacAddressName {
			get { return string.Join(":", Target.Take(6).Select(tb => tb.ToString("X2")).ToArray()); }
		}

		public uint Identifier { get; set; }
		public uint Source { get; set; }
		public ushort Size { get; set; }
		public const ushort Protocol = 1024;

		public LifxPacket(byte[] data) {
			MemoryStream ms = new MemoryStream(data);
			var reader = new BinaryReader(ms);
			Size = reader.ReadUInt16();
			var protoString = reader.ReadBytes(2);
			Addressable = GetBit(protoString, 12);
			Tagged = GetBit(protoString, 13);
			Origin = GetBit(protoString, 14) ? (byte) 1 : (byte) 0;
			Source = reader.ReadUInt32();
			Target = reader.ReadBytes(8);
			reader.ReadBytes(6); // Reserved
			var resByte = reader.ReadByte();
			ResponseRequired = GetBit(resByte, 0);
			AcknowledgeRequired = GetBit(resByte, 1);
			Sequence = reader.ReadByte();
			var nanoseconds = reader.ReadUInt64();
			AtTime = Utilities.Epoch.AddMilliseconds(nanoseconds * 0.000001);
			Type = (MessageType) reader.ReadUInt16();
			Identifier = Source;
			Payload = new Payload();
			if (Size > 36) {
				Payload = new Payload(reader.ReadBytes(Size - 36));
			}
		}

		public LifxPacket(LifxPacket lifxPacket) {
			Size = lifxPacket.Size;
			Addressable = lifxPacket.Addressable;
			Tagged = lifxPacket.Tagged;
			Origin = lifxPacket.Origin;
			Source = lifxPacket.Source;
			Identifier = Source;
			Target = lifxPacket.Target;
			ResponseRequired = lifxPacket.ResponseRequired;
			AcknowledgeRequired = lifxPacket.AcknowledgeRequired;
			Sequence = lifxPacket.Sequence;
			Type = lifxPacket.Type;
			AtTime = lifxPacket.AtTime;
			Payload = lifxPacket.Payload;
		}

		public LifxPacket(uint id, MessageType type, params object[] args) {
			Identifier = MessageId.GetNextIdentifier();
			Source = Identifier;
			Type = type;
			if (type == MessageType.DeviceGetService) {
				Tagged = true;
				AtTime = DateTime.MinValue;
				Identifier = id;
				Source = id;
			}

			Target = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
			Payload = new Payload(args);
		}

		public LifxPacket(MessageType type, params object[] args) {
			Identifier = MessageId.GetNextIdentifier();
			Source = Identifier;
			Type = type;
			Target = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
			Payload = new Payload(args);
		}


		/// <summary>
		/// Return our packet as a series of bytes
		/// </summary>
		/// <returns></returns>
		public byte[] Encode() {
			Size = (ushort) (Payload.Length + 36);
			Tagged = Target[0] == 0;
			Addressable = true;
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(Size));
			var proto = BitConverter.GetBytes((ushort) 1024);
			proto = SetBit(proto, 12, Addressable);
			proto = SetBit(proto, 13, Tagged);
			proto = SetBit(proto, 14, Origin == 1);
			bytes.AddRange(proto);
			bytes.AddRange(BitConverter.GetBytes(Source));
			// Pad target address if only 6 bytes
			var tList = Target.ToList();
			if (Target.Length == 6) {
				tList.AddRange(new byte[] {0, 0});
			}

			bytes.AddRange(tList);
			bytes.AddRange(new byte[] {0, 0, 0, 0, 0, 0}); // Reserved
			var resByte = (byte) 0;
			resByte = SetBit(resByte, 0, ResponseRequired);
			resByte = SetBit(resByte, 1, AcknowledgeRequired);
			bytes.Add(resByte);
			bytes.Add(Sequence);
			AtTime = DateTime.Now;
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			var tLong = Convert.ToInt64((AtTime - epoch).TotalSeconds);
			bytes.AddRange(BitConverter.GetBytes(tLong));
			bytes.AddRange(BitConverter.GetBytes((ushort) Type));
			bytes.AddRange(new byte[] {0, 0}); // Reserved
			bytes.AddRange(Payload.ToArray());
			return bytes.ToArray();
		}

		/// <summary>
		/// Encode the Packet to a string
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			var encoded = Encode();
			return string.Join(",", (from a in encoded select a.ToString("X2")).ToArray());
		}


		private static bool GetBit(byte b, int bitNumber) {
			byte[] bytearray = {b};
			var bitArray = new BitArray(bytearray);
			return bitArray[bitNumber];
		}

		private static bool GetBit(byte[] b, int bitNumber) {
			byte[] bytearray = b;
			var bitArray = new BitArray(bytearray);
			return bitArray[bitNumber];
		}

		private static byte SetBit(byte b, int bitNumber, bool value) {
			byte[] bytearray = {b};
			var bitArray = new BitArray(bytearray);
			bitArray.Set(bitNumber, value);
			bitArray.CopyTo(bytearray, 0);
			return bytearray[0];
		}

		private static byte[] SetBit(byte[] b, int bitNumber, bool value) {
			byte[] bytearray = b;
			var bitArray = new BitArray(bytearray);
			bitArray.Set(bitNumber, value);
			bitArray.CopyTo(bytearray, 0);
			return bytearray;
		}
	}
}