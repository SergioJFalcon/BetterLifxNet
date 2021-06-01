using System;

namespace LifxNetPlus {
	/// <summary>
	/// Simple class to get message ID (Sequence) and/or source for messages
	/// </summary>
	public static class MessageId {
		private static byte _sequence = 1;
		private static readonly object IdentifierLock = new object();
		private static uint _source;

		/// <summary>
		/// Get the next number in our sequence, rotate back to 1 if we reach the maximum
		/// </summary>
		/// <returns></returns>
		public static byte GetNextSequence() {
			lock (IdentifierLock) {
				_sequence++;
				if (_sequence > 255) {
					_sequence = 1;
				}
			}

			return _sequence;
		}

		/// <summary>
		/// Generate a message source
		/// </summary>
		/// <returns></returns>
		public static uint GetSource() {
			if (_source == 0) {
				var rand = new Random();
				_source = ((uint) rand.Next(1 << 30) << 2) | (uint) rand.Next(1 << 2);
			}

			return _source;
		}
	}
}