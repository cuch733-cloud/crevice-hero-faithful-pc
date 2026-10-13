using System.Collections.Generic;
using UnityEngine;

public class ISDSettings : ScriptableObject
{
	public const string VERSION_NUMBER = "1.0";

	private const string ISDAssetPath = "Extensions/IOSDeploy/Resources";

	private const string ISDAssetName = "ISDSettingsResource";

	private const string ISDAssetExtension = ".asset";

	public bool IsfwSettingOpen;

	public bool IslinkerSettingOpne;

	public bool IscompilerSettingsOpen = true;

	public List<string> frameworks = new List<string>();

	public List<string> compileFlags = new List<string>();

	public List<string> linkFlags = new List<string>();

	private static ISDSettings instance;

	public static ISDSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load("ISDSettingsResource") as ISDSettings;
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<ISDSettings>();
				}
			}
			return instance;
		}
	}
}
