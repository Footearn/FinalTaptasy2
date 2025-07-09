using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric.Answers.Internal
{
	internal class AnswersEventInstanceJavaObject
	{
		public AndroidJavaObject javaObject;

		private static bool IsNumericType(object o)
		{
			switch (Type.GetTypeCode(o.GetType()))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			default:
				return false;
			}
		}

		private static AndroidJavaObject AsDouble(object param)
		{
			return new AndroidJavaObject("java.lang.Double", param.ToString());
		}
	}
}
