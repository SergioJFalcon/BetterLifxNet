using System;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;

namespace LifxNetPlus {
	internal static class Utilities {
		public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


		public static ushort[] RgbToHsl(Color rgb) {
			// normalize red, green and blue values
			var r = rgb.R / 255.0;
			var g = rgb.G / 255.0;
			var b = rgb.B / 255.0;

			var max = Math.Max(r, Math.Max(g, b));
			var min = Math.Min(r, Math.Min(g, b));

			var h = 0.0;
			if (max == r && g >= b) {
				h = 60 * (g - b) / (max - min);
			} else if (max == r && g < b) {
				h = 60 * (g - b) / (max - min) + 360;
			} else if (max == g) {
				h = 60 * (b - r) / (max - min) + 120;
			} else if (max == b) {
				h = 60 * (r - g) / (max - min) + 240;
			}

			var s = max == 0 ? 0.0 : 1.0 - min / max;
			
			return new[] {
				(ushort) (h / 360 * 65535),
				(ushort) (s * 65535),
				(ushort) (max * 65535)
			};
		}
		
		public static double[] RgbToHsb(Color input) {
			// '# Normalize the RGB values by scaling them to be between 0 and 1
			var red = input.R / (double)255;
			var green = input.G / (double)255;
			var blue = input.B / (double)255;
			
			var minValue = Math.Min(red, Math.Min(green, blue));
			var maxValue = Math.Max(red, Math.Max(green, blue));
			
			var minByte = Math.Min(input.R, Math.Min(input.G, input.B));
			var maxByte = Math.Max(input.R, Math.Max(input.G, input.B));
			var delta = maxValue - minValue;
			double h = 0;
			var v = maxValue;

			// '# Calculate the hue (in degrees of a circle, between 0 and 360)
			if (maxByte == input.R) {
				if (green >= blue) {
					if (delta == 0)
						h = 0;
					else
						h = 60 * (green - blue) / delta;
				}
				else if (green < blue)
					h = 60 * (green - blue) / delta + 360;
			} else if (maxByte == input.G) {
				h = 60 * (blue - red) / delta + 120;
			}else if (maxByte == input.B) {
				h = 60 * (red - green) / delta + 240;
			}

			if (red == 0 && green == 0 && blue == 0) h = 0d;
			
			// '# Calculate the saturation (between 0 and 1)
			var s = maxValue == 0 ? 0 : 1 - minValue / maxValue;
			return new [] {h, s, v};
			
		}

		
		public static ushort[] RgbToHsv(Color color) {
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));

			var hue = color.GetHue();
			var saturation = max == 0 ? 0 : 1d - 1d * min / max;
			var value = max / 255d;

			return new[] {
				(ushort) (hue / 360 * 65535),
				(ushort) (saturation * 65535),
				(ushort) (value * 65535)
			};
		}

		public static ushort[] RgbToHsv2(Color color) {
			// maxc = max(r, g, b)
			// minc = min(r, g, b)
			float r = color.R;
			float g = color.G;
			float b = color.B;
			float maxc = Math.Max(color.R, Math.Max(color.G, color.B));
			float minc = Math.Min(color.R, Math.Min(color.G, color.B));
			// v = maxc

			var v = maxc / 255;
			if (Math.Abs(minc - maxc) < float.MinValue) return new ushort[] {0, 0, 0};
			var s = (maxc - minc) / maxc;
			// if minc == maxc:
			// return 0.0, 0.0, v
			// 	s = (maxc-minc) / maxc
			var rc = (maxc - r) / (maxc - minc);
			var gc = (maxc - g) / (maxc - minc);
			var bc = (maxc - b) / (maxc - minc);
			float h;
			if ((int)r == (int)maxc){
				h = bc - gc;
			} else if ((int)g == (int)maxc) {
				h = 2.0f + rc - bc;
			} else {
				h = 4.0f + gc - rc;
			}

			h = (float)(h / 6 % 1.0) * 100;
			//h = (1 - h) * 100;
			return new[] {
				(ushort) (h / 360 * 65535),
				(ushort) (s * 65535),
				(ushort) (v * 65535)
			};
			// return h, s, v
		}
		
		public static Color HsvToRgb(ushort sHue, ushort sSaturation, ushort sValue) {
			var hue = sHue / 65536f * 360f;
			var saturation = sSaturation / 65535f;
			var value = sValue / 65535f;
			int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			value = value * 255;
			int v = Convert.ToInt32(value);
			int p = Convert.ToInt32(value * (1 - saturation));
			int q = Convert.ToInt32(value * (1 - f * saturation));
			int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

			if (hi == 0)
				return Color.FromArgb(255, v, t, p);
			if (hi == 1)
				return Color.FromArgb(255, q, v, p);
			if (hi == 2)
				return Color.FromArgb(255, p, v, t);
			if (hi == 3)
				return Color.FromArgb(255, p, q, v);
			if (hi == 4)
				return Color.FromArgb(255, t, p, v);
			return Color.FromArgb(255, v, p, q);
		}

	}
}