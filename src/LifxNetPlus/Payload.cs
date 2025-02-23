using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LifxNetPlus {
	/// <summary>
	///     A wrapper class for a byte payload
	///     Any time the payload is read from, our pointer increments
	///     the proper number of bytes until the end is reached,
	///     at which time a message will be logged. Should eventually throw an error or something...
	/// </summary>
	public class Payload {
		/// <summary>
		///     Get the length of the internal byte array
		/// </summary>
		public int Length => _data.Count;

		/// <summary>
		///     Get the current position of the array
		/// </summary>
		public int Position => (int) _ms.Position;

		/// <summary>
		/// Gets or sets the value of the objects
		/// </summary>
		public List<object> Objects { get; set; }

		/// <summary>
		/// The br
		/// </summary>
		private readonly BinaryReader _br;
		/// <summary>
		/// The data
		/// </summary>
		private readonly List<byte> _data;
		/// <summary>
		/// The len
		/// </summary>
		private readonly long _len;
		/// <summary>
		/// The ms
		/// </summary>
		private readonly MemoryStream _ms;


		/// <summary>
		///     Initialize a new, empty payload where we can serialize outgoing data
		/// </summary>
		public Payload() {
			_data = new List<byte>();
			Objects = new List<object>();
			_ms = new MemoryStream(_data.ToArray());
			_len = _ms.Length;
			_br = new BinaryReader(_ms);
		}

		/// <summary>
		///     Create a payload from an array of objects
		/// </summary>
		/// <param name="args"></param>
		/// <exception cref="NotSupportedException"></exception>
		public Payload(object[] args) {
			Objects = args.ToList();
			_data = new List<byte>();
			foreach (var arg in args) {
				switch (arg) {
					case byte b:
						Add(b);
						break;
					case ushort @ushort:
						Add(@ushort);
						break;
					case uint u:
						Add(u);
						break;
					case bool b:
						Add(b ? (byte) 1 : (byte) 0);
						break;
					case byte[] bytes:
						Add(bytes);
						break;
					case string s:
						Add(s);
						break;
					case long l:
						Add(l);
						break;
					case double d:
						Add(d);
						break;
					case float f:
						Add(f);
						break;
					case short s:
						Add(s);
						break;
					case int i:
						Add(i);
						break;
					case ulong u:
						Add(u);
						break;
					case LifxColor c:
						Add(c);
						break;
					case IEnumerable<LifxColor> colors:
						Add(colors);
						break;
					case DateTime dt:
						Add(dt);
						break;
					case Tile t:
						Add(t);
						break;
					case LifxNetPlus.LifxClient.WaveFormType wft:
						Add(wft);
						break;
					// TODO: Add the rest of the enum types
					// Do we use the enums Services, Direction, MultiZoneApplicationRequest, MultiZoneEffectType, 
					// MultiZoneExtendedApplicationRequest, TileEffectSkyType, TileEffectSkyPalette, and LightLastHevCycleResult?
					// case LifxNetPlus.LifxClient.TileEffectType tet:
					// 	Add((byte) tet);
					// 	break;
					default:
						Console.WriteLine("Unsupported type!" + args.GetType().FullName);
						throw new NotSupportedException(args.GetType().FullName);
				}
			}

			_ms = new MemoryStream(_data.ToArray());
			_len = _ms.Length;
			_br = new BinaryReader(_ms);
		}

		/// <summary>
		///     Initialize with a byte array
		/// </summary>
		/// <param name="data"></param>
		public Payload(byte[] data) {
			Objects = new List<object>();
			_data = data.ToList();
			_ms = new MemoryStream(data);
			_len = _ms.Length;
			_br = new BinaryReader(_ms);
		}

		/// <summary>
		///     Return our base byte list as an array
		/// </summary>
		/// <returns></returns>
		public byte[] ToArray() {
			return _data.ToArray();
		}

		/// <summary>
		///     Return our base byte list
		/// </summary>
		/// <returns></returns>
		public List<byte> ToList() {
			return _data;
		}

		/// <summary>
		///     Serialize base byte list to a string
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return string.Join(",", (from a in _data select a.ToString("X2")).ToArray());
		}

		/// <summary>
		///     Check to see if we still have data to read
		/// </summary>
		/// <returns></returns>
		public bool HasContent() {
			return _ms.Position < _len;
		}

		/// <summary>
		///     Rewind our pointer N bytes
		/// </summary>
		/// <param name="len">How far to rewind. Default is 1.</param>
		public void Rewind(int len = 1) {
			if (_ms.Position - len < 0) {
				Reset();
			} else {
				_ms.Seek(len * -1, SeekOrigin.Current);
			}
		}

		/// <summary>
		///     Forward our pointer N bytes
		/// </summary>
		/// <param name="len">How far to advance. Default is 1.</param>
		public void Advance(int len = 1) {
			if (_ms.Position + len < _len) {
				_ms.Seek(len, SeekOrigin.Current);
			} else {
				FastForward();
			}
		}

		/// <summary>
		///     Forward the pointer to the end of the array
		/// </summary>
		public void FastForward() {
			_ms.Seek(0, SeekOrigin.End);
		}

		/// <summary>
		///     Reset our pointer to 0
		/// </summary>
		public void Reset() {
			_ms.Seek(0, SeekOrigin.Begin);
		}

		/// <summary>
		///     Read LifxColor from array and increment pointer 8 bytes
		/// </summary>
		/// <returns></returns>
		public LifxColor GetColor() {
			var color = new LifxColor();
			try {
				var h = GetUInt16(false);
				var s = GetUInt16(false);
				var b = GetUInt16(false);
				var k = GetUInt16(false);
				color = new LifxColor(h, s, b, k);
			} catch (Exception) {
				//Debug.WriteLine("Exception: " + e.Message);
			}

			Objects.Add(color);
			return color;
		}

		/// <summary>
		/// Read a date from the current payload
		/// </summary>
		/// <returns>A Datetime</returns>
		public DateTime GetDate() {
			var date = DateTime.MinValue;
			try {
				var stamp = GetUInt64();
				var epoch = stamp / 1000;
				date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
					.AddSeconds(epoch);
			} catch (Exception) {
				//Debug.WriteLine("Exception: " + e.Message);
			}

			Objects.Add(date);
			return date;
		}

		/// <summary>
		///     Get an array of bytes from the reader
		/// </summary>
		/// <param name="len"></param>
		/// <returns></returns>
		public byte[] GetBytes(int len) {
			var bytes = _br.ReadBytes(len);
			Objects.Add(bytes);
			return bytes;
		}

		/// <summary>
		///     Read Uint8 from array and increment pointer 1 byte
		/// </summary>
		/// <returns>byte</returns>
		public byte GetUint8() {
			byte num = 0;
			try {
				num = _br.ReadByte();
			} catch {
				//Debug.WriteLine("Error reading byte, pos is " + _ms.Position);
			}

			Objects.Add(num);
			return num;
		}

		/// <summary>
		///     Read UInt16 from array and increment pointer 2 bytes
		/// </summary>
		/// <returns>ushort</returns>
		public ushort GetUInt16(bool addToData = true) {
			ushort num = 0;
			try {
				num = _br.ReadUInt16();
			} catch {
				//Debug.WriteLine($"Error getting Uint16 from payload, pointer {_ms.Position} of range: " + _len);
			}

			if (addToData) {
				Objects.Add(num);
			}

			return num;
		}

		/// <summary>
		///     Read Int16 from array and increment pointer 2 bytes.
		/// </summary>
		/// <returns>short</returns>
		public short GetInt16() {
			short num = 0;
			try {
				num = _br.ReadInt16();
			} catch {
				//Debug.WriteLine($"Error getting int16 from payload, pointer {_ms.Position} of range: " + _len);
			}

			Objects.Add(num);
			return num;
		}

		/// <summary>
		///     Read Int32 from array and increment pointer 4 bytes.
		/// </summary>
		/// <returns>int</returns>
		public int GetInt32() {
			var num = 0;
			try {
				num = _br.ReadInt32();
			} catch {
				//Debug.WriteLine($"Error getting Int32 from payload, pointer {_ms.Position} of range: " + _len);
			}

			Objects.Add(num);
			return num;
		}

		/// <summary>
		///     Read a UInt32 from array and increment pointer 4 bytes.
		/// </summary>
		/// <returns></returns>
		public uint GetUInt32() {
			uint num = 0;
			try {
				num = _br.ReadUInt32();
			} catch {
				//Debug.WriteLine($"Error getting Uint32 from payload, pointer {_ms.Position} of range: " + _len);
			}

			Objects.Add(num);
			return num;
		}

		/// <summary>
		///     Read an Int64 from array and increment pointer 8 bytes.
		/// </summary>
		/// <returns>long</returns>
		public long GetInt64() {
			long num = 0;
			try {
				num = _br.ReadInt64();
			} catch {
				//Debug.WriteLine($"Error getting Int64 from payload, pointer {_ms.Position} of range: " + _len);
			}

			Objects.Add(num);
			return num;
		}

		/// <summary>
		///     Read a UInt64 from array and increment pointer 8 bytes.
		/// </summary>
		/// <returns>ulong</returns>
		public ulong GetUInt64() {
			ulong num = 0;
			try {
				num = _br.ReadUInt64();
			} catch {
				//Debug.WriteLine($"Error getting Uint64 from payload, pointer {_ms.Position} of range: " + _len);
			}

			Objects.Add(num);
			return num;
		}

		/// <summary>
		///     Read a Float32 from array and increment pointer 4 bytes.
		/// </summary>
		/// <returns>float</returns>
		public float GetFloat32() {
			var num = 0f;
			try {
				num = _br.ReadSingle();
			} catch {
				//Debug.WriteLine($"Error getting Float32 from payload, pointer {_ms.Position} of range: " + _len);
			}

			Objects.Add(num);
			return num;
		}

		/// <summary>
		///     Read a string from our payload.
		/// </summary>
		/// <param name="length">The number of chars to read. If none specified, will read the entire payload</param>
		/// <returns>string</returns>
		public string GetString(long length = -1) {
			var output = string.Empty;
			if (length == -1) {
				length = _len - 1 - _ms.Position;
			}

			try {
				var str = _br.ReadChars((int) length);
				StringBuilder builder = new StringBuilder();
				foreach (var value in str) {
					builder.Append(value);
				}

				output = builder.ToString();
				output = output.Replace("\0", string.Empty);
			} catch {
				//Debug.WriteLine($"Error getting string, pointer {_ms.Position} out of range: " + _len);
			}

			Objects.Add(output);
			return output;
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(string input) {
			var bytes = input.Select(l => Encoding.Unicode.GetBytes(l.ToString())[0]).ToList();
			var paddedBytes = new byte[32];
			for (var i = 0; i < 32; i++) {
				if (i < bytes.Count) {
					paddedBytes[i] = bytes[i];
				} else {
					paddedBytes[i] = 0;
				}
			}

			_data.AddRange(paddedBytes);
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(byte input) {
			_data.Add(input);
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(byte[] input) {
			_data.AddRange(input);
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(short input) {
			_data.AddRange(BitConverter.GetBytes(input));
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(int input) {
			_data.AddRange(BitConverter.GetBytes(input));
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(uint input) {
			_data.AddRange(BitConverter.GetBytes(input));
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(long input) {
			_data.AddRange(BitConverter.GetBytes(input));
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(float input) {
			_data.AddRange(BitConverter.GetBytes(input));
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(double input) {
			_data.AddRange(BitConverter.GetBytes(input));
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(ushort input) {
			_data.AddRange(BitConverter.GetBytes(input));
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(ulong input) {
			_data.AddRange(BitConverter.GetBytes(input));
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(LifxColor input) {
			_data.AddRange(input.ToBytes());
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(IEnumerable<LifxColor> input) {
			foreach (var lc in input) {
				Add(lc);
			}
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(DateTime input) {
			var epoch = new DateTime(1970, 1, 1);
			var updated = input - epoch;
			Add(updated.TotalMilliseconds * 1000);
		}

		/// <summary>
		/// Adds the input
		/// </summary>
		/// <param name="input">The input</param>
		private void Add(Tile input) {
			Add(input.AccelMeasX);
			Add(input.AccelMeasY);
			Add(input.AccelMeasZ);
			Add(0);
			Add(input.UserX);
			Add(input.UserY);
			Add(input.Width);
			Add(input.Height);
			Add((byte) 0);
			Add(input.DeviceVersionVendor);
			Add(input.DeviceVersionProduct);
			Add(input.DeviceVersionVersion);
			Add(input.FirmwareBuild);
			Add((ulong) 0);
			Add(input.FirmwareVersionMinor);
			Add(input.FirmwareVersionMajor);
			Add((uint) 0);
		}

		/// <summary>
		/// Adds the WaveForm enum input
		/// </summary>
		/// <param name="input"></param>
		private void Add(LifxNetPlus.LifxClient.WaveFormType input) {
			Console.WriteLine($"Adding waveform type ${input}");
			var value = (byte) input;
			Console.WriteLine($"Value is {value}");
			Add((byte) input);
		}
	}
}
