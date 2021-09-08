using System;
using LifxNetPlus;
using Console = Colorful.Console;

namespace LifxEmulator {
	/// <summary>
	/// The device data class
	/// </summary>
	public class DeviceData {
		/// <summary>
		/// Gets or sets the value of the label
		/// </summary>
		public string Label { get; set; }
		/// <summary>
		/// Gets or sets the value of the group
		/// </summary>
		public byte[] Group { get; set; }
		/// <summary>
		/// Gets or sets the value of the group updated
		/// </summary>
		public DateTime GroupUpdated { get; set; }
		/// <summary>
		/// Gets or sets the value of the group label
		/// </summary>
		public string GroupLabel { get; set; }
		/// <summary>
		/// Gets or sets the value of the location
		/// </summary>
		public byte[] Location { get; set; }
		/// <summary>
		/// Gets or sets the value of the location label
		/// </summary>
		public string LocationLabel { get; set; }
		/// <summary>
		/// Gets or sets the value of the location updated
		/// </summary>
		public DateTime LocationUpdated { get; set; }
		/// <summary>
		/// Gets or sets the value of the power level
		/// </summary>
		public ushort PowerLevel { get; set; }
		/// <summary>
		/// Gets or sets the value of the color
		/// </summary>
		public LifxColor Color { get; set; }
		/// <summary>
		/// Gets or sets the value of the color array
		/// </summary>
		public LifxColor[] ColorArray { get; set; }
		/// <summary>
		/// Gets or sets the value of the out packet
		/// </summary>
		public LifxPacket OutPacket { get; set; }
		
		/// <summary>
		/// Gets or sets the value of the product data
		/// </summary>
		public Product ProductData { get; set; }
		/// <summary>
		/// Gets or sets the value of the multizone id
		/// </summary>
		public uint MultizoneId { get; set; }
		/// <summary>
		/// Gets or sets the value of the effect type
		/// </summary>
		public byte EffectType { get; set; }
		/// <summary>
		/// Gets or sets the value of the effect speed
		/// </summary>
		public uint EffectSpeed { get; set; }
		/// <summary>
		/// Gets or sets the value of the effect dur
		/// </summary>
		public ulong EffectDur { get; set; }
		/// <summary>
		/// Gets or sets the value of the effect param
		/// </summary>
		public byte[] EffectParam { get; set; }
		
		/// <summary>
		/// Gets or sets the value of the owner packet
		/// </summary>
		public byte[] OwnerPacket { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceData"/> class
		/// </summary>
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
			LocationUpdated = DateTime.Now;
			var group = new byte[16];
			rand.NextBytes(group);
			Group = group;
			GroupLabel = "Emu group";
			GroupUpdated = DateTime.Now;
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