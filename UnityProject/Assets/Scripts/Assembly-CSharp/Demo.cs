using System;
using UnityEngine;

public class Demo : MonoBehaviour
{
	private bool expired;

	private void Start()
	{
		GameConfig.TEMP_TEST = true;
		GameConfig.PRINT_DEBUG = true;
		GameConfig.USE_FLURRY = false;
		string format = "yyyy'##'MM'##'dd' 'HH'*'mm'*'ss";
		string s = "2015##01##25 00*00*00";
		DateTime dateTime = DateTime.ParseExact(s, format, null);
		Util.d(dateTime.Ticks + " " + DateTime.Now.Ticks);
		expired = DateTime.Now.Ticks > dateTime.Ticks;
		if (expired)
		{
			Invoke("EXIT", 10f);
		}
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(5f, 5f, 100f, 20f), "DEMO");
		if (!expired || GUI.Button(new Rect(100f, 100f, Screen.width - 200, Screen.height - 200), "DEMO EXPIRED: Quit...."))
		{
		}
		if (!expired && GameController.Instance.GetState() == GameController.STATE.ST_CREDIT)
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
			if (GUI.Button(new Rect(10f, 320f, 150f, 60f), (!GameConfig.SHOW_ADS) ? "Ads on" : "Ads off"))
			{
				GameConfig.SHOW_ADS = !GameConfig.SHOW_ADS;
			}
		}
	}

	private void EXIT()
	{
		Util.d("EXIT: expired");
		Application.Quit();
	}
}
