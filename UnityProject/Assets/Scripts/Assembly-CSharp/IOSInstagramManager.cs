using System;
using UnityEngine;

public class IOSInstagramManager : SA_Singleton<IOSInstagramManager>
{
	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Share(Texture2D texture)
	{
		Share(texture, string.Empty);
	}

	public void Share(Texture2D texture, string message)
	{
	}

	private void OnPostSuccess()
	{
		dispatch("post_succeeded");
	}

	private void OnPostFailed(string data)
	{
		int num = Convert.ToInt32(data);
		InstagramPostResult instagramPostResult = InstagramPostResult.NO_APPLICATION_INSTALLED;
		switch (num)
		{
		case 1:
			instagramPostResult = InstagramPostResult.NO_APPLICATION_INSTALLED;
			break;
		case 2:
			instagramPostResult = InstagramPostResult.USER_CANCELLED;
			break;
		case 3:
			instagramPostResult = InstagramPostResult.SYSTEM_VERSION_ERROR;
			break;
		}
		dispatch("post_failed", instagramPostResult);
	}
}
