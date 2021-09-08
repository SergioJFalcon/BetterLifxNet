using System;
using System.Threading.Tasks;

namespace LifxNetPlus {
	/// <summary>
	/// The lifx client class
	/// </summary>
	public partial class LifxClient {
		/// <summary>
		/// Tile Effect Type
		/// </summary>
		public enum TileEffectType {
			/// <summary>
			/// The off tile effect type
			/// </summary>
			Off = 0,
			/// <summary>
			/// The reserved tile effect type
			/// </summary>
			Reserved1 = 1,
			/// <summary>
			/// The morph tile effect type
			/// </summary>
			Morph = 2,
			/// <summary>
			/// The flame tile effect type
			/// </summary>
			Flame = 3,
			/// <summary>
			/// The reserved tile effect type
			/// </summary>
			Reserved2 = 4
		}

		/// <summary>
		/// The reserved
		/// </summary>
		private const int Reserved = 0x00;

		/// <summary>
		///     This message returns information about the tiles in the chain.
		/// </summary>
		/// <param name="group"></param>
		/// <returns>StateDeviceChainResponse</returns>
		public async Task<StateDeviceChainResponse?> GetDeviceChainAsync(Device group) {
			if (group == null) {
				throw new ArgumentNullException(nameof(group));
			}

			return await BroadcastMessageAsync<StateDeviceChainResponse>(group,
				new LifxPacket(MessageType.GetDeviceChain));
		}

		/// <summary>
		///     Used to tell each tile what their position is.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="tileIndex"></param>
		/// <param name="userX"></param>
		/// <param name="userY"></param>
		/// <returns></returns>
		public async Task SetUserPositionAsync(Device group, int tileIndex, float userX, float userY) {
			if (group == null) {
				throw new ArgumentNullException(nameof(@group));
			}

			var packet = new LifxPacket(MessageType.SetUserPosition, tileIndex, Reserved, userX, userY);
			await BroadcastMessageAsync<AcknowledgementResponse>(group, packet);
		}

		/// <summary>
		///     Get the state of 64 pixels in the tile in a rectangle that has a starting point and width.
		///     The tile_index is used to control the starting tile in the chain and length is used to get the state of
		///     that many tiles beginning from the tile_index. This will result in a separate response from each tile.
		///     For the LIFX Tile it really only makes sense to set x and y to zero, and width to 8.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="tileIndex">used to control the starting tile in the chain</param>
		/// <param name="length">used to get the state of that many tiles beginning from the tile_index.</param>
		/// <param name="x">Leave at 0</param>
		/// <param name="y">Leave at 0</param>
		/// <param name="width">Leave at 8</param>
		/// <returns>StateTileState64Response</returns>
		public async Task<StateTileState64Response?> GetTileState64Async(Device device, int tileIndex, int length,
			int x = 0, int y = 0, int width = 8) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.GetTileState64, tileIndex, length, Reserved, x, y, width);
			return await BroadcastMessageAsync<StateTileState64Response>(device, packet);
		}


		/// <summary>
		///     Set the state of 16 pixels in the tile in a rectangle that has a starting point and width.
		///     The tile_index is used to control the starting tile in the chain and length is used to get the state of
		///     that many tiles beginning from the tile_index. This will result in a separate response from each tile.
		///     For the LIFX Tile it really only makes sense to set x and y to zero, and width to 8.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="tileIndex">used to control the starting tile in the chain</param>
		/// <param name="length">used to get the state of that many tiles beginning from the tile_index.</param>
		/// <param name="duration"></param>
		/// <param name="colors">Colors to set (16).</param>
		/// <param name="x">Leave at 0</param>
		/// <param name="y">Leave at 0</param>
		/// <param name="width">Leave at 8</param>
		/// <returns>StateTileState64Response</returns>
		public async Task<StateTileState16Response?> SetTileState16Async(Device device, int tileIndex, int length,
			long duration, LifxColor[] colors, int x = 0, int y = 0, int width = 8) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.SetTileState16) {
				Payload = new Payload(new object[] {
					(byte) tileIndex, (byte) length, (byte) Reserved, (byte) x, (byte) y, (byte) width, (uint) duration,
					colors
				})
			};
			return await BroadcastMessageAsync<StateTileState16Response>(device, packet);
		}

		/// <summary>
		///     Set the state of 64 pixels in the tile in a rectangle that has a starting point and width.
		///     The tile_index is used to control the starting tile in the chain and length is used to get the state of
		///     that many tiles beginning from the tile_index. This will result in a separate response from each tile.
		///     For the LIFX Tile it really only makes sense to set x and y to zero, and width to 8.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="tileIndex">used to control the starting tile in the chain</param>
		/// <param name="length">used to get the state of that many tiles beginning from the tile_index.</param>
		/// <param name="duration"></param>
		/// <param name="colors">Colors to set.</param>
		/// <param name="x">Leave at 0</param>
		/// <param name="y">Leave at 0</param>
		/// <param name="width">Leave at 8</param>
		/// <returns>StateTileState64Response</returns>
		public async Task<StateTileState64Response?> SetTileState64Async(Device device, int tileIndex, int length,
			long duration, LifxColor[] colors, int x = 0, int y = 0, int width = 8) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.SetTileState64) {
				Payload = new Payload(new object[] {
					(byte) tileIndex, (byte) length, (byte) Reserved, (byte) x, (byte) y, (byte) width, (uint) duration,
					colors
				})
			};
			return await BroadcastMessageAsync<StateTileState64Response>(device, packet);
		}

		/// <summary>
		///     This packet will let you start a Firmware Effect on the device.
		///     Will return one StateTileEffect (720) message
		///     This packet requires the device has the Matrix Zones capability.
		///     You may use GetVersion (32), GetHostFirmware (14) and the Product Registry to determine
		///     whether your device has this capability
		/// </summary>
		/// <param name="device"></param>
		/// <param name="speed"></param>
		/// <param name="duration"></param>
		/// <param name="instanceId"></param>
		/// <param name="type"></param>
		/// <param name="palette"></param>
		/// <returns>StateTileState64Response</returns>
		public async Task<StateTileEffectResponse?> SetTileEffectAsync(Device device, uint instanceId,
			TileEffectType type, uint speed, ulong duration, LifxColor[] palette) {
			if (device == null) {
				throw new ArgumentNullException(nameof(device));
			}

			var packet = new LifxPacket(MessageType.SetTileState16) {
				Payload = new Payload(new object[] {
					(byte) Reserved, (byte) Reserved, instanceId, (byte) type, speed, duration, (ushort) Reserved,
					(uint) Reserved, (byte) palette.Length, palette
				})
			};
			return await BroadcastMessageAsync<StateTileEffectResponse>(device, packet);
		}
	}
}