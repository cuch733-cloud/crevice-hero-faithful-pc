using System;
using UnityEngine;

public class MSPMoreActionsExample : MonoBehaviour
{
	public Texture2D imageForPosting;

	private void Awake()
	{
		SPInstagram instance = SA_Singleton<SPInstagram>.instance;
		instance.OnPostingCompleteAction = (Action<InstagramPostResult>)Delegate.Combine(instance.OnPostingCompleteAction, new Action<InstagramPostResult>(OnPostingCompleteAction));
	}

	public void InstaShare()
	{
		SA_Singleton<SPInstagram>.instance.Share(imageForPosting);
	}

	public void InstaShareWithText()
	{
		SA_Singleton<SPInstagram>.instance.Share(imageForPosting, "I am posting from my app");
	}

	public void NativeShare()
	{
		SPShareUtility.ShareMedia("Share Caption", "Share Message");
	}

	public void NativeShareWithImage()
	{
		SPShareUtility.ShareMedia("Share Caption", "Share Message", imageForPosting);
	}

	public void SendMail()
	{
		SPShareUtility.SendMail("My E-mail Subject", "This is my text to share", "mail1@gmail.com, mail2@gmail.com", imageForPosting);
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
