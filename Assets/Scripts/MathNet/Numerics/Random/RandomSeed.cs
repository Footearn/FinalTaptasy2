using System;

namespace MathNet.Numerics.Random
{
	public static class RandomSeed
	{
		private static readonly object Lock = new object();

		private static readonly System.Random MasterRng = new System.Random();

		public static int Time()
		{
			return Environment.TickCount;
		}

		public static int Guid()
		{
			return Environment.TickCount ^ System.Guid.NewGuid().GetHashCode();
		}

		public static int Robust()
		{
			lock (Lock)
			{
				return MasterRng.NextFullRangeInt32() ^ Environment.TickCount ^ System.Guid.NewGuid().GetHashCode();
				IL_0030:
				int result;
				return result;
			}
		}
	}
}
