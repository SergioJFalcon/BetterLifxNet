using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using LifxNetPlus;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleApp.Universal {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page {
		/// <summary>
		/// The client
		/// </summary>
		LifxClient _client;
		/// <summary>
		/// The device
		/// </summary>
		ObservableCollection<Device> bulbs = new ObservableCollection<Device>();

		/// <summary>
		/// The hue
		/// </summary>
		UInt16 hue;
		/// <summary>
		/// The pending update color
		/// </summary>
		private Task pendingUpdateColor;

		/// <summary>
		/// The pending update color action
		/// </summary>
		private Action pendingUpdateColorAction;
		/// <summary>
		/// The saturation
		/// </summary>
		UInt16 saturation;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainPage"/> class
		/// </summary>
		public MainPage() {
			InitializeComponent();
			bulbList.ItemsSource = bulbs;
		}

		/// <summary>
		/// Ons the navigated to using the specified e
		/// </summary>
		/// <param name="e">The </param>
		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			base.OnNavigatedTo(e);
			_client = await LifxClient.CreateAsync();
			_client.DeviceDiscovered += ClientDeviceDeviceDiscovered;
			_client.DeviceLost += ClientDeviceDeviceLost;
			_client.StartDeviceDiscovery();
			await Task.FromResult(true);
		}

		/// <summary>
		/// Ons the navigating from using the specified e
		/// </summary>
		/// <param name="e">The </param>
		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
			_client.DeviceDiscovered -= ClientDeviceDeviceDiscovered;
			_client.DeviceLost -= ClientDeviceDeviceLost;
			_client.StopDeviceDiscovery();
			_client = null;
			base.OnNavigatingFrom(e);
		}

		/// <summary>
		/// Clients the device device lost using the specified sender
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The </param>
		private void ClientDeviceDeviceLost(object sender, LifxClient.DeviceDiscoveryEventArgs e) {
			var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
				var bulb = e.Device;
				if (bulbs.Contains(bulb))
					bulbs.Remove(bulb);
			});
		}

		/// <summary>
		/// Clients the device device discovered using the specified sender
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The </param>
		private void ClientDeviceDeviceDiscovered(object sender, LifxClient.DeviceDiscoveryEventArgs e) {
			var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
				var bulb = e.Device;
				if (!bulbs.Contains(bulb))
					bulbs.Add(bulb);
			});
		}

		/// <summary>
		/// Bulbs the list selection changed using the specified sender
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The </param>
		private async void bulbList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var bulb = bulbList.SelectedItem;
			if (bulb != null) {
				var state = await _client.GetLightStateAsync((Device)bulb);
				Name.Text = state.Label;
				PowerState.IsOn = state.IsOn;
				hue = state.Hue;
				saturation = state.Saturation;
				translate.X = ColorGrid.ActualWidth / 65535 * hue;
				translate.Y = ColorGrid.ActualHeight / 65535 * saturation;
				brightnessSlider.Value = state.Brightness;
				statePanel.Visibility = Visibility.Visible;
			} else
				statePanel.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// Powers the state toggled using the specified sender
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The </param>
		private async void PowerState_Toggled(object sender, RoutedEventArgs e) {
			var bulb = bulbList.SelectedItem;
			if (bulb != null) {
				await _client.SetDevicePowerStateAsync((Device)bulb, PowerState.IsOn);
			}
		}

		/// <summary>
		/// Brightnesses the slider value changed using the specified sender
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The </param>
		private void brightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
			var bulb = bulbList.SelectedItem;
			if (bulb != null)
				SetColor((Device)bulb, null, null, (UInt16) e.NewValue);
		}

		/// <summary>
		/// Sets the color using the specified bulb
		/// </summary>
		/// <param name="bulb">The bulb</param>
		/// <param name="hue">The hue</param>
		/// <param name="saturation">The saturation</param>
		/// <param name="brightness">The brightness</param>
		private async void SetColor(Device bulb, ushort? hue, ushort? saturation, ushort? brightness) {
			if (_client == null || bulb == null) return;
			//Is a task already running? This avoids updating too often.
			//Come back and execute last call when currently running operation is complete
			if (pendingUpdateColor != null) {
				pendingUpdateColorAction = () => SetColor(bulb, hue, saturation, brightness);
				return;
			}

			this.hue = hue.HasValue ? hue.Value : this.hue;
			this.saturation = saturation.HasValue ? saturation.Value : this.saturation;
			var b = brightness.HasValue ? brightness.Value : (UInt16) brightnessSlider.Value;
			var color = new LifxColor(this.hue, this.saturation, b, 2700);
			var setColorTask = _client.SetColorAsync(bulb, color);
			var throttleTask = Task.Delay(50); //Ensure task takes minimum 50 ms (no more than 20 messages per second)
			pendingUpdateColor = Task.WhenAll(setColorTask, throttleTask);
			try {
				Task timeoutTask = Task.Delay(2000);
				await Task.WhenAny(timeoutTask, pendingUpdateColor);
				if (!pendingUpdateColor.IsCompleted) {
					//timeout
				}
			} catch {
			} //ignore errors (usually timeout)

			pendingUpdateColor = null;
			if (pendingUpdateColorAction != null) //if a pending action is waiting, run it now;
			{
				var a = pendingUpdateColorAction;
				pendingUpdateColorAction = null;
				a();
			}
		}

		/// <summary>
		/// Colors the grid tapped using the specified sender
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">The </param>
		private void ColorGrid_Tapped(object sender, TappedRoutedEventArgs e) {
			FrameworkElement elm = (FrameworkElement) sender;
			var p = e.GetPosition(elm);
			var Hue = p.X / elm.ActualWidth * 65535;
			var Sat = p.Y / elm.ActualHeight * 65535;
			var bulb = bulbList.SelectedItem;
			if (bulb != null) {
				SetColor((Device)bulb, (ushort) Hue, (ushort) Sat, null);
			}

			translate.X = p.X;
			translate.Y = p.Y;
		}
	}
}