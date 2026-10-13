using System.Collections;
using UnionAssets.FLE;
using UnityEngine;

public class MSPTwitterUseExample : MonoBehaviour
{
	private static string TWITTER_CONSUMER_KEY = "wEvDyAUr2QabVAsWPDiGwg";

	private static string TWITTER_CONSUMER_SECRET = "igRxZbOrkLQPNLSvibNC3mdNJ5tOlVOPH3HNNKDY0";

	private static bool IsUserInfoLoaded;

	private static bool IsAuntifivated;

	public Texture2D ImageToShare;

	public DefaultPreviewButton connectButton;

	public SA_Texture avatar;

	public SA_Label Location;

	public SA_Label Language;

	public SA_Label Status;

	public SA_Label Name;

	public DefaultPreviewButton[] AuthDependedButtons;

	private void Awake()
	{
		SPTwitter.instance.addEventListener("twitter_inited", OnInit);
		SPTwitter.instance.addEventListener("authentication_succeeded", OnAuth);
		SPTwitter.instance.addEventListener("post_succeeded", OnPost);
		SPTwitter.instance.addEventListener("post_failed", OnPostFailed);
		SPTwitter.instance.addEventListener("user_data_loaded", OnUserDataLoaded);
		SPTwitter.instance.addEventListener("user_data_failed_to_load", OnUserDataLoadFailed);
		SPTwitter.instance.Init(TWITTER_CONSUMER_KEY, TWITTER_CONSUMER_SECRET);
	}

	private void FixedUpdate()
	{
		if (IsAuntifivated)
		{
			connectButton.text = "Disconnect";
			Name.text = "Player Connected";
			DefaultPreviewButton[] authDependedButtons = AuthDependedButtons;
			foreach (DefaultPreviewButton defaultPreviewButton in authDependedButtons)
			{
				defaultPreviewButton.EnabledButton();
			}
			if (IsUserInfoLoaded)
			{
				if (SPTwitter.instance.userInfo.profile_image != null)
				{
					avatar.texture = SPTwitter.instance.userInfo.profile_image;
				}
				Name.text = SPTwitter.instance.userInfo.name + " aka " + SPTwitter.instance.userInfo.screen_name;
				Location.text = SPTwitter.instance.userInfo.location;
				Language.text = SPTwitter.instance.userInfo.lang;
				Status.text = SPTwitter.instance.userInfo.status.text;
			}
		}
		else
		{
			DefaultPreviewButton[] authDependedButtons2 = AuthDependedButtons;
			foreach (DefaultPreviewButton defaultPreviewButton2 in authDependedButtons2)
			{
				defaultPreviewButton2.DisabledButton();
			}
			connectButton.text = "Connect";
			Name.text = "Player Disconnected";
		}
	}

	private void Connect()
	{
		if (!IsAuntifivated)
		{
			SPTwitter.instance.AuthenticateUser();
		}
		else
		{
			LogOut();
		}
	}

	private void PostWithAuthCheck()
	{
		SPTwitter.instance.PostWithAuthCheck("Hello, I'am posting this from my app");
	}

	private void PostNativeScreenshot()
	{
		StartCoroutine(PostTWScreenshot());
	}

	private void PostMSG()
	{
		SPShareUtility.TwitterShare("This is my text to share");
	}

	private void PostImage()
	{
		SPShareUtility.TwitterShare("This is my text to share", ImageToShare);
	}

	private IEnumerator PostTWScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		SPShareUtility.TwitterShare("This is my text to share", tex);
		Object.Destroy(tex);
	}

	private void LoadUserData()
	{
		SPTwitter.instance.LoadUserData();
	}

	private void PostMessage()
	{
		SPTwitter.instance.Post("Hello, I'am posting this from my app");
	}

	private void PostScreehShot()
	{
		StartCoroutine(PostScreenshot());
	}

	private void OnUserDataLoadFailed()
	{
		Debug.Log("Opps, user data load failed, something was wrong");
	}

	private void OnUserDataLoaded()
	{
		IsUserInfoLoaded = true;
		SPTwitter.instance.userInfo.LoadProfileImage();
		SPTwitter.instance.userInfo.LoadBackgroundImage();
	}

	private void OnPost()
	{
		Debug.Log("Congrats, you just postet something to twitter");
	}

	private void OnPostFailed()
	{
		Debug.Log("Opps, post failed, something was wrong");
	}

	private void OnInit()
	{
		if (SPTwitter.instance.IsAuthed)
		{
			OnAuth();
		}
	}

	private void OnAuth()
	{
		IsAuntifivated = true;
	}

	private void RetrieveTimeLine()
	{
		TW_UserTimeLineRequest tW_UserTimeLineRequest = TW_UserTimeLineRequest.Create();
		tW_UserTimeLineRequest.addEventListener("complete", OnTimeLineRequestComplete);
		tW_UserTimeLineRequest.AddParam("screen_name", "unity3d");
		tW_UserTimeLineRequest.AddParam("count", "1");
		tW_UserTimeLineRequest.Send();
	}

	private void UserLookUpRequest()
	{
		TW_UsersLookUpRequest tW_UsersLookUpRequest = TW_UsersLookUpRequest.Create();
		tW_UsersLookUpRequest.addEventListener("complete", OnLookUpRequestComplete);
		tW_UsersLookUpRequest.AddParam("screen_name", "unity3d");
		tW_UsersLookUpRequest.Send();
	}

	private void FriedsidsRequest()
	{
		TW_FriendsIdsRequest tW_FriendsIdsRequest = TW_FriendsIdsRequest.Create();
		tW_FriendsIdsRequest.addEventListener("complete", OnIdsLoaded);
		tW_FriendsIdsRequest.AddParam("screen_name", "unity3d");
		tW_FriendsIdsRequest.Send();
	}

	private void FollowersidsRequest()
	{
		TW_FollowersIdsRequest tW_FollowersIdsRequest = TW_FollowersIdsRequest.Create();
		tW_FollowersIdsRequest.addEventListener("complete", OnIdsLoaded);
		tW_FollowersIdsRequest.AddParam("screen_name", "unity3d");
		tW_FollowersIdsRequest.Send();
	}

	private void TweetSearch()
	{
		TW_SearchTweetsRequest tW_SearchTweetsRequest = TW_SearchTweetsRequest.Create();
		tW_SearchTweetsRequest.addEventListener("complete", OnSearchRequestComplete);
		tW_SearchTweetsRequest.AddParam("q", "@noradio");
		tW_SearchTweetsRequest.AddParam("count", "1");
		tW_SearchTweetsRequest.Send();
	}

	private void OnIdsLoaded(CEvent e)
	{
		TW_APIRequstResult tW_APIRequstResult = e.data as TW_APIRequstResult;
		if (tW_APIRequstResult.IsSucceeded)
		{
			Debug.Log("Totals ids loaded: " + tW_APIRequstResult.ids.Count);
		}
		else
		{
			Debug.Log(tW_APIRequstResult.responce);
		}
	}

	private void OnLookUpRequestComplete(CEvent e)
	{
		TW_APIRequstResult tW_APIRequstResult = e.data as TW_APIRequstResult;
		if (tW_APIRequstResult.IsSucceeded)
		{
			string text = "User Id: ";
			text += tW_APIRequstResult.users[0].id;
			text += "\n";
			text = text + "User Name:" + tW_APIRequstResult.users[0].name;
			Debug.Log(text);
		}
		else
		{
			Debug.Log(tW_APIRequstResult.responce);
		}
	}

	private void OnSearchRequestComplete(CEvent e)
	{
		TW_APIRequstResult tW_APIRequstResult = e.data as TW_APIRequstResult;
		if (tW_APIRequstResult.IsSucceeded)
		{
			string text = "Tweet text:\n";
			text += tW_APIRequstResult.tweets[0].text;
			Debug.Log(text);
		}
		else
		{
			Debug.Log(tW_APIRequstResult.responce);
		}
	}

	private void OnTimeLineRequestComplete(CEvent e)
	{
		TW_APIRequstResult tW_APIRequstResult = e.data as TW_APIRequstResult;
		if (tW_APIRequstResult.IsSucceeded)
		{
			string text;
			if (tW_APIRequstResult.tweets.Count > 0)
			{
				text = "Last Tweet text:\n";
				text += tW_APIRequstResult.tweets[0].text;
			}
			else
			{
				text = "NO tweens found";
			}
			Debug.Log(text);
		}
		else
		{
			Debug.Log(tW_APIRequstResult.responce);
		}
	}

	private IEnumerator PostScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		SPTwitter.instance.Post("My app ScreehShot", tex);
		Object.Destroy(tex);
	}

	private void LogOut()
	{
		IsUserInfoLoaded = false;
		IsAuntifivated = false;
		SPTwitter.instance.LogOut();
	}
}
