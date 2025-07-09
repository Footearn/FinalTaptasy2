using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Util : Singleton<Util>
{
	private static DateTime dateTimeNotUse = new DateTime(2016, 1, 1);

	public static Vector2 getCurrentScreenToWorldPosition()
	{
		return Singleton<CachedManager>.instance.ingameCamera.ScreenToWorldPoint(Input.mousePosition);
	}

	public static Vector2 getCurrentScreenToWorldPosition(Vector2 position)
	{
		return Singleton<CachedManager>.instance.ingameCamera.ScreenToWorldPoint(position);
	}

	public static double Clamp(double targetValue, double min, double max)
	{
		double num = targetValue;
		if (num > max)
		{
			num = max;
		}
		if (num < min)
		{
			num = min;
		}
		return num;
	}

	public static double Lerp(double startValue, double endValue, double time)
	{
		return startValue + (endValue - startValue) * time;
	}

	public static Color getCalculatedColor(float r, float g, float b)
	{
		return new Color(r / 255f, g / 255f, b / 255f, 1f);
	}

	public static Color getCalculatedColor(float r, float g, float b, float a)
	{
		return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
	}

	public static void changeSpriteColor(SpriteRenderer targetSpriteRenderer, Color targetColor)
	{
		targetSpriteRenderer.color = targetColor;
	}

	public static void changeSpritesColor(SpriteRenderer[] targetSpriteRenderers, Color targetColor)
	{
		for (int i = 0; i < targetSpriteRenderers.Length; i++)
		{
			if (targetSpriteRenderers[i] != null)
			{
				targetSpriteRenderers[i].color = targetColor;
			}
		}
	}

	public static MemoryStream SerializeToStream<T>(T o)
	{
		MemoryStream memoryStream = new MemoryStream();
		IFormatter formatter = new BinaryFormatter();
		formatter.Serialize(memoryStream, o);
		return memoryStream;
	}

	public static object DeserializeFromStream(MemoryStream stream)
	{
		IFormatter formatter = new BinaryFormatter();
		stream.Seek(0L, SeekOrigin.Begin);
		return formatter.Deserialize(stream);
	}

	public static bool checkInXRange(Vector2 position1, Vector2 position2, float width)
	{
		bool result = false;
		if (Mathf.Abs(position1.x - position2.x) <= width)
		{
			result = true;
		}
		return result;
	}

	public static bool checkInRect(Vector2 position1, Vector2 position2, float width, float height)
	{
		bool result = false;
		if (Mathf.Abs(position1.x - position2.x) <= width && Mathf.Abs(position1.y - position2.y) <= height)
		{
			result = true;
		}
		return result;
	}

	public static Transform getNearTransform(Vector2 basePosition, params Transform[] targets)
	{
		Transform result = null;
		float num = float.MaxValue;
		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i] != null)
			{
				float num2 = Vector2.Distance(basePosition, targets[i].position);
				if (num2 <= num)
				{
					num = num2;
					result = targets[i];
				}
			}
		}
		return result;
	}

	public static bool isInternetConnection()
	{
		bool result = false;
		if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			result = true;
		}
		return result;
	}

	public void captureScreenShot(Vector2 position, Action<Sprite> endEvent)
	{
		StartCoroutine(waitForCaptureScreenShot(position, delegate(Sprite sprite)
		{
			endEvent(sprite);
		}));
	}

	private IEnumerator waitForCaptureScreenShot(Vector2 position, Action<Sprite> endEvent)
	{
		Rect textureRect = new Rect(position.x, position.y, Screen.width, Screen.height / 3);
		Texture2D texture = new Texture2D(Screen.width, Screen.height / 3, TextureFormat.ARGB32, false, false);
		yield return new WaitForEndOfFrame();
		texture.ReadPixels(textureRect, 0, 0);
		texture.Apply();
		Sprite screenShot = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one * 0.5f, 25f);
		endEvent(screenShot);
	}

	public static long UnixTimestampFromDateTime(DateTime date)
	{
		long num = date.Ticks - new DateTime(1970, 1, 1).Ticks;
		return num / 10000000;
	}

	public static DateTime DateTimeNotUse()
	{
		return dateTimeNotUse;
	}

	public static string SHA256Token(string message, string secret)
	{
		//Discarded unreachable code: IL_0043
		secret = secret ?? string.Empty;
		ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
		byte[] bytes = aSCIIEncoding.GetBytes(secret);
		byte[] bytes2 = aSCIIEncoding.GetBytes(message);
		using (HMACSHA256 hMACSHA = new HMACSHA256(bytes))
		{
			byte[] inArray = hMACSHA.ComputeHash(bytes2);
			return Convert.ToBase64String(inArray);
		}
	}

	public static string SHA256TokenUTF8(string message, string secret)
	{
		//Discarded unreachable code: IL_0043
		secret = secret ?? string.Empty;
		byte[] bytes = Encoding.UTF8.GetBytes(secret);
		byte[] bytes2 = Encoding.UTF8.GetBytes(message);
		using (HMACSHA256 hMACSHA = new HMACSHA256(bytes))
		{
			byte[] inArray = hMACSHA.ComputeHash(bytes2);
			return Convert.ToBase64String(inArray);
		}
	}

	public static bool isActive(GameObject targetGameObject)
	{
		return targetGameObject.activeSelf && targetGameObject.activeInHierarchy;
	}
}
