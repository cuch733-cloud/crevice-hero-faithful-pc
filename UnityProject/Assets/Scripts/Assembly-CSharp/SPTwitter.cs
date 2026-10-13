using System;
using UnionAssets.FLE;
using UnityEngine;

public class SPTwitter : MonoBehaviour, IDispatcher, TwitterManagerInterface
{
	private static SPTwitter _instance;

	private static TwitterManagerInterface _twitterGate;

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

	public static SPTwitter instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject("SPTwitter").AddComponent<SPTwitter>();
			}
			return _instance;
		}
	}

	public bool IsAuthed
	{
		get
		{
			return _twitterGate.IsAuthed;
		}
	}

	public bool IsInited
	{
		get
		{
			return _twitterGate.IsInited;
		}
	}

	public TwitterUserInfo userInfo
	{
		get
		{
			return _twitterGate.userInfo;
		}
	}

	private void Awake()
	{
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.Android)
		{
			_twitterGate = SA_Singleton<AndroidTwitterManager>.instance;
			AndroidTwitterManager androidTwitterManager = SA_Singleton<AndroidTwitterManager>.instance;
			androidTwitterManager.OnTwitterInitedAction = (Action<TWResult>)Delegate.Combine(androidTwitterManager.OnTwitterInitedAction, new Action<TWResult>(OnTwitterInitedActionMethod));
			AndroidTwitterManager androidTwitterManager2 = SA_Singleton<AndroidTwitterManager>.instance;
			androidTwitterManager2.OnAuthCompleteAction = (Action<TWResult>)Delegate.Combine(androidTwitterManager2.OnAuthCompleteAction, new Action<TWResult>(OnAuthCompleteActionMethod));
			AndroidTwitterManager androidTwitterManager3 = SA_Singleton<AndroidTwitterManager>.instance;
			androidTwitterManager3.OnPostingCompleteAction = (Action<TWResult>)Delegate.Combine(androidTwitterManager3.OnPostingCompleteAction, new Action<TWResult>(OnPostingCompleteActionMethod));
			AndroidTwitterManager androidTwitterManager4 = SA_Singleton<AndroidTwitterManager>.instance;
			androidTwitterManager4.OnUserDataRequestCompleteAction = (Action<TWResult>)Delegate.Combine(androidTwitterManager4.OnUserDataRequestCompleteAction, new Action<TWResult>(OnUserDataRequestCompleteActionMethod));
		}
		else
		{
			_twitterGate = SA_Singleton<IOSTwitterManager>.instance;
			IOSTwitterManager iOSTwitterManager = SA_Singleton<IOSTwitterManager>.instance;
			iOSTwitterManager.OnTwitterInitedAction = (Action<TWResult>)Delegate.Combine(iOSTwitterManager.OnTwitterInitedAction, new Action<TWResult>(OnTwitterInitedActionMethod));
			IOSTwitterManager iOSTwitterManager2 = SA_Singleton<IOSTwitterManager>.instance;
			iOSTwitterManager2.OnAuthCompleteAction = (Action<TWResult>)Delegate.Combine(iOSTwitterManager2.OnAuthCompleteAction, new Action<TWResult>(OnAuthCompleteActionMethod));
			IOSTwitterManager iOSTwitterManager3 = SA_Singleton<IOSTwitterManager>.instance;
			iOSTwitterManager3.OnPostingCompleteAction = (Action<TWResult>)Delegate.Combine(iOSTwitterManager3.OnPostingCompleteAction, new Action<TWResult>(OnPostingCompleteActionMethod));
			IOSTwitterManager iOSTwitterManager4 = SA_Singleton<IOSTwitterManager>.instance;
			iOSTwitterManager4.OnUserDataRequestCompleteAction = (Action<TWResult>)Delegate.Combine(iOSTwitterManager4.OnUserDataRequestCompleteAction, new Action<TWResult>(OnUserDataRequestCompleteActionMethod));
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Init()
	{
		_twitterGate.Init();
	}

	public void Init(string consumer_key, string consumer_secret)
	{
		_twitterGate.Init(consumer_key, consumer_secret);
	}

	public void AuthenticateUser()
	{
		if (_twitterGate != null)
		{
			_twitterGate.AuthenticateUser();
		}
		else
		{
			Debug.Log("SPTwitter.Init should be called before SPTwitter.AuthificateUser");
		}
	}

	public void LoadUserData()
	{
		if (_twitterGate != null)
		{
			_twitterGate.LoadUserData();
		}
		else
		{
			Debug.Log("SPTwitter.Init should be called before SPTwitter.LoadUserData");
		}
	}

	public void Post(string status)
	{
		if (_twitterGate != null)
		{
			_twitterGate.Post(status);
		}
		else
		{
			Debug.Log("SPTwitter.Init should be called before SPTwitter.Post");
		}
	}

	public void Post(string status, Texture2D texture)
	{
		if (_twitterGate != null)
		{
			_twitterGate.Post(status, texture);
		}
		else
		{
			Debug.Log("SPTwitter.Init should be called before SPTwitter.Post");
		}
	}

	public TwitterPostingTask PostWithAuthCheck(string status)
	{
		return PostWithAuthCheck(status, null);
	}

	public TwitterPostingTask PostWithAuthCheck(string status, Texture2D texture)
	{
		if (_twitterGate != null)
		{
			return _twitterGate.PostWithAuthCheck(status, texture);
		}
		Debug.Log("SPTwitter.Init should be called before SPTwitter.Post");
		return null;
	}

	public void LogOut()
	{
		if (_twitterGate != null)
		{
			_twitterGate.LogOut();
		}
		else
		{
			Debug.Log("SPTwitter.Init should be called before SPTwitter.LogOut");
		}
	}

	public void addEventListener(string eventName, EventHandlerFunction handler)
	{
		_twitterGate.addEventListener(eventName, handler);
	}

	public void addEventListener(string eventName, DataEventHandlerFunction handler)
	{
		_twitterGate.addEventListener(eventName, handler);
	}

	public void addEventListener(int eventID, EventHandlerFunction handler)
	{
		_twitterGate.addEventListener(eventID, handler);
	}

	public void addEventListener(int eventID, DataEventHandlerFunction handler)
	{
		_twitterGate.addEventListener(eventID, handler);
	}

	public void removeEventListener(string eventName, EventHandlerFunction handler)
	{
		_twitterGate.removeEventListener(eventName, handler);
	}

	public void removeEventListener(string eventName, DataEventHandlerFunction handler)
	{
		_twitterGate.removeEventListener(eventName, handler);
	}

	public void removeEventListener(int eventID, EventHandlerFunction handler)
	{
		_twitterGate.removeEventListener(eventID, handler);
	}

	public void removeEventListener(int eventID, DataEventHandlerFunction handler)
	{
		_twitterGate.removeEventListener(eventID, handler);
	}

	public void dispatchEvent(int eventID)
	{
		_twitterGate.dispatchEvent(eventID);
	}

	public void dispatchEvent(int eventID, object data)
	{
		_twitterGate.dispatchEvent(eventID, data);
	}

	public void dispatchEvent(string eventName)
	{
		_twitterGate.dispatchEvent(eventName);
	}

	public void dispatchEvent(string eventName, object data)
	{
		_twitterGate.dispatchEvent(eventName, data);
	}

	public void dispatch(int eventID)
	{
		_twitterGate.dispatch(eventID);
	}

	public void dispatch(int eventID, object data)
	{
		_twitterGate.dispatch(eventID, data);
	}

	public void dispatch(string eventName)
	{
		_twitterGate.dispatch(eventName);
	}

	public void dispatch(string eventName, object data)
	{
		_twitterGate.dispatch(eventName, data);
	}

	private void OnTwitterInitedActionMethod(TWResult obj)
	{
		OnTwitterInitedAction(obj);
	}

	private void OnAuthCompleteActionMethod(TWResult obj)
	{
		OnAuthCompleteAction(obj);
	}

	private void OnPostingCompleteActionMethod(TWResult obj)
	{
		OnPostingCompleteAction(obj);
	}

	private void OnUserDataRequestCompleteActionMethod(TWResult obj)
	{
		OnUserDataRequestCompleteAction(obj);
	}

	public void clearEvents()
	{
		_twitterGate.clearEvents();
	}
}
