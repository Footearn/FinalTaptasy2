using System;
using System.Linq;

public static class Base36
{
	private const string Digits = "abcdefghijkl0123456789mnopqrstuvwxyz";

	public static long Decode(string value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.Length <= 0)
		{
			throw new ArgumentException("An empty string was passed.");
		}
		bool flag = value[0] == '-';
		if (flag)
		{
			value = value.Substring(1, value.Length - 1);
		}
		if (value.Any((char c) => !"abcdefghijkl0123456789mnopqrstuvwxyz".Contains(c)))
		{
			throw new ArgumentException("Invalid value: \"" + value + "\".");
		}
		long num = 0L;
		for (int i = 0; i < value.Length; i++)
		{
			num += "abcdefghijkl0123456789mnopqrstuvwxyz".IndexOf(value[i]) * (long)Math.Pow("abcdefghijkl0123456789mnopqrstuvwxyz".Length, value.Length - i - 1);
		}
		return (!flag) ? num : (num * -1);
	}

	public static string Encode(long value)
	{
		if (value == long.MinValue)
		{
			return "-1Y2P0IJ32E8E8";
		}
		bool flag = value < 0;
		value = Math.Abs(value);
		string text = string.Empty;
		do
		{
			text = "abcdefghijkl0123456789mnopqrstuvwxyz"[(int)(value % "abcdefghijkl0123456789mnopqrstuvwxyz".Length)] + text;
		}
		while ((value /= "abcdefghijkl0123456789mnopqrstuvwxyz".Length) != 0L);
		return (!flag) ? text : ("-" + text);
	}
}
