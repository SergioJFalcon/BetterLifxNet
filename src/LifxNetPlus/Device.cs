using System;
using System.Linq;

namespace LifxNetPlus {
	/// <summary>
	/// LIFX Generic Device
	/// </summary>
	public abstract class Device {
		/// <summary>
		/// Service ID
		/// </summary>
		public byte Service { get; }

		/// <summary>
		/// Gets the MAC address
		/// </summary>
		public byte[] MacAddress { get; }

		/// <summary>
		/// Hostname for the device
		/// </summary>
		public string HostName { get; internal set; }

		/// <summary>
		/// Gets the MAC address
		/// </summary>
		public string MacAddressName {
			get { return string.Join(":", MacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray()); }
		}

		/// <summary>
		/// Service port
		/// </summary>
		public uint Port { get; }

		internal DateTime LastSeen { get; set; }

		internal Device(string hostname, byte[] macAddress, byte service, uint port) {
			if (hostname == null)
				throw new ArgumentNullException(nameof(hostname));
			if (string.IsNullOrWhiteSpace(hostname))
				throw new ArgumentException(nameof(hostname));
			HostName = hostname;
			MacAddress = macAddress;
			Service = service;
			Port = port;
			LastSeen = DateTime.MinValue;
		}
	}
}