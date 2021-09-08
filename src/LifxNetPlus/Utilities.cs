using System;
using System.Drawing;

namespace LifxNetPlus {
	/// <summary>
	/// The utilities class
	/// </summary>
	internal static class Utilities {
		/// <summary>
		/// The utc
		/// </summary>
		public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Rgs the bto hsb using the specified r
		/// </summary>
		/// <param name="r">The </param>
		/// <param name="g">The </param>
		/// <param name="b">The </param>
		/// <param name="hsbvals">The hsbvals</param>
		/// <returns>The hsbvals</returns>
		public static float[] RGBtoHSB(int r, int g, int b, float[] hsbvals) {
			float brightness;
			float hue;
			float saturation;
			if (hsbvals == null) {
				hsbvals = new float[3];
			}
        
			int cmin = Math.Min(r, Math.Min(g, b));
			var cmax = Math.Max(r, Math.Max(g, b));
        
			brightness = (float) cmax / 255;
			if (cmax != 0) {
				saturation = (cmax - cmin) / (float) cmax;
			}
			else {
				saturation = 0;
			}
        
			if (saturation == 0) {
				hue = 0;
			}
			else {
				var redc = (cmax - r) / (float)(cmax - cmin);
				var greenc = (cmax - g) / (float)(cmax - cmin);
				var bluec = (cmax - b) / (float)(cmax - cmin);
				if (r == cmax) {
					hue = bluec - greenc;
				}
				else if (g == cmax) {
					hue = 2 + (redc - bluec);
				} else {
					hue = 4 + (greenc - redc);
				}
            
				hue /= 6;
				if (hue < 0) {
					hue += 1;
				}
            
			}
        
			hsbvals[0] = hue;
			hsbvals[1] = saturation;
			hsbvals[2] = brightness;
			return hsbvals;
		}
		/// <summary>
		/// Rgbs the to hsb using the specified input
		/// </summary>
		/// <param name="input">The input</param>
		/// <returns>The double array</returns>
		public static double[] RgbToHsb(Color input) {
			// '# Normalize the RGB values by scaling them to be between 0 and 1
			var red = input.R / (double) 255;
			var green = input.G / (double) 255;
			var blue = input.B / (double) 255;

			var minValue = Math.Min(red, Math.Min(green, blue));
			var maxValue = Math.Max(red, Math.Max(green, blue));

			var maxByte = Math.Max(input.R, Math.Max(input.G, input.B));
			var delta = maxValue - minValue;
			double h = 0;
			var v = maxValue;

			// '# Calculate the hue (in degrees of a circle, between 0 and 360)
			if (maxByte == input.R) {
				if (green >= blue) {
					if (delta == 0) {
						h = 0;
					} else {
						h = 60 * (green - blue) / delta;
					}
				} else if (green < blue) {
					h = 60 * (green - blue) / delta + 360;
				}
			} else if (maxByte == input.G) {
				h = 60 * (blue - red) / delta + 120;
			} else if (maxByte == input.B) {
				h = 60 * (red - green) / delta + 240;
			}

			if (red == 0 && green == 0 && blue == 0) {
				h = 0d;
			}

			// '# Calculate the saturation (between 0 and 1)
			var s = maxValue == 0 ? 0 : 1 - minValue / maxValue;
			return new[] {h, s, v};
		}
	}
}