using System.Collections.Generic;
using MathNet.Numerics.Random;

public static class ExtensionClass
{
	private static MersenneTwister rng = new MersenneTwister();

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = rng.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
