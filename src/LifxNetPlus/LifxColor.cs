using System;
using System.Collections.Generic;
using System.Drawing;

namespace LifxNetPlus {
	[Serializable]
	public class LifxColor {
		public double B { get; set; }
		public double H { get; set; }
		public double S { get; set; }

		public ushort Kelvin { get; set; } = 5750;


		private Color _color;

		public LifxColor() {
		}

		public LifxColor(byte r, byte g, byte b, byte a = 255) {
			var color = Color.FromArgb(a, r, g, b);
			var hsb = Utilities.RgbToHsb(color);
			H = hsb[0];
			S = hsb[1];
			B = hsb[2];
		}

		public LifxColor(ushort h, ushort s, ushort b, ushort k) {
			H = h;
			S = s;
			B = b;
			Kelvin = k;
		}

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
		/// <param name="brightness"></param>
		public LifxColor(Color color, double brightness = -1) {
			var hsb = Utilities.RgbToHsb(color);
			H = hsb[0];
			S = hsb[1];
			B = hsb[2];
			if (brightness == -1) {
				return;
			}

			B *= brightness;
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

			var h = (ushort) (H / 360 * 65535);
			var s = (ushort) (S * 65535);
			var b = (ushort) (B * 65535);

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