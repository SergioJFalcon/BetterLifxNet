using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LifxNetPlus {
	[Serializable]
	public class LifxPacket {

		public Frame LifxFrame { get; set; }
		public FrameAddress LifxFrameAddress { get; set; }
		public ProtocolHeader LifxProtocolHeader { get; set; }
		public Payload Payload { get; set; }
		

		public LifxPacket(byte[] data) {
			MemoryStream ms = new MemoryStream(data);
			var br = new BinaryReader(ms);
			//Header
			LifxFrame = new Frame(br);
			LifxFrameAddress = new FrameAddress(br);
			LifxProtocolHeader = new ProtocolHeader(br);
			Payload = new Payload();
			if (LifxFrame.Size > 36) {
				Payload = new Payload(br.ReadBytes(LifxFrame.Size - 36));
			}
		}

		public byte[] Encode() {
			var bytes = new List<byte>();
			bytes.AddRange(LifxFrame.Encode());
			bytes.AddRange(LifxFrameAddress.Encode());
			bytes.AddRange(LifxProtocolHeader.Encode());
			return bytes.ToArray();
		}

		public override string ToString() {
			var encoded = Encode();
			return string.Join(",", (from a in encoded select a.ToString("X2")).ToArray());
		}
		
		
		public static bool GetBit(byte b, int bitNumber) {
			byte[] bytearray = {b};
			var bitArray = new BitArray(bytearray);
			return bitArray[bitNumber];
		}
		
		public static bool GetBit(byte[] b, int bitNumber) {
			byte[] bytearray = b;
			var bitArray = new BitArray(bytearray);
			return bitArray[bitNumber];
		}

		public static byte SetBit(byte b, int bitNumber, bool value) {
			byte[] bytearray = {b};
			var bitArray = new BitArray(bytearray);
			bitArray.Set(bitNumber, value);
			bitArray.CopyTo(bytearray, 0);
			return bytearray[0];
		}
		
		public static byte[] SetBit(byte[] b, int bitNumber, bool value) {
			byte[] bytearray = b;
			var bitArray = new BitArray(bytearray);
			bitArray.Set(bitNumber, value);
			bitArray.CopyTo(bytearray, 0);
			return bytearray;
		}


		[Serializable]
		public class Frame {
			public ushort Size { get; }
			public const ushort Protocol = 1024;
			public bool Addressable { get; set; }
			public bool Tagged { get; set; }
			public byte Origin { get; set; }
			public uint Source { get; set; }

			public byte[] TargetMacAddress = {0, 0, 0, 0, 0, 0, 0, 0};
			
			public Frame(BinaryReader reader) {
				Size = reader.ReadUInt16();
				var protoString = reader.ReadBytes(2);
				Addressable = GetBit(protoString, 12);
				Tagged = GetBit(protoString, 13);
				Origin = GetBit(protoString, 14) ? (byte) 1 : (byte) 0;
				Source = reader.ReadUInt32();
			}

			public byte[] Encode() {
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(Size));
				var proto = BitConverter.GetBytes(1024);
				proto = SetBit(proto, 12,Addressable);
				proto = SetBit(proto, 13,Tagged);
				proto = SetBit(proto, 14,Origin == 1);
				bytes.AddRange(proto);
				bytes.AddRange(BitConverter.GetBytes(Source));
				return bytes.ToArray();
			}
			
			public string TargetMacAddressName {
				get { return string.Join(":", TargetMacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray()); }
			}
		}

		[Serializable]
		public class FrameAddress {
			public byte[] Target { get; set; } = {0, 0, 0, 0, 0, 0, 0, 0};
			public bool ResponseRequired { get; set; }
			public bool AcknowledgeRequired { get; set; }
			public byte Sequence { get; set; }
			
			public FrameAddress(BinaryReader reader) {
				Target = reader.ReadBytes(8);
				reader.ReadBytes(6);
				var resByte = reader.ReadByte();
				ResponseRequired = GetBit(resByte, 0);
				AcknowledgeRequired = GetBit(resByte, 1);
				Sequence = reader.ReadByte();
			}

			public byte[] Encode() {
				var bytes = new List<byte>();
				// Pad target address if only 6 bytes
				var tList = Target.ToList();
				if (Target.Length == 6) {
					tList.AddRange(new byte[]{0,0});
				}
				bytes.AddRange(tList);
				bytes.AddRange(new byte[]{0,0,0,0,0,0}); // Reserved
				var resByte = (byte) 0;
				resByte = SetBit(resByte, 0, ResponseRequired);
				resByte = SetBit(resByte, 1, AcknowledgeRequired);
				bytes.Add(resByte);
				bytes.Add(Sequence);
				return bytes.ToArray();
			}
		}

		[Serializable]
		public class ProtocolHeader {
			public MessageType Type { get; set; }
			public DateTime AtTime { get; set; }
			public ProtocolHeader(BinaryReader reader) {
				var nanoseconds = reader.ReadUInt64();
				AtTime = Utilities.Epoch.AddMilliseconds(nanoseconds * 0.000001);
				Type = (MessageType) reader.ReadUInt16();
			}

			public byte[] Encode() {
				var bytes = new List<byte>();
				AtTime = DateTime.Now;
				var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				var tLong = Convert.ToInt64((AtTime - epoch).TotalSeconds);
				bytes.AddRange(BitConverter.GetBytes(tLong));
				bytes.AddRange(new byte[]{0,0,0,0,0,0,0,0}); // Reserved
				bytes.AddRange(BitConverter.GetBytes((ushort)Type));
				bytes.AddRange(new byte[]{0,0}); // Reserved
				return bytes.ToArray();
			}
		}
	}
}