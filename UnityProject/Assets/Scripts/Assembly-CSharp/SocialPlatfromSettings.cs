using System.Collections.Generic;
using UnityEngine;

public class SocialPlatfromSettings : ScriptableObject
{
	public const string VERSION_NUMBER = "4.2";

	public const string FB_SDK_VERSION_NUMBER = "6.1.0";

	private const string ISNSettingsAssetName = "SocialSettings";

	private const string ISNSettingsPath = "Extensions/GooglePlayCommon/Resources";

	private const string ISNSettingsAssetExtension = ".asset";

	public bool showPermitions = true;

	public bool ShowActions = true;

	public bool ShowAPIS = true;

	public List<string> fb_scopes_list = new List<string>();

	public string TWITTER_CONSUMER_KEY = "REPLACE_WITH_YOUR_CONSUMER_KEY";

	public string TWITTER_CONSUMER_SECRET = "REPLACE_WITH_YOUR_CONSUMER_SECRET";

	public bool TwitterAPI = true;

	public bool NativeSharingAPI = true;

	public bool InstagramAPI = true;

	public bool KeepManifestClean = true;

	private static SocialPlatfromSettings instance;

	public static SocialPlatfromSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load("SocialSettings") as SocialPlatfromSettings;
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<SocialPlatfromSettings>();
				}
			}
			return instance;
		}
	}

	public string fb_scopes
	{
		get
		{
			string text = string.Empty;
			foreach (string item in fb_scopes_list)
			{
				text = text + item + ",";
			}
			if (text.Length > 0)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}
	}

	public void AddDefaultScopes()
	{
		instance.fb_scopes_list.Add("user_about_me");
		instance.fb_scopes_list.Add("user_friends");
		instance.fb_scopes_list.Add("email");
		instance.fb_scopes_list.Add("publish_actions");
		instance.fb_scopes_list.Add("read_friendlists");
		instance.fb_scopes_list.Add("user_games_activity");
		instance.fb_scopes_list.Add("user_activities");
		instance.fb_scopes_list.Add("user_likes");
	}
}
