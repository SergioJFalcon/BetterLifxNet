using System;
using LifxNetPlus;
using Console = Colorful.Console;

namespace LifxEmulator {
	public class DeviceData {
		public string Label { get; set; }
		public byte[] Group { get; set; }
		public ulong GroupUpdated { get; set; }
		public string GroupLabel { get; set; }
		public byte[] Location { get; set; }
		public string LocationLabel { get; set; }
		public ulong LocationUpdated { get; set; }
		public ushort PowerLevel { get; set; }
		public LifxColor Color { get; set; }
		public LifxColor[] ColorArray { get; set; }
		public LifxPacket OutPacket { get; set; }
		
		public Product ProductData { get; set; }
		public uint MultizoneId { get; set; }
		public byte EffectType { get; set; }
		public uint EffectSpeed { get; set; }
		public ulong EffectDur { get; set; }
		public byte[] EffectParam { get; set; }
		
		public byte[] OwnerPacket { get; set; }

		public DeviceData() {
			Color = new LifxColor(255, 0, 0);
			Console.WriteLine("Color created...");
			Label = "Test Light";
			PowerLevel = 65535;
			var rand = new Random();
			var location = new byte[16];
			rand.NextBytes(location);
			Location = location;
			LocationLabel = "Emu room";
			LocationUpdated = 0;
			var group = new byte[16];
			rand.NextBytes(group);
			Group = group;
			GroupLabel = "Emu group";
			GroupUpdated = 0;
			var thirtyBits = (uint) rand.Next(1 << 30);
			var twoBits = (uint) rand.Next(1 << 2);
			MultizoneId = (thirtyBits << 2) | twoBits;
			EffectType = 0;
			EffectSpeed = 0;
			EffectDur = 0;
			EffectParam = new byte[]
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
			OwnerPacket = new byte[] {
				0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC0,0xDF,0x9E,0xF3,0x00,0x95
			};
		}
	}
}