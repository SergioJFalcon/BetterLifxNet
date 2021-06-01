using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LifxNetPlus {
	
	
	/// <summary>
	/// Lifx Product Class
	/// </summary>
	[Serializable]
	public class Product {
		/// <summary>
		/// Supported features of the device
		/// </summary>
		public Features features { get; set; }
		/// <summary>
		/// Product id
		/// </summary>
		public int pid { get; set; }
		/// <summary>
		/// Device Name
		/// </summary>
		public string name { get; set; }
	}
	
	/// <summary>
	/// Class representation of device features
	/// </summary>
	[Serializable]
	public class Features {
		/// <summary>
		/// Can be chained?
		/// </summary>
		public bool chain { get; set; }
		/// <summary>
		/// Can render colors
		/// </summary>
		public bool color { get; set; }
		/// <summary>
		/// Infrared
		/// </summary>
		public bool infrared { get; set; }
		/// <summary>
		/// Matrix
		/// </summary>
		public bool matrix { get; set; }
		/// <summary>
		/// Multizone support
		/// </summary>
		public bool multizone { get; set; }
		/// <summary>
		/// Has Buttons
		/// </summary>
		public bool? buttons { get; set; }
		/// <summary>
		/// HEV
		/// </summary>
		public bool? hev { get; set; }
		/// <summary>
		/// Relays
		/// </summary>
		public bool? relays { get; set; }
		/// <summary>
		/// Min firmware
		/// </summary>
		public int? min_ext_mz_firmware { get; set; }
		/// <summary>
		/// Min ex mz firmware components
		/// </summary>
		public List<int> min_ext_mz_firmware_components { get; set; }
		/// <summary>
		/// Range of supported color temperatures
		/// </summary>
		public List<int> temperature_range { get; set; }
	}


	/// <summary>
	/// Outer Product data class
	/// </summary>
	public class ProductData {
		/// <summary>
		/// Vendor ID
		/// </summary>
		public int vid { get; set; }

		private List<Product> _products;

		private const string _productJson =
			"[{\"vid\":1,\"name\":\"LIFX\",\"products\":[{\"pid\":1,\"name\":\"LIFX Original 1000\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":3,\"name\":\"LIFX Color 650\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":10,\"name\":\"LIFX White 800 (Low Voltage)\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2700,6500]}},{\"pid\":11,\"name\":\"LIFX White 800 (High Voltage)\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2700,6500]}},{\"pid\":15,\"name\":\"LIFX Color 1000\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":18,\"name\":\"LIFX White 900 BR30 (Low Voltage)\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":19,\"name\":\"LIFX White 900 BR30 (High Voltage)\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":20,\"name\":\"LIFX Color 1000 BR30\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":22,\"name\":\"LIFX Color 1000\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":27,\"name\":\"LIFX A19\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":28,\"name\":\"LIFX BR30\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":29,\"name\":\"LIFX A19 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":30,\"name\":\"LIFX BR30 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":31,\"name\":\"LIFX Z\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":true,\"temperature_range\":[2500,9000]}},{\"pid\":32,\"name\":\"LIFX Z\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":true,\"temperature_range\":[2500,9000],\"min_ext_mz_firmware\":1532997580,\"min_ext_mz_firmware_components\":[2,77]}},{\"pid\":36,\"name\":\"LIFX Downlight\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":37,\"name\":\"LIFX Downlight\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":38,\"name\":\"LIFX Beam\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":true,\"temperature_range\":[2500,9000],\"min_ext_mz_firmware\":1532997580,\"min_ext_mz_firmware_components\":[2,77]}},{\"pid\":39,\"name\":\"LIFX Downlight White To Warm\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[1500,9000]}},{\"pid\":40,\"name\":\"LIFX Downlight\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":43,\"name\":\"LIFX A19\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":44,\"name\":\"LIFX BR30\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":45,\"name\":\"LIFX A19 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":46,\"name\":\"LIFX BR30 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":49,\"name\":\"LIFX Mini Color\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":50,\"name\":\"LIFX Mini White To Warm\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[1500,4000]}},{\"pid\":51,\"name\":\"LIFX Mini White\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2700,2700]}},{\"pid\":52,\"name\":\"LIFX GU10\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":53,\"name\":\"LIFX GU10\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":55,\"name\":\"LIFX Tile\",\"features\":{\"color\":true,\"chain\":true,\"matrix\":true,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":57,\"name\":\"LIFX Candle\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":true,\"infrared\":false,\"multizone\":false,\"temperature_range\":[1500,9000]}},{\"pid\":59,\"name\":\"LIFX Mini Color\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":60,\"name\":\"LIFX Mini White To Warm\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[1500,4000]}},{\"pid\":61,\"name\":\"LIFX Mini White\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2700,2700]}},{\"pid\":62,\"name\":\"LIFX A19\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":63,\"name\":\"LIFX BR30\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":64,\"name\":\"LIFX A19 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":65,\"name\":\"LIFX BR30 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":66,\"name\":\"LIFX Mini White\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2700,2700]}},{\"pid\":68,\"name\":\"LIFX Candle\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":true,\"infrared\":false,\"multizone\":false,\"temperature_range\":[1500,9000]}},{\"pid\":70,\"name\":\"LIFX Switch\",\"features\":{\"color\":false,\"relays\":true,\"chain\":false,\"matrix\":false,\"buttons\":true,\"infrared\":false,\"multizone\":false}},{\"pid\":81,\"name\":\"LIFX Candle White To Warm\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2200,6500]}},{\"pid\":82,\"name\":\"LIFX Filament Clear\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2100,2100]}},{\"pid\":85,\"name\":\"LIFX Filament Amber\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2000,2000]}},{\"pid\":87,\"name\":\"LIFX Mini White\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2700,2700]}},{\"pid\":88,\"name\":\"LIFX Mini White\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2700,2700]}},{\"pid\":89,\"name\":\"LIFX Switch\",\"features\":{\"color\":false,\"relays\":true,\"chain\":false,\"matrix\":false,\"buttons\":true,\"infrared\":false,\"multizone\":false}},{\"pid\":90,\"name\":\"LIFX Clean\",\"features\":{\"hev\":true,\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":91,\"name\":\"LIFX Color\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":92,\"name\":\"LIFX Color\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":94,\"name\":\"LIFX BR30\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":96,\"name\":\"LIFX Candle White To Warm\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2200,6500]}},{\"pid\":97,\"name\":\"LIFX A19\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":98,\"name\":\"LIFX BR30\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":99,\"name\":\"LIFX Clean\",\"features\":{\"hev\":true,\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":100,\"name\":\"LIFX Filament Clear\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2100,2100]}},{\"pid\":101,\"name\":\"LIFX Filament Amber\",\"features\":{\"color\":false,\"chain\":false,\"matrix\":false,\"infrared\":false,\"multizone\":false,\"temperature_range\":[2000,2000]}},{\"pid\":109,\"name\":\"LIFX A19 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":110,\"name\":\"LIFX BR30 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}},{\"pid\":111,\"name\":\"LIFX A19 Night Vision\",\"features\":{\"color\":true,\"chain\":false,\"matrix\":false,\"infrared\":true,\"multizone\":false,\"temperature_range\":[2500,9000]}}]}]";

		private static List<ProductData> LoadData() {
			var data = JsonConvert.DeserializeObject<List<ProductData>>(_productJson);
			return data ?? new List<ProductData>();
		}

		/// <summary>
		/// Return product information by product id
		/// </summary>
		/// <param name="pid">The product ID to look up.</param>
		/// <returns>A <seealso cref="Product"/> if the PID is valid, or null.</returns>
		public static Product? GetProduct(int pid) {
			var products = LoadData()[0]._products;
			return products.FirstOrDefault(product => product.pid == pid);
		}
	}
}