using UnityEngine;

public class SALevelLoader : SA_Singleton<SALevelLoader>
{
	private Texture2D bg;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void LoadLevel(string name)
	{
		Application.LoadLevel(name);
	}

	public void Restart()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}
}
