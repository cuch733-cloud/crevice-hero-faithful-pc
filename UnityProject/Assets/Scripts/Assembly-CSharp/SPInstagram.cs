using System;
using UnionAssets.FLE;
using UnityEngine;

public class SPInstagram : SA_Singleton<SPInstagram>
{
	public Action<InstagramPostResult> OnPostingCompleteAction = delegate
	{
	};

	private void Awake()
	{
		Debug.Log("SPInstagram subscribed");
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			SA_Singleton<IOSInstagramManager>.instance.addEventListener("post_succeeded", OnPost);
			SA_Singleton<IOSInstagramManager>.instance.addEventListener("post_failed", OnPostFailed);
			break;
		case RuntimePlatform.Android:
			SA_Singleton<AndroidInstagramManager>.instance.addEventListener("post_succeeded", OnPost);
			SA_Singleton<AndroidInstagramManager>.instance.addEventListener("post_failed", OnPostFailed);
			break;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Share(Texture2D texture)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			SA_Singleton<IOSInstagramManager>.instance.Share(texture);
			break;
		case RuntimePlatform.Android:
			Debug.Log(SA_Singleton<AndroidInstagramManager>.instance);
			SA_Singleton<AndroidInstagramManager>.instance.Share(texture);
			break;
		case RuntimePlatform.PS3:
		case RuntimePlatform.XBOX360:
			break;
		}
	}

	public void Share(Texture2D texture, string message)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			SA_Singleton<IOSInstagramManager>.instance.Share(texture, message);
			break;
		case RuntimePlatform.Android:
			SA_Singleton<AndroidInstagramManager>.instance.Share(texture, message);
			break;
		case RuntimePlatform.PS3:
		case RuntimePlatform.XBOX360:
			break;
		}
	}

	private void OnPost()
	{
		Debug.Log("SPInstagram OnPost");
		OnPostingCompleteAction(InstagramPostResult.RESULT_OK);
	}

	private void OnPostFailed(CEvent e)
	{
		Debug.Log("SPInstagram OnPostFailed");
		InstagramPostResult obj = (InstagramPostResult)(int)e.data;
		OnPostingCompleteAction(obj);
	}
}
