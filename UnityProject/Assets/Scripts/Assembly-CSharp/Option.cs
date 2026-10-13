using SimpleJSON;
using UnityEngine;

public class Option
{
	private static Option _Instance = null;

	private JSONNode root;

	private static string[] keys = new string[4] { "sound", "vibration", "lang", "last_player" };

	public static Option Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new Option();
			}
			return _Instance;
		}
	}

	public void Save()
	{
		if (root != null)
		{
			Util.d("Option SAVE: " + root.ToString());
			PlayerPrefs.SetString("OPTION", root.ToString());
		}
	}

	public int Get(OptionData opt)
	{
		string aKey = keys[(int)opt];
		return root[aKey].AsInt;
	}

	public void Set(OptionData opt, int val)
	{
		string aKey = keys[(int)opt];
		root[aKey].AsInt = val;
	}

	public void Load()
	{
		string text = PlayerPrefs.GetString("OPTION");
		Util.d("Option LOAD: " + text);
		if (text.Length == 0)
		{
			text = "{}";
			root = JSON.Parse(text);
			Set(OptionData.SOUND, 1);
			Set(OptionData.VIBRATION, 0);
			if (Application.systemLanguage == SystemLanguage.Korean)
			{
				Set(OptionData.LANG, 1);
			}
			else
			{
				Set(OptionData.LANG, 0);
			}
			Set(OptionData.LAST_PLAYER, 0);
		}
		else
		{
			root = JSON.Parse(text);
		}
	}
}
