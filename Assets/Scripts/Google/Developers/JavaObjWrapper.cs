using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Google.Developers
{
	public class JavaObjWrapper
	{
		private IntPtr raw;

		private IntPtr cachedRawClass = IntPtr.Zero;

		public IntPtr RawObject
		{
			get
			{
				return raw;
			}
		}

		public virtual IntPtr RawClass
		{
			get
			{
				if (cachedRawClass == IntPtr.Zero && raw != IntPtr.Zero)
				{
					cachedRawClass = AndroidJNI.GetObjectClass(raw);
				}
				return cachedRawClass;
			}
		}

		protected JavaObjWrapper()
		{
		}

		public JavaObjWrapper(string clazzName)
		{
			raw = AndroidJNI.AllocObject(AndroidJNI.FindClass(clazzName));
		}

		public JavaObjWrapper(IntPtr rawObject)
		{
			raw = rawObject;
		}

		public void CreateInstance(string clazzName, params object[] args)
		{
			if (raw != IntPtr.Zero)
			{
				throw new Exception("Java object already set");
			}
			IntPtr constructorID = AndroidJNIHelper.GetConstructorID(RawClass, args);
			jvalue[] args2 = ConstructArgArray(args);
			raw = AndroidJNI.NewObject(RawClass, constructorID, args2);
		}

		protected static jvalue[] ConstructArgArray(object[] theArgs)
		{
			object[] array = new object[theArgs.Length];
			for (int i = 0; i < theArgs.Length; i++)
			{
				if (theArgs[i] is JavaObjWrapper)
				{
					array[i] = ((JavaObjWrapper)theArgs[i]).raw;
				}
				else
				{
					array[i] = theArgs[i];
				}
			}
			jvalue[] array2 = AndroidJNIHelper.CreateJNIArgArray(array);
			for (int j = 0; j < theArgs.Length; j++)
			{
				if (theArgs[j] is JavaObjWrapper)
				{
					array2[j].l = ((JavaObjWrapper)theArgs[j]).raw;
				}
				else if (theArgs[j] is JavaInterfaceProxy)
				{
					IntPtr l = AndroidJNIHelper.CreateJavaProxy((AndroidJavaProxy)theArgs[j]);
					array2[j].l = l;
				}
			}
			if (array2.Length == 1)
			{
				for (int k = 0; k < array2.Length; k++)
				{
					Debug.Log("---- [" + k + "] -- " + array2[k].l);
				}
			}
			return array2;
		}

		public static T GetStaticObjectField<T>(string clsName, string name, string sig)
		{
			IntPtr clazz = AndroidJNI.FindClass(clsName);
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, sig);
			IntPtr staticObjectField = AndroidJNI.GetStaticObjectField(clazz, staticFieldID);
			ConstructorInfo constructor = typeof(T).GetConstructor(new Type[1]
			{
				staticObjectField.GetType()
			});
			if (constructor != null)
			{
				return (T)constructor.Invoke(new object[1]
				{
					staticObjectField
				});
			}
			Type typeFromHandle = typeof(T);
			return (T)Marshal.PtrToStructure(staticObjectField, typeFromHandle);
		}

		public static int GetStaticIntField(string clsName, string name)
		{
			IntPtr clazz = AndroidJNI.FindClass(clsName);
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, "I");
			return AndroidJNI.GetStaticIntField(clazz, staticFieldID);
		}

		public static string GetStaticStringField(string clsName, string name)
		{
			IntPtr clazz = AndroidJNI.FindClass(clsName);
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, "Ljava/lang/String;");
			return AndroidJNI.GetStaticStringField(clazz, staticFieldID);
		}

		public static float GetStaticFloatField(string clsName, string name)
		{
			IntPtr clazz = AndroidJNI.FindClass(clsName);
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, "F");
			return AndroidJNI.GetStaticFloatField(clazz, staticFieldID);
		}

		public void InvokeCallVoid(string name, string sig, params object[] args)
		{

		}
	}
}
