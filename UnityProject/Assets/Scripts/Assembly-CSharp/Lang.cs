using System.Collections.Generic;
using UnityEngine;

public class Lang
{
	public const string QUIT_PLAY = "quit_play";

	public const string QUIT_GAME = "quit_game";

	public const string DISTANCE = "distance";

	public const string MAX_DISTANCE = "max_distance";

	public const string LANGUAGE = "language";

	public const string LANGUAGE_VAL = "language_val";

	public const string NOT_ENOUGH_GEM = "not_enough_gem";

	public const string TO_BE_UPDATED = "to_be_updated";

	public const string LEVEL = "level";

	public const string COOLTIME = "cooltime";

	public const string SPEED = "speed";

	private string LANG_PATH = "lang";

	private static Lang _Instance;

	private Dictionary<string, string> texts = new Dictionary<string, string>();

	public static Lang Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new Lang();
			}
			return _Instance;
		}
	}

	public string GetResource(string resource)
	{
		if (Option.Instance.Get(OptionData.LANG) == 0)
		{
			return resource;
		}
		return resource + "_ko";
	}

	public void Load()
	{
		Util.d("Lang.Load");
		TextAsset textAsset = Resources.Load(LANG_PATH) as TextAsset;
		string[,] array = CsvParser.SplitCsvGrid(textAsset.text);
		int num = array.GetUpperBound(1) - 1;
		texts.Clear();
		int num2 = ((Option.Instance.Get(OptionData.LANG) != 0) ? 1 : 2);
		for (int i = 1; i < num; i++)
		{
			string value = array[num2, i];
			texts.Add(array[0, i], value);
		}
	}

	public string Get(string key)
	{
		string value;
		if (texts.TryGetValue(key, out value))
		{
			return value;
		}
		return string.Empty;
	}
}
