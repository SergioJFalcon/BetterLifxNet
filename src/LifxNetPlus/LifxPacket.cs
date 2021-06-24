using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LifxNetPlus {
	/// <summary>
	/// Lifx Packet Structure
	/// </summary>
	[Serializable]
	public class LifxPacket {
		/// <summary>
		/// Acknowledgement required
		/// </summary>
		public bool AcknowledgeRequired { get; set; }
		/// <summary>
		/// Addressable
		/// </summary>
		public bool Addressable { get; set; }
		/// <summary>
		/// Response required
		/// </summary>
		public bool ResponseRequired { get; set; }
		/// <summary>
		/// Packet is tagged
		/// </summary>
		public bool Tagged { get; set; }
		/// <summary>
		/// Origin
		/// </summary>
		public byte Origin { get; set; }
		/// <summary>
		/// Sequence
		/// </summary>
		public byte Sequence { get; set; }
		/// <summary>
		/// Target
		/// </summary>
		public byte[] Target { get; set; }
		/// <summary>
		/// Message time
		/// </summary>
		public DateTime AtTime { get; set; }
		/// <summary>
		/// Message Type
		/// </summary>
		public MessageType Type { get; set; }

		/// <summary>
		/// The payload
		/// </summary>
		public Payload Payload { get; set; }

		/// <summary>
		/// Target mac address as a string
		/// </summary>
		public string TargetMacAddressName {
			get { return string.Join(":", Target.Take(6).Select(tb => tb.ToString("X2")).ToArray()); }
		}

		/// <summary>
		/// Source identifier
		/// </summary>
		public uint Source { get; set; }

		/// <summary>
		/// Message size
		/// </summary>
		public ushort Size { get; set; }

		/// <summary>
		/// Construct a packet from incoming data
		/// </summary>
		/// <param name="data"></param>
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
			reader.ReadUInt16(); // Reserved!
			Payload = new Payload();
			var len = ms.Capacity - ms.Position;
			if (len > 0) {
				Payload = new Payload(reader.ReadBytes((int) len));
			}
		}

		/// <summary>
		/// Create a lifx packet from an existing packet
		/// </summary>
		/// <param name="lifxPacket"></param>
		public LifxPacket(LifxPacket lifxPacket) {
			Size = lifxPacket.Size;
			Addressable = lifxPacket.Addressable;
			Tagged = lifxPacket.Tagged;
			Origin = lifxPacket.Origin;
			Source = lifxPacket.Source;
			Target = lifxPacket.Target;
			ResponseRequired = lifxPacket.ResponseRequired;
			AcknowledgeRequired = lifxPacket.AcknowledgeRequired;
			Sequence = lifxPacket.Sequence;
			Type = lifxPacket.Type;
			AtTime = lifxPacket.AtTime;
			Payload = lifxPacket.Payload;
			if (Size == 0) {
				Size = (ushort) (Payload.ToArray().Length + 36);
			}
		}


		/// <summary>
		/// Create a new Lifx Packet using a type and array of argument objects
		/// </summary>
		/// <param name="type">Message Type</param>
		/// <param name="args">Argument array</param>
		public LifxPacket(MessageType type, params object[] args) {
			Source = MessageId.GetSource();
			Sequence = MessageId.GetNextSequence();
			Type = type;
			Target = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
			Payload = new Payload(args);
			Size = (ushort) (Payload.ToArray().Length + 36);
		}


		/// <summary>
		///     Return our packet as a series of bytes
		/// </summary>
		/// <returns></returns>
		public byte[] Encode() {
			Addressable = true;
			// Frame header
			Size = (ushort) (Payload.ToArray().Length + 36);
			var bytes = new List<byte>(Size);
			bytes.AddRange(BitConverter.GetBytes(Size));
			var proto = BitConverter.GetBytes((ushort) 1024);
			var proto2 = proto;
			proto2 = SetBit(proto2, 12, Addressable);
			proto2 = SetBit(proto2, 13, Tagged);
			proto2 = SetBit(proto2, 14, Origin == 1);

			bytes.AddRange(proto2);
			bytes.AddRange(BitConverter.GetBytes(Source));

			// Frame address

			// Pad target address if only 6 bytes
			var tList = Target.ToList();
			if (Target.Length == 6) {
				tList.AddRange(new byte[] {0, 0});
			}

			bytes.AddRange(tList);
			bytes.AddRange(new byte[] {0, 0, 0, 0, 0, 0}); // Reserved 1
			var resByte = (byte) 0; // Reserved 2
			resByte = SetBit(resByte, 0, ResponseRequired);
			resByte = SetBit(resByte, 1, AcknowledgeRequired);
			bytes.Add(resByte);
			bytes.Add(Sequence);
			AtTime = DateTime.Now;
			bytes.AddRange(new byte[] {0, 0, 0, 0, 0, 0, 0, 0}); // Reserved 2

			bytes.AddRange(BitConverter.GetBytes((ushort) Type));
			bytes.AddRange(new byte[] {0, 0}); // Reserved 3

			bytes.AddRange(Payload.ToArray());
			return bytes.ToArray();
		}

		/// <summary>
		///     Encode the Packet to a string
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