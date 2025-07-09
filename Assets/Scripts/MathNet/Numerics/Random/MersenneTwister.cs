using System;
using System.Collections.Generic;

namespace MathNet.Numerics.Random
{
	public class MersenneTwister : RandomSource
	{
		private const uint LowerMask = 2147483647u;

		private const int M = 397;

		private const uint MatrixA = 2567483615u;

		private const int N = 624;

		private const double Reciprocal = 2.3283064365386963E-10;

		private const uint UpperMask = 2147483648u;

		private static readonly uint[] Mag01 = new uint[2]
		{
			0u,
			2567483615u
		};

		private readonly uint[] _mt = new uint[624];

		private int _mti = 625;

		[ThreadStatic]
		private static MersenneTwister DefaultInstance;

		public static MersenneTwister Default
		{
			get
			{
				if (DefaultInstance == null)
				{
					DefaultInstance = new MersenneTwister(RandomSeed.Robust(), threadSafe: true);
				}
				return DefaultInstance;
			}
		}

		public MersenneTwister()
			: this(RandomSeed.Robust())
		{
		}

		public MersenneTwister(bool threadSafe)
			: this(RandomSeed.Robust(), threadSafe)
		{
		}

		public MersenneTwister(int seed)
		{
			init_genrand((uint)seed);
		}

		public MersenneTwister(int seed, bool threadSafe)
			: base(threadSafe)
		{
			init_genrand((uint)seed);
		}

		private void init_genrand(uint s)
		{
			_mt[0] = (uint)((int)s & -1);
			for (_mti = 1; _mti < 624; _mti++)
			{
				_mt[_mti] = (uint)((int)(1812433253 * (_mt[_mti - 1] ^ (_mt[_mti - 1] >> 30))) + _mti);
				_mt[_mti] &= uint.MaxValue;
			}
		}

		private uint genrand_int32()
		{
			uint num;
			if (_mti >= 624)
			{
				if (_mti == 625)
				{
					init_genrand(5489u);
				}
				int i;
				for (i = 0; i < 227; i++)
				{
					num = (uint)(((int)_mt[i] & int.MinValue) | (int)(_mt[i + 1] & int.MaxValue));
					_mt[i] = (_mt[i + 397] ^ (num >> 1) ^ Mag01[num & 1]);
				}
				for (; i < 623; i++)
				{
					num = (uint)(((int)_mt[i] & int.MinValue) | (int)(_mt[i + 1] & int.MaxValue));
					_mt[i] = (_mt[i + -227] ^ (num >> 1) ^ Mag01[num & 1]);
				}
				num = (uint)(((int)_mt[623] & int.MinValue) | (int)(_mt[0] & int.MaxValue));
				_mt[623] = (_mt[396] ^ (num >> 1) ^ Mag01[num & 1]);
				_mti = 0;
			}
			num = _mt[_mti++];
			num ^= num >> 11;
			num = (uint)((int)num ^ ((int)(num << 7) & -1658038656));
			num = (uint)((int)num ^ ((int)(num << 15) & -272236544));
			return num ^ (num >> 18);
		}

		protected sealed override double DoSample()
		{
			return (double)genrand_int32() * 2.3283064365386963E-10;
		}

		protected sealed override int DoSampleInteger()
		{
			uint num = genrand_int32();
			int num2 = (int)(num >> 1);
			if (num2 == int.MaxValue)
			{
				return DoSampleInteger();
			}
			return num2;
		}

		public static void Doubles(double[] values, int seed)
		{
			uint[] array = new uint[624];
			array[0] = (uint)(seed & -1);
			int i;
			for (i = 1; i < 624; i++)
			{
				array[i] = (uint)((int)(1812433253 * (array[i - 1] ^ (array[i - 1] >> 30))) + i);
				array[i] &= uint.MaxValue;
			}
			for (int j = 0; j < values.Length; j++)
			{
				uint num;
				if (i >= 624)
				{
					int k;
					for (k = 0; k < 227; k++)
					{
						num = (uint)(((int)array[k] & int.MinValue) | (int)(array[k + 1] & int.MaxValue));
						array[k] = (array[k + 397] ^ (num >> 1) ^ Mag01[num & 1]);
					}
					for (; k < 623; k++)
					{
						num = (uint)(((int)array[k] & int.MinValue) | (int)(array[k + 1] & int.MaxValue));
						array[k] = (array[k + -227] ^ (num >> 1) ^ Mag01[num & 1]);
					}
					num = (uint)(((int)array[623] & int.MinValue) | (int)(array[0] & int.MaxValue));
					array[623] = (array[396] ^ (num >> 1) ^ Mag01[num & 1]);
					i = 0;
				}
				num = array[i++];
				num ^= num >> 11;
				num = (uint)((int)num ^ ((int)(num << 7) & -1658038656));
				num = (uint)((int)num ^ ((int)(num << 15) & -272236544));
				num ^= num >> 18;
				values[j] = (double)num * 2.3283064365386963E-10;
			}
		}

		public static double[] Doubles(int length, int seed)
		{
			double[] array = new double[length];
			Doubles(array, seed);
			return array;
		}

		public static IEnumerable<double> DoubleSequence(int seed)
		{
			uint[] t = new uint[624];
			t[0] = (uint)(seed & -1);
			int i;
			for (i = 1; i < 624; i++)
			{
				t[i] = (uint)((int)(1812433253 * (t[i - 1] ^ (t[i - 1] >> 30))) + i);
				t[i] &= uint.MaxValue;
			}
			while (true)
			{
				uint y6;
				if (i >= 624)
				{
					int kk;
					for (kk = 0; kk < 227; kk++)
					{
						y6 = (uint)(((int)t[kk] & int.MinValue) | (int)(t[kk + 1] & int.MaxValue));
						t[kk] = (t[kk + 397] ^ (y6 >> 1) ^ Mag01[y6 & 1]);
					}
					for (; kk < 623; kk++)
					{
						y6 = (uint)(((int)t[kk] & int.MinValue) | (int)(t[kk + 1] & int.MaxValue));
						t[kk] = (t[kk + -227] ^ (y6 >> 1) ^ Mag01[y6 & 1]);
					}
					y6 = (uint)(((int)t[623] & int.MinValue) | (int)(t[0] & int.MaxValue));
					t[623] = (t[396] ^ (y6 >> 1) ^ Mag01[y6 & 1]);
					i = 0;
				}
				uint[] array = t;
				int num;
				i = (num = i) + 1;
				y6 = array[num];
				y6 ^= y6 >> 11;
				y6 = (uint)((int)y6 ^ ((int)(y6 << 7) & -1658038656));
				y6 = (uint)((int)y6 ^ ((int)(y6 << 15) & -272236544));
				y6 ^= y6 >> 18;
				yield return (double)y6 * 2.3283064365386963E-10;
			}
		}
	}
}
