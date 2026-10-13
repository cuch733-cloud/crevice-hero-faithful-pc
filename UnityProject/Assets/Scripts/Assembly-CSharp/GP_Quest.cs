using System;
using UnityEngine;

public class GP_Quest
{
	public string Id;

	public string Name;

	public string Description;

	public string IconImageUrl;

	public string BannerImageUrl;

	public GP_QuestState state;

	public long LastUpdatedTimestamp;

	public long AcceptedTimestamp;

	public long EndTimestamp;

	private Texture2D _icon;

	private Texture2D _banner;

	public Texture2D icon
	{
		get
		{
			return _icon;
		}
	}

	public Texture2D banner
	{
		get
		{
			return _banner;
		}
	}

	public void LoadIcon()
	{
		if (!(icon != null))
		{
			WWWTextureLoader wWWTextureLoader = WWWTextureLoader.Create();
			wWWTextureLoader.OnLoad = (Action<Texture2D>)Delegate.Combine(wWWTextureLoader.OnLoad, new Action<Texture2D>(OnIconLoaded));
			wWWTextureLoader.LoadTexture(IconImageUrl);
		}
	}

	public void LoadBanner()
	{
		if (!(icon != null))
		{
			WWWTextureLoader wWWTextureLoader = WWWTextureLoader.Create();
			wWWTextureLoader.OnLoad = (Action<Texture2D>)Delegate.Combine(wWWTextureLoader.OnLoad, new Action<Texture2D>(OnBannerLoaded));
			wWWTextureLoader.LoadTexture(BannerImageUrl);
		}
	}

	private void OnBannerLoaded(Texture2D tex)
	{
		_banner = tex;
	}

	private void OnIconLoaded(Texture2D tex)
	{
		_icon = tex;
	}
}
