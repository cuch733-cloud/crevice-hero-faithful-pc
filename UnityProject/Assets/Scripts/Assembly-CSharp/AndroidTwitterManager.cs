using System;
using UnionAssets.FLE;
using UnityEngine;

public class AndroidTwitterManager : SA_Singleton<AndroidTwitterManager>, IDispatcher, TwitterManagerInterface
{
	private bool _IsAuthed;

	private bool _IsInited;

	private TwitterUserInfo _userInfo;

	public Action<TWResult> OnTwitterInitedAction = delegate
	{
	};

	public Action<TWResult> OnAuthCompleteAction = delegate
	{
	};

	public Action<TWResult> OnPostingCompleteAction = delegate
	{
	};

	public Action<TWResult> OnUserDataRequestCompleteAction = delegate
	{
	};

	public bool IsAuthed
	{
		get
		{
			return _IsAuthed;
		}
	}

	public bool IsInited
	{
		get
		{
			return _IsInited;
		}
	}

	public TwitterUserInfo userInfo
	{
		get
		{
			return _userInfo;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Init()
	{
		Init(SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY, SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET);
	}

	public void Init(string consumer_key, string consumer_secret)
	{
		if (!_IsInited)
		{
			_IsInited = true;
			AndroidNative.TwitterInit(consumer_key, consumer_secret);
		}
	}

	public void AuthenticateUser()
	{
		if (_IsAuthed)
		{
			OnAuthSuccess();
		}
		else
		{
			AndroidNative.AuthificateUser();
		}
	}

	public void LoadUserData()
	{
		if (_IsAuthed)
		{
			AndroidNative.LoadUserData();
			return;
		}
		Debug.LogWarning("Auth user before loadin data, fail event generated");
		TWResult tWResult = new TWResult(false, null);
		dispatch("user_data_failed_to_load", tWResult);
		OnUserDataRequestCompleteAction(tWResult);
	}

	public void Post(string status)
	{
		if (!_IsAuthed)
		{
			Debug.LogWarning("Auth user before posting data, fail event generated");
			TWResult tWResult = new TWResult(false, null);
			dispatch("post_failed", tWResult);
			OnPostingCompleteAction(tWResult);
		}
		else
		{
			AndroidNative.TwitterPost(status);
		}
	}

	public void Post(string status, Texture2D texture)
	{
		if (!_IsAuthed)
		{
			Debug.LogWarning("Auth user before posting data, fail event generated");
			TWResult tWResult = new TWResult(false, null);
			dispatch("post_failed", tWResult);
			OnPostingCompleteAction(tWResult);
		}
		else
		{
			byte[] inArray = texture.EncodeToPNG();
			string data = Convert.ToBase64String(inArray);
			AndroidNative.TwitterPostWithImage(status, data);
		}
	}

	public TwitterPostingTask PostWithAuthCheck(string status)
	{
		return PostWithAuthCheck(status, null);
	}

	public TwitterPostingTask PostWithAuthCheck(string status, Texture2D texture)
	{
		TwitterPostingTask twitterPostingTask = TwitterPostingTask.Cretae();
		twitterPostingTask.Post(status, texture, this);
		return twitterPostingTask;
	}

	public void LogOut()
	{
		_IsAuthed = false;
		AndroidNative.LogoutFromTwitter();
	}

	private void OnInited(string data)
	{
		if (data.Equals("1"))
		{
			_IsAuthed = true;
		}
		TWResult tWResult = new TWResult(true, null);
		dispatch("twitter_inited", tWResult);
		OnTwitterInitedAction(tWResult);
	}

	private void OnAuthSuccess()
	{
		_IsAuthed = true;
		TWResult tWResult = new TWResult(true, null);
		dispatch("authentication_succeeded", tWResult);
		OnAuthCompleteAction(tWResult);
	}

	private void OnAuthFailed()
	{
		TWResult tWResult = new TWResult(false, null);
		dispatch("authentication_failed", tWResult);
		OnAuthCompleteAction(tWResult);
	}

	private void OnPostSuccess()
	{
		TWResult tWResult = new TWResult(true, null);
		dispatch("post_succeeded", tWResult);
		OnPostingCompleteAction(tWResult);
	}

	private void OnPostFailed()
	{
		TWResult tWResult = new TWResult(false, null);
		dispatch("post_failed", tWResult);
		OnPostingCompleteAction(tWResult);
	}

	private void OnUserDataLoaded(string data)
	{
		_userInfo = new TwitterUserInfo(data);
		TWResult tWResult = new TWResult(true, data);
		dispatch("user_data_loaded", tWResult);
		OnUserDataRequestCompleteAction(tWResult);
	}

	private void OnUserDataLoadFailed()
	{
		TWResult tWResult = new TWResult(false, null);
		dispatch("user_data_failed_to_load", tWResult);
		OnUserDataRequestCompleteAction(tWResult);
	}
}
