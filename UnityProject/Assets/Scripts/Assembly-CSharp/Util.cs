using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Util
{
	public static void d(object msg)
	{
		if (GameConfig.PRINT_DEBUG)
		{
			Debug.Log(msg);
		}
	}

	public static string Md5Sum(string strToEncrypt)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public static int GetIntValue(string key, string hashKey, int defaultVal)
	{
		int result = defaultVal;
		int num = PlayerPrefs.GetInt(key, defaultVal);
		if (num > 0)
		{
			string intHash = GetIntHash(num);
			string text = PlayerPrefs.GetString(hashKey, string.Empty);
			if (intHash == text)
			{
				result = num;
			}
			else
			{
				d("distance hash not match");
				num = defaultVal;
			}
		}
		else
		{
			num = defaultVal;
		}
		return result;
	}

	public static string GetIntHash(int val)
	{
		return Md5Sum("heHAMA" + (val * 73 + 11) + "!!");
	}

	public static IEnumerator Shake(GameObject obj, float duration, float magnitude, float hDamp = 1f, float vDamp = 1f)
	{
		float elapsed = 0f;
		Vector3 originalPos = obj.transform.position;
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime + 0.02f;
			float percentComplete = elapsed / duration;
			float damper = 1f - Mathf.Clamp(4f * percentComplete - 3f, 0f, 1f);
			float x = UnityEngine.Random.value * 2f - 1f;
			float y = UnityEngine.Random.value * 2f - 1f;
			x *= magnitude * damper * hDamp;
			y *= magnitude * damper * vDamp;
			obj.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
			yield return new WaitForSeconds(0.02f);
		}
		obj.transform.position = originalPos;
	}

	public static string xs(string textToEncrypt)
	{
		StringBuilder stringBuilder = new StringBuilder(textToEncrypt);
		StringBuilder stringBuilder2 = new StringBuilder(textToEncrypt.Length);
		int num = 98739;
		for (int i = 0; i < textToEncrypt.Length; i++)
		{
			char c = stringBuilder[i];
			c = (char)(c ^ num);
			stringBuilder2.Append(c);
		}
		return stringBuilder2.ToString();
	}

	public static string dc(string message)
	{
		byte[] bytes = Convert.FromBase64String(message);
		return Encoding.UTF8.GetString(bytes);
	}

	public static void LogEvent(string message)
	{
		if ((bool)AndroidHub.Instance)
		{
			AndroidHub.Instance.LogEvent(message);
		}
	}
}
