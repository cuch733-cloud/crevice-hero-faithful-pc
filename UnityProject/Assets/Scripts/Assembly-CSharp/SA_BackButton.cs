using UnityEngine;

public class SA_BackButton : DefaultPreviewButton
{
	public static string firstLevel = string.Empty;

	private void Start()
	{
		if (firstLevel != string.Empty)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (firstLevel == string.Empty)
		{
			firstLevel = Application.loadedLevelName;
		}
	}

	private void FixedUpdate()
	{
		if (Application.loadedLevelName.Equals(firstLevel))
		{
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
		}
		else
		{
			GetComponent<Renderer>().enabled = true;
			GetComponent<Collider>().enabled = true;
		}
	}

	protected override void OnClick()
	{
		base.OnClick();
		GoBack();
	}

	private void GoBack()
	{
		SA_Singleton<SALevelLoader>.instance.LoadLevel(firstLevel);
	}
}
