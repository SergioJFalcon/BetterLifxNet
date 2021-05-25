using System;

namespace LifxNetPlus {
	public static class MessageId {
		private static byte _sequence = 1;
		private static readonly object IdentifierLock = new object();
		public static uint Source;

		public static byte GetNextSequence() {
			lock (IdentifierLock) {
				_sequence++;
				if (_sequence > 255) _sequence = 1;
			}

			return _sequence;
		}

		public static uint GetSource() {
			if (Source == 0) {
				var rand = new Random();
				Source = (uint)rand.Next(1 << 30) << 2 | (uint)rand.Next(1 << 2);
			}
			return Source;
		}
	}
}