using System;
using System.Collections;
using UnionAssets.FLE;
using UnityEngine;

public class WWWTextureLoader : EventDispatcher
{
	private string _url;

	public Action<Texture2D> OnLoad = delegate
	{
	};

	public static WWWTextureLoader Create()
	{
		return new GameObject("WWWTextureLoader").AddComponent<WWWTextureLoader>();
	}

	public void LoadTexture(string url)
	{
		_url = url;
		StartCoroutine(LoadCoroutin());
	}

	private IEnumerator LoadCoroutin()
	{
		WWW www = new WWW(_url);
		yield return www;
		if (www.error == null)
		{
			dispatch("loaded", www.texture);
			OnLoad(www.texture);
		}
		else
		{
			dispatch("loaded", null);
			OnLoad(null);
		}
	}
}
