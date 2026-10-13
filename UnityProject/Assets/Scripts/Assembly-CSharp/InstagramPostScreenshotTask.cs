using System.Collections;
using UnionAssets.FLE;
using UnityEngine;

public class InstagramPostScreenshotTask : EventDispatcher
{
	private string text;

	public static InstagramPostScreenshotTask Create()
	{
		return new GameObject("InstagramPostScreenshotTask").AddComponent<InstagramPostScreenshotTask>();
	}

	public void Post(string _text)
	{
		text = _text;
		StartCoroutine(PostScreenshot());
	}

	private IEnumerator PostScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		SA_Singleton<SPInstagram>.instance.Share(tex, text);
		Object.Destroy(tex);
		dispatch("complete");
	}
}
