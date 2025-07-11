using System;
using System.Collections.Generic;

public class SA_DataConverter
{
	public const char DATA_SPLITTER = '|';

	public const string DATA_SPLITTER2 = "|%|";

	public const string ARRAY_SPLITTER = "%%%";

	public const string DATA_EOF = "endofline";

	public static string SerializeArray(string[] array, string splitter = "%%%")
	{
		if (array == null)
		{
			return string.Empty;
		}
		if (array.Length == 0)
		{
			return string.Empty;
		}
		string text = string.Empty;
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			if (i != 0)
			{
				text += splitter;
			}
			text += array[i];
		}
		return text;
	}

	public static string[] ParseArray(string arrayData, string splitter = "%%%")
	{
		List<string> list = new List<string>();
		string[] array = arrayData.Split(new string[1]
		{
			splitter
		}, StringSplitOptions.None);
		for (int i = 0; i < array.Length && !(array[i] == "endofline"); i++)
		{
			list.Add(array[i]);
		}
		return list.ToArray();
	}
}
