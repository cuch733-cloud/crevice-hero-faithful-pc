using System;
using UnityEngine;

public class GP_Event
{
	public string Id;

	public string Description;

	public string IconImageUrl;

	public string FormattedValue;

	public long Value;

	private Texture2D _icon;

	public Texture2D icon
	{
		get
		{
			return _icon;
		}
	}

	public void LoadIcon()
	{
		if (!(icon != null))
		{
			WWWTextureLoader wWWTextureLoader = WWWTextureLoader.Create();
			wWWTextureLoader.OnLoad = (Action<Texture2D>)Delegate.Combine(wWWTextureLoader.OnLoad, new Action<Texture2D>(OnTextureLoaded));
			wWWTextureLoader.LoadTexture(IconImageUrl);
		}
	}

	private void OnTextureLoaded(Texture2D tex)
	{
		_icon = tex;
	}
}
