using System;
using System.Collections.Generic;
using System.Drawing;

namespace LifxNetPlus {
	[Serializable]
	public class LifxColor {

		private byte _r;
		public byte R {
			get => _r;
			set {
				_r = value;
				SetHsb();
				ScaleHsb();
			}
		}

		private byte _g;

		public byte G {
			get => _g;
			set {
				_g = value;
				SetHsb();
				ScaleHsb();
			}
		}

		private byte _b;
		public byte B {
			get => _b;
			set {
				_b = value;
				SetHsb();
				ScaleHsb();
			}
		}
		public byte A { get; set; }

		private float _hue;
		public float Hue {
			get => _hue;
			set {
				_hue = value;
				ScaleHsb();
				SetRgb();
			}
		}

		private float _saturation;
		public float Saturation {
			get => _saturation;
			set {
				_saturation = value;
				ScaleHsb();
				SetRgb();
			}
		}

		private float _brightness;
		public float Brightness {
			get => _brightness;
			set {
				_brightness = value;
				ScaleHsb();
				SetRgb();
			}
		}

		private ushort _hueScaled;
		public ushort HueScaled {
			get => _hueScaled;
			set {
				_hueScaled = value;
				DescaleHsb();
				SetRgb();
			}
		}

		private ushort _saturationScaled;
		public ushort SaturationScaled {
			get => _saturationScaled;
			set {
				_saturationScaled = value;
				DescaleHsb();
				SetRgb();
			}
		}

		private ushort _brightnessScaled;
		public ushort BrightnessScaled {
			get => _brightnessScaled;
			set {
				_brightnessScaled = value;
				DescaleHsb();
				SetRgb();
			}
		}
		public ushort Kelvin { get; set; }
		
		public Color Color => Color.FromArgb(_r, _g, _b);

		public LifxColor() {
			_hueScaled = 0;
			_brightnessScaled = 0;
			_saturationScaled = 0;
			DescaleHsb();
			SetRgb();
		}

		public LifxColor(ushort hue, ushort sat, ushort bri, ushort k) {
			_hueScaled = hue;
			_saturationScaled = sat;
			_brightnessScaled = bri;
			Kelvin = k;
			DescaleHsb();
			SetRgb();
		}

		public LifxColor(byte r, byte g, byte b, byte a= 255) {
			_r = r;
			_g = g;
			_b = b;
			A = a;
			SetHsb();
			ScaleHsb();
		}

		public LifxColor(Color color) {
			_r = color.R;
			_g = color.G;
			_b = color.B;
			A = color.A;
			SetHsb();
			ScaleHsb();

		}
		
		public byte[] ToBytes() {
			var output = new List<byte>();
			foreach (var u in new[] {_hueScaled, _saturationScaled, _brightnessScaled, Kelvin}) {
				output.AddRange(BitConverter.GetBytes(u));
			}
			return output.ToArray();
		}
		
		/// <summary>
		/// Return Lifx HSBK string representation of the color
		/// </summary>
		/// <returns></returns>
		public string ToHsbkString() {
			return _hueScaled + ", " + _saturationScaled + ", " + _brightnessScaled + ", " + Kelvin;
		}

		private void SetHsb() {
			int max = Math.Max(R, Math.Max(G, B));
			int min = Math.Min(R, Math.Min(G, B));
			var color = Color.FromArgb(R, G, B);
			_hue = color.GetHue();
			_saturation = max == 0 ? 0 : 1f - 1f * min / max;
			_brightness = max / 255f;
		}

		private void DescaleHsb() {
			_hue = (_hueScaled & 65535) / (float) 65535 * 360;
			_saturation = (_saturationScaled & 65535) / (float) 65535;
			_brightness = (_brightnessScaled & 65535) / (float) 65535;
		}

		private void ScaleHsb() {
			_hueScaled = (ushort) (int)(_hue * 65536);
			_saturationScaled = (ushort) (int)(Saturation * 65535);
			_brightnessScaled = (ushort) (int)(Brightness * 65535);
		}
		
		private void SetRgb() {
			var hi = Convert.ToInt32(Math.Floor(_hue / 60)) % 6;
			var f = _hue / 60 - Math.Floor(_hue / 60);
			var bri = _brightness * 255;
			var v = Convert.ToInt32(bri);
			var p = Convert.ToInt32(bri * (1 - _saturation));
			var q = Convert.ToInt32(bri * (1 - f * _saturation));
			var t = Convert.ToInt32(bri * (1 - (1 - f) * _saturation));
			A = 255;
			switch (hi) {
				case 0:
					_r = (byte) v;
					_g = (byte) t;
					_b = (byte) p;
					break;
				case 1:
					_r = (byte) q;
					_g = (byte) v;
					_b = (byte) p;
					break;
				case 2:
					_r = (byte) p;
					_g = (byte) v;
					_b = (byte) t;
					break;
				case 3:
					_r = (byte) p;
					_g = (byte) q;
					_b = (byte) v;
					break;
				case 4:
					_r = (byte) t;
					_g = (byte) p;
					_b = (byte) v;
					break;
				default:
					_r = (byte) v;
					_g = (byte) p;
					_b = (byte) q;
					break;
			}
		}
	}
}