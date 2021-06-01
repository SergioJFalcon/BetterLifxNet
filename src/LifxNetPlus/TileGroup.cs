using System;
using System.Collections.Generic;

namespace LifxNetPlus {
	/// <summary>
	/// Class representation of a Tile Response
	/// </summary>
	[Serializable]
	public class Tile {
		/// <summary>
		/// Height
		/// </summary>
		public byte Height { get; set; }
		/// <summary>
		/// Width
		/// </summary>
		public byte Width { get; set; }
		/// <summary>
		/// X position
		/// </summary>
		public float UserX { get; set; }
		/// <summary>
		/// Y position
		/// </summary>
		public float UserY { get; set; }
		/// <summary>
		/// X rotation
		/// </summary>
		public short AccelMeasX { get; set; }
		/// <summary>
		/// Y rotation
		/// </summary>
		public short AccelMeasY { get; set; }
		/// <summary>
		/// Z rotition
		/// </summary>
		public short AccelMeasZ { get; set; }
		/// <summary>
		/// Product Version
		/// </summary>
		public uint DeviceVersionProduct { get; set; }
		/// <summary>
		/// Vendor
		/// </summary>
		public uint DeviceVersionVendor { get; set; }
		/// <summary>
		/// Device Version
		/// </summary>
		public uint DeviceVersionVersion { get; set; }
		/// <summary>
		/// Firmware build number
		/// </summary>
		public ulong FirmwareBuild { get; set; }
		/// <summary>
		/// Major FW Version
		/// </summary>
		public ushort FirmwareVersionMajor { get; set; }
		/// <summary>
		/// Minor FW Version
		/// </summary>
		public ushort FirmwareVersionMinor { get; set; }

		/// <summary>
		/// Create a new empty tile with defaults initialized
		/// </summary>
		/// <param name="index"></param>
		public void CreateDefault(int index) {
			AccelMeasX = 0;
			AccelMeasY = 0;
			AccelMeasZ = 0;
			UserX = index * .5f;
			UserY = 8.06f;
			Width = 8;
			Height = 8;
			DeviceVersionProduct = 55;
			DeviceVersionVendor = 1;
			DeviceVersionVersion = 10;
			FirmwareBuild = 1532997580;
			FirmwareVersionMajor = 50;
			FirmwareVersionMinor = 3;
		}

		/// <summary>
		///     Read payload into tile
		/// </summary>
		/// <param name="payload"></param>
		public void LoadBytes(Payload payload) {
			AccelMeasX = payload.GetInt16();
			AccelMeasY = payload.GetInt16();
			AccelMeasZ = payload.GetInt16();
			payload.Advance(2);
			UserX = payload.GetFloat32();
			UserY = payload.GetFloat32();
			Width = payload.GetUint8();
			Height = payload.GetUint8();
			payload.Advance();
			DeviceVersionVendor = payload.GetUInt32();
			DeviceVersionProduct = payload.GetUInt32();
			DeviceVersionVersion = payload.GetUInt32();
			FirmwareBuild = payload.GetUInt64();
			payload.Advance(8);
			FirmwareVersionMajor = payload.GetUInt16();
			FirmwareVersionMinor = payload.GetUInt16();
			payload.Advance(4);
		}

		/// <summary>
		/// Serialize Tile data to a byte array
		/// </summary>
		/// <returns></returns>
		public byte[] ToBytes() {
			var output = new List<byte>();
			output.AddRange(BitConverter.GetBytes(AccelMeasX));
			output.AddRange(BitConverter.GetBytes(AccelMeasY));
			output.AddRange(BitConverter.GetBytes(AccelMeasZ));
			output.AddRange(BitConverter.GetBytes((short) 0));
			output.AddRange(BitConverter.GetBytes(UserX));
			output.AddRange(BitConverter.GetBytes(UserY));
			output.Add(Width);
			output.Add(Height);
			output.Add(50); // Reserved
			output.AddRange(BitConverter.GetBytes(DeviceVersionVendor));
			output.AddRange(BitConverter.GetBytes(DeviceVersionProduct));
			output.AddRange(BitConverter.GetBytes(DeviceVersionVersion));
			output.AddRange(BitConverter.GetBytes(FirmwareBuild));
			output.AddRange(BitConverter.GetBytes(FirmwareBuild));
			output.AddRange(BitConverter.GetBytes(FirmwareVersionMajor));
			output.AddRange(BitConverter.GetBytes(FirmwareVersionMinor));
			output.AddRange(BitConverter.GetBytes(uint.MinValue)); // Reserved

			return output.ToArray();
		}
	}
}