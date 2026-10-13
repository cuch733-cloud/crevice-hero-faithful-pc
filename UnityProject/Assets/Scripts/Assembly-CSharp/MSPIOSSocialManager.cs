using UnionAssets.FLE;
using UnityEngine;

public class MSPIOSSocialManager : EventDispatcher
{
	public const string TWITTER_POST_FAILED = "twitter_post_failed";

	public const string TWITTER_POST_SUCCESS = "twitter_post_success";

	public const string FACEBOOK_POST_FAILED = "facebook_post_failed";

	public const string FACEBOOK_POST_SUCCESS = "facebook_post_success";

	private static MSPIOSSocialManager _instance;

	public static MSPIOSSocialManager instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("IOSSocialManager");
				_instance = gameObject.AddComponent<MSPIOSSocialManager>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void ShareMedia(string text)
	{
		ShareMedia(text, null);
	}

	public void ShareMedia(string text, Texture2D texture)
	{
	}

	public void TwitterPost(string text)
	{
		TwitterPost(text, null);
	}

	public void TwitterPost(string text, Texture2D texture)
	{
	}

	public void FacebookPost(string text)
	{
		FacebookPost(text, null);
	}

	public void FacebookPost(string text, Texture2D texture)
	{
	}

	public void SendMail(string subject, string body, string recipients)
	{
		SendMail(subject, body, recipients, null);
	}

	public void SendMail(string subject, string body, string recipients, Texture2D texture)
	{
		if (!(texture == null))
		{
		}
	}

	private void OnTwitterPostFailed()
	{
		dispatch("twitter_post_failed");
	}

	private void OnTwitterPostSuccess()
	{
		dispatch("twitter_post_success");
	}

	private void OnFacebookPostFailed()
	{
		dispatch("facebook_post_failed");
	}

	private void OnFacebookPostSuccess()
	{
		dispatch("facebook_post_success");
	}
}
