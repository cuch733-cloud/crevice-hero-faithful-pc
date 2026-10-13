using System.Collections;
using UnionAssets.FLE;
using UnityEngine;

public class TwitterPostScreenshotTask : EventDispatcher
{
	private string text;

	public static TwitterPostScreenshotTask Create()
	{
		return new GameObject("TwitterPostScreenshotTask").AddComponent<TwitterPostScreenshotTask>();
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
		SPShareUtility.TwitterShare(text, tex);
		Object.Destroy(tex);
		dispatch("complete");
	}
}
