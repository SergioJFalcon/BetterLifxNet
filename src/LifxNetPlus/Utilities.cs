using System;
using System.Drawing;

namespace LifxNetPlus {
	internal static class Utilities {
		public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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