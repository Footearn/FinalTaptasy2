using System;
using System.Collections.Generic;

namespace MathNet.Numerics.Random
{
	public abstract class RandomSource : System.Random
	{
		private readonly bool _threadSafe;

		private readonly object _lock = new object();

		protected RandomSource()
			: base(RandomSeed.Robust())
		{
			_threadSafe = false;
		}

		protected RandomSource(bool threadSafe)
			: base(RandomSeed.Robust())
		{
			_threadSafe = threadSafe;
		}

		public void NextDoubles(double[] values)
		{
			if (_threadSafe)
			{
				lock (_lock)
				{
					for (int i = 0; i < values.Length; i++)
					{
						values[i] = DoSample();
					}
				}
				return;
			}
			for (int j = 0; j < values.Length; j++)
			{
				values[j] = DoSample();
			}
		}

		public double[] NextDoubles(int count)
		{
			double[] array = new double[count];
			NextDoubles(array);
			return array;
		}

		public IEnumerable<double> NextDoubleSequence()
		{
			for (int j = 0; j < 64; j++)
			{
				yield return NextDouble();
			}
			double[] buffer = new double[64];
			while (true)
			{
				NextDoubles(buffer);
				for (int i = 0; i < buffer.Length; i++)
				{
					yield return buffer[i];
				}
			}
		}

		public sealed override int Next()
		{
			if (_threadSafe)
			{
				lock (_lock)
				{
					return DoSampleInteger();
					IL_0024:;
				}
			}
			return DoSampleInteger();
		}

		public sealed override int Next(int maxExclusive)
		{
			if (maxExclusive <= 0)
			{
				throw new ArgumentException("ArgumentMustBePositive");
			}
			if (maxExclusive == int.MaxValue)
			{
				return Next();
			}
			if (_threadSafe)
			{
				lock (_lock)
				{
					return DoSampleInteger(0, maxExclusive);
					IL_004a:;
				}
			}
			return DoSampleInteger(0, maxExclusive);
		}

		public sealed override int Next(int minInclusive, int maxExclusive)
		{
			if (minInclusive > maxExclusive)
			{
				throw new ArgumentException("ArgumentMinValueGreaterThanMaxValue");
			}
			if (maxExclusive == int.MaxValue && minInclusive == 0)
			{
				return Next();
			}
			if (_threadSafe)
			{
				lock (_lock)
				{
					return DoSampleInteger(minInclusive, maxExclusive);
					IL_0050:;
				}
			}
			return DoSampleInteger(minInclusive, maxExclusive);
		}

		public void NextInt32s(int[] values)
		{
			if (_threadSafe)
			{
				lock (_lock)
				{
					for (int i = 0; i < values.Length; i++)
					{
						values[i] = DoSampleInteger();
					}
				}
				return;
			}
			for (int j = 0; j < values.Length; j++)
			{
				values[j] = DoSampleInteger();
			}
		}

		public int[] NextInt32s(int count)
		{
			int[] array = new int[count];
			NextInt32s(array);
			return array;
		}

		public void NextInt32s(int[] values, int minInclusive, int maxExclusive)
		{
			if (minInclusive > maxExclusive)
			{
				throw new ArgumentException("ArgumentMinValueGreaterThanMaxValue");
			}
			if (maxExclusive == int.MaxValue && minInclusive == 0)
			{
				NextInt32s(values);
				return;
			}
			if (_threadSafe)
			{
				lock (_lock)
				{
					for (int i = 0; i < values.Length; i++)
					{
						values[i] = DoSampleInteger(minInclusive, maxExclusive);
					}
				}
				return;
			}
			for (int j = 0; j < values.Length; j++)
			{
				values[j] = DoSampleInteger(minInclusive, maxExclusive);
			}
		}

		public int[] NextInt32s(int count, int minInclusive, int maxExclusive)
		{
			int[] array = new int[count];
			NextInt32s(array, minInclusive, maxExclusive);
			return array;
		}

		public IEnumerable<int> NextInt32Sequence()
		{
			for (int j = 0; j < 64; j++)
			{
				yield return Next();
			}
			int[] buffer = new int[64];
			while (true)
			{
				NextInt32s(buffer);
				for (int i = 0; i < buffer.Length; i++)
				{
					yield return buffer[i];
				}
			}
		}

		public IEnumerable<int> NextInt32Sequence(int minInclusive, int maxExclusive)
		{
			if (minInclusive > maxExclusive)
			{
				throw new ArgumentException("ArgumentMinValueGreaterThanMaxValue");
			}
			for (int j = 0; j < 64; j++)
			{
				yield return Next(minInclusive, maxExclusive);
			}
			int[] buffer = new int[64];
			while (true)
			{
				NextInt32s(buffer, minInclusive, maxExclusive);
				for (int i = 0; i < buffer.Length; i++)
				{
					yield return buffer[i];
				}
			}
		}

		public override void NextBytes(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (_threadSafe)
			{
				lock (_lock)
				{
					for (int i = 0; i < buffer.Length; i++)
					{
						buffer[i] = (byte)(DoSampleInteger() % 256);
					}
				}
				return;
			}
			for (int j = 0; j < buffer.Length; j++)
			{
				buffer[j] = (byte)(DoSampleInteger() % 256);
			}
		}

		protected sealed override double Sample()
		{
			if (_threadSafe)
			{
				lock (_lock)
				{
					return DoSample();
					IL_0024:;
				}
			}
			return DoSample();
		}

		protected abstract double DoSample();

		protected virtual int DoSampleInteger()
		{
			return (int)(DoSample() * 2147483647.0);
		}

		protected virtual int DoSampleInteger(int minInclusive, int maxExclusive)
		{
			return (int)(DoSample() * (double)(maxExclusive - minInclusive)) + minInclusive;
		}
	}
}
