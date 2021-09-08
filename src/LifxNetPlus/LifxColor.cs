using System;
using System.Collections.Generic;
using System.Drawing;

namespace LifxNetPlus {
	/// <summary>
	/// Handler Converting RGB color to Lifx data format
	/// </summary>
	[Serializable]
	public class LifxColor {
		/// <summary>
		/// Brightness
		/// </summary>
		public double B { get; set; }
		/// <summary>
		/// Hue
		/// </summary>
		public double H { get; set; }
		/// <summary>
		/// Saturation
		/// </summary>
		public double S { get; set; }

		/// <summary>
		/// Color Temperature
		/// </summary>
		public ushort Kelvin { get; set; } = 0;

		/// <summary>
		/// Default constructor
		/// </summary>
		public LifxColor() {
		}

		/// <summary>
		/// Set lifx color based on RGBA value
		/// </summary>
		/// <param name="r">Red</param>
		/// <param name="g">Green</param>
		/// <param name="b">Blue</param>
		/// <param name="a">Alpha (default is 255)</param>
		public LifxColor(byte r, byte g, byte b, byte a = 255) {
			var color = Color.FromArgb(a, r, g, b);
			var hsb = Utilities.RgbToHsb(color);
			H = hsb[0];
			S = hsb[1];
			B = hsb[2];
		}

		/// <summary>
		/// Create lifx color using HSBK values
		/// </summary>
		/// <param name="h">Hue</param>
		/// <param name="s">Saturation</param>
		/// <param name="b">Brightness</param>
		/// <param name="k">Kelvin</param>
		public LifxColor(ushort h, ushort s, ushort b, ushort k) {
			H = h;
			S = s;
			B = b;
			Kelvin = k;
		}

		/// <summary>
		/// Overload to create lifx color using HSB and optional K value
		/// </summary>
		/// <param name="h">Hue</param>
		/// <param name="s">Saturation</param>
		/// <param name="b">Brightness</param>
		/// <param name="k">Kelvin (Default is 5750)</param>
		public LifxColor(double h, double s, double b, double k = 5750) {
			H = h;
			S = s;
			B = b;
			Kelvin = (ushort) k;
		}

		/// <summary>
		///     Create a lifx color from RGB color and optional brightness (double, 0-1)
		/// </summary>
		/// <param name="color"></param>
		/// <param name="brightness">Brightness as a decimal</param>
		public LifxColor(Color color, double brightness = -1) {
			var hsb = Utilities.RgbToHsb(color);
			H = hsb[0];
			S = hsb[1];
			B = hsb[2];
			if (brightness == -1) {
				return;
			}

			if (B > brightness) {
				B = brightness;
			}
		}

		/// <summary>
		///     Convert(theoretically) our values to HSBK values
		/// </summary>
		/// <returns></returns>
		public byte[] ToBytes() {
			var output = new List<byte>(8);

			var hu = (ushort) (Math.Round(0x10000 * H) / 360 % 0x10000);
			var sa = (ushort) Math.Round(0xFFFF * S);
			var br = (ushort) Math.Round(0xFFFF * B);

			output.AddRange(BitConverter.GetBytes(hu));
			output.AddRange(BitConverter.GetBytes(sa));
			output.AddRange(BitConverter.GetBytes(br));
			output.AddRange(BitConverter.GetBytes(Kelvin));
			return output.ToArray();
		}

		/// <summary>
		///     Return Lifx HSBK string representation of the color
		/// </summary>
		/// <returns></returns>
		public string ToHsbkString() {
			return H + ", " + S + ", " + B + ", " + Kelvin;
		}
	}
}