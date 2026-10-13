using UnityEngine;

public class Cheat : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GameConfig.CHEAT_ENABLED)
		{
			if (GUI.Button(new Rect(10f, 10f, 150f, 60f), "CLEAR DATA"))
			{
				User.Instance.ClearData();
			}
			if (GUI.Button(new Rect(10f, 110f, 150f, 60f), "GEM + 100"))
			{
				User.Instance.AddGems(100);
			}
			if (GUI.Button(new Rect(10f, 210f, 150f, 60f), "Unlock Players"))
			{
				User.Instance.UnlockPlayers();
			}
		}
	}
}
