using System;
using System.Collections.Generic;

namespace MathNet.Numerics.Random
{
	public static class RandomExtensions
	{
		public static void NextDoubles(this System.Random rnd, double[] values)
		{
			RandomSource randomSource = rnd as RandomSource;
			if (randomSource != null)
			{
				randomSource.NextDoubles(values);
				return;
			}
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = rnd.NextDouble();
			}
		}

		public static double[] NextDoubles(this System.Random rnd, int count)
		{
			double[] array = new double[count];
			rnd.NextDoubles(array);
			return array;
		}

		public static IEnumerable<double> NextDoubleSequence(this System.Random rnd)
		{
			RandomSource randomSource = rnd as RandomSource;
			if (randomSource != null)
			{
				return randomSource.NextDoubleSequence();
			}
			return NextDoubleSequenceEnumerable(rnd);
		}

		private static IEnumerable<double> NextDoubleSequenceEnumerable(System.Random rnd)
		{
			while (true)
			{
				yield return rnd.NextDouble();
			}
		}

		public static byte[] NextBytes(this System.Random rnd, int count)
		{
			byte[] array = new byte[count];
			rnd.NextBytes(array);
			return array;
		}

		public static void NextInt32s(this System.Random rnd, int[] values)
		{
			RandomSource randomSource = rnd as RandomSource;
			if (randomSource != null)
			{
				randomSource.NextInt32s(values);
				return;
			}
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = rnd.Next();
			}
		}

		public static void NextInt32s(this System.Random rnd, int[] values, int minInclusive, int maxExclusive)
		{
			RandomSource randomSource = rnd as RandomSource;
			if (randomSource != null)
			{
				randomSource.NextInt32s(values, minInclusive, maxExclusive);
				return;
			}
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = rnd.Next(minInclusive, maxExclusive);
			}
		}

		public static IEnumerable<int> NextInt32Sequence(this System.Random rnd, int minInclusive, int maxExclusive)
		{
			RandomSource randomSource = rnd as RandomSource;
			if (randomSource != null)
			{
				return randomSource.NextInt32Sequence(minInclusive, maxExclusive);
			}
			return NextInt32SequenceEnumerable(rnd, minInclusive, maxExclusive);
		}

		private static IEnumerable<int> NextInt32SequenceEnumerable(System.Random rnd, int minInclusive, int maxExclusive)
		{
			while (true)
			{
				yield return rnd.Next(minInclusive, maxExclusive);
			}
		}

		public static long NextInt64(this System.Random rnd)
		{
			byte[] array = new byte[8];
			rnd.NextBytes(array);
			long num = BitConverter.ToInt64(array, 0);
			num &= 0x7FFFFFFFFFFFFFFFL;
			return (num != long.MaxValue) ? num : rnd.NextInt64();
		}

		public static int NextFullRangeInt32(this System.Random rnd)
		{
			byte[] array = new byte[4];
			rnd.NextBytes(array);
			return BitConverter.ToInt32(array, 0);
		}

		public static long NextFullRangeInt64(this System.Random rnd)
		{
			byte[] array = new byte[8];
			rnd.NextBytes(array);
			return BitConverter.ToInt64(array, 0);
		}

		public static decimal NextDecimal(this System.Random rnd)
		{
			decimal num;
			do
			{
				num = new decimal(rnd.NextFullRangeInt32(), rnd.NextFullRangeInt32(), rnd.NextFullRangeInt32(), false, 28);
			}
			while (num >= 1.0m);
			return num;
		}

		public static bool NextBoolean(this System.Random rnd)
		{
			return rnd.NextDouble() >= 0.5;
		}
	}
}
