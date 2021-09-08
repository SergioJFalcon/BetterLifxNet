using System;
using System.Linq;

namespace LifxNetPlus {
	/// <summary>
	///     LIFX Generic Device
	/// </summary>
	public class Device {
		/// <summary>
		///     Service ID
		/// </summary>
		public byte Service { get; }

		/// <summary>
		///     Gets the MAC address
		/// </summary>
		public byte[] MacAddress { get; }

		/// <summary>
		///     Hostname for the device
		/// </summary>
		public string HostName { get; internal set; }

		/// <summary>
		///     Gets the MAC address
		/// </summary>
		public string MacAddressName {
			get { return string.Join(":", MacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray()); }
		}

		/// <summary>
		///     Service port
		/// </summary>
		public uint Port { get; }
		
		/// <summary>
		/// Product ID
		/// </summary>
		public uint ProductId { get; set; }

		/// <summary>
		/// Gets or sets the value of the last seen
		/// </summary>
		internal DateTime LastSeen { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Device"/> class
		/// </summary>
		/// <param name="hostname">The hostname</param>
		/// <param name="macAddress">The mac address</param>
		/// <param name="service">The service</param>
		/// <param name="port">The port</param>
		/// <param name="productId">The product id</param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public Device(string hostname, byte[] macAddress, byte service, uint port, uint productId = 0) {
			if (hostname == null) {
				throw new ArgumentNullException(nameof(hostname));
			}

			if (string.IsNullOrWhiteSpace(hostname)) {
				throw new ArgumentException(nameof(hostname));
			}

			HostName = hostname;
			MacAddress = macAddress;
			Service = service;
			Port = port;
			LastSeen = DateTime.MinValue;
			ProductId = productId;
		}
	}
}