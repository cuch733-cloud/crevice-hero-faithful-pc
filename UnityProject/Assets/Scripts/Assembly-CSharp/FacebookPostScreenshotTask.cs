using System.Collections;
using UnionAssets.FLE;
using UnityEngine;

public class FacebookPostScreenshotTask : EventDispatcher
{
	private string text;

	public static FacebookPostScreenshotTask Create()
	{
		return new GameObject("FacebookPostScreenshotTask").AddComponent<FacebookPostScreenshotTask>();
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
		SPShareUtility.FacebookShare(text, tex);
		Object.Destroy(tex);
		dispatch("complete");
	}
}
