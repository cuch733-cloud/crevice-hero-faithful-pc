using System;
using UnityEngine;

public class AndroidInstagramManager : SA_Singleton<AndroidInstagramManager>
{
	public Action<InstagramPostResult> OnPostingCompleteAction = delegate
	{
	};

	public void Share(Texture2D texture)
	{
		Share(texture, string.Empty);
	}

	public void Share(Texture2D texture, string message)
	{
		byte[] inArray = texture.EncodeToPNG();
		string data = Convert.ToBase64String(inArray);
		AndroidNative.InstagramPostImage(data, message);
	}

	private void OnPostSuccess()
	{
		OnPostingCompleteAction(InstagramPostResult.RESULT_OK);
		dispatch("post_succeeded", InstagramPostResult.RESULT_OK);
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
			instagramPostResult = InstagramPostResult.INTERNAL_EXCEPTION;
			break;
		}
		OnPostingCompleteAction(instagramPostResult);
		dispatch("post_failed", instagramPostResult);
	}
}
