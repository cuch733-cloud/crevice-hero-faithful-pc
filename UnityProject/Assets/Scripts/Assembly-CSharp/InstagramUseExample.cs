using System;
using UnityEngine;

public class InstagramUseExample : MonoBehaviour
{
	public Texture2D imageForPosting;

	private void Awake()
	{
		SPInstagram instance = SA_Singleton<SPInstagram>.instance;
		instance.OnPostingCompleteAction = (Action<InstagramPostResult>)Delegate.Combine(instance.OnPostingCompleteAction, new Action<InstagramPostResult>(OnPostingCompleteAction));
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(30f, 70f, 180f, 50f), "Share Image"))
		{
			SA_Singleton<SPInstagram>.instance.Share(imageForPosting);
		}
		if (GUI.Button(new Rect(250f, 70f, 180f, 50f), "Share Image With Message"))
		{
			SA_Singleton<SPInstagram>.instance.Share(imageForPosting, "I am posting from my app");
		}
	}

	private void OnPostingCompleteAction(InstagramPostResult result)
	{
		if (result == InstagramPostResult.RESULT_OK)
		{
			Debug.Log("Posting Successful");
		}
		else
		{
			Debug.Log("Posting failed with error code " + result);
		}
	}
}
