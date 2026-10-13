using System;
using SimpleJSON;
using UnityEngine;

public class PlayerManager
{
	private PlayerData[] data = new PlayerData[16];

	public int[] gemIDs = new int[16];

	private static PlayerManager _Instance;

	public static PlayerManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new PlayerManager();
			}
			return _Instance;
		}
	}

	public int GetGemIdByID(int id)
	{
		return gemIDs[id];
	}

	public int GetPlayerSpeed(int playerID, int level)
	{
		return data[playerID].speed + GameConfig.addedSpeed[level];
	}

	public void Load()
	{
		Util.d("PlayerManager.Load");
		TextAsset textAsset = Resources.Load("char") as TextAsset;
		JSONNode jSONNode = null;
		try
		{
			jSONNode = JSON.Parse(Util.dc(textAsset.text));
		}
		catch (Exception)
		{
			GameController.Instance.ExitOnError("PlayerData.Load");
			return;
		}
		if (jSONNode == null)
		{
			GameController.Instance.ExitOnError("PlayerData.Load");
			return;
		}
		JSONNode jSONNode2 = jSONNode["chars"];
		for (int i = 0; i < 16 && i < jSONNode2.Count; i++)
		{
			data[i] = new PlayerData();
			data[i].Parse(jSONNode2[i]);
		}
	}

	public PlayerData GetDataByID(int id)
	{
		return data[id];
	}
}
