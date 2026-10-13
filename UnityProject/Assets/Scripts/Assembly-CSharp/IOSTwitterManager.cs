using System;
using UnionAssets.FLE;
using UnityEngine;

public class IOSTwitterManager : SA_Singleton<IOSTwitterManager>, IDispatcher, TwitterManagerInterface
{
	private const int NO_TWITTER_ACCOUNTS_ON_DEVICE = 0;

	private const int TWITTER_PERMISSION_DENIED = 1;

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

	public void Init(string consumer_key, string consumer_secret)
	{
		Init();
	}

	public void Init()
	{
		if (!_IsInited)
		{
			_IsInited = true;
		}
	}

	public void AuthenticateUser()
	{
	}

	public void LoadUserData()
	{
	}

	public void Post(string status)
	{
	}

	public void Post(string status, Texture2D texture)
	{
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

	private void OnAuthFailed(string data)
	{
		TWResult tWResult = new TWResult(false, data);
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
