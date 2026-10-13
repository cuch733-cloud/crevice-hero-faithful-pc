using UnionAssets.FLE;
using UnityEngine;

public abstract class SA_Singleton<T> : EventDispatcher where T : MonoBehaviour
{
	private static T _instance;

	private static bool applicationIsQuitting;

	public static T instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				return (T)null;
			}
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(T)) as T;
				if (_instance == null)
				{
					_instance = new GameObject().AddComponent<T>();
					_instance.gameObject.name = _instance.GetType().Name;
				}
			}
			return _instance;
		}
	}

	public static bool HasInstance
	{
		get
		{
			return !IsDestroyed;
		}
	}

	public static bool IsDestroyed
	{
		get
		{
			if (_instance == null)
			{
				return true;
			}
			return false;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_instance = (T)null;
		applicationIsQuitting = true;
	}

	protected virtual void OnApplicationQuit()
	{
		_instance = (T)null;
		applicationIsQuitting = true;
	}
}
