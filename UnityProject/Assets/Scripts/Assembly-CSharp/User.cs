using System;
using SimpleJSON;
using UnityEngine;

public class User
{
	private string ADS_REMOVED = "ads_removed";

	private string MAX_DISTANCE = "max_distance";

	private string GEM_COUNTS = "gem_count_";

	private string CHAR_LEVELS = "char_level_";

	private string PLAY_COUNT = "play_count";

	private string GPS_LOGIN = "gps_login";

	private string RATED = "rated";

	private string LINK_IDX = "link_idx";

	private string GAME_LINK_IDX = "gal_idx";

	private bool adsRemoved;

	private int totalPlayCount;

	private int maxDistance;

	private int[] gemCounts = new int[16];

	public int[] playerLevels = new int[16];

	public int gpsLoginState;

	private bool rated;

	public int linkGameIdx;

	public int linkIdx;

	private static User _Instance;

	public static User Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new User();
			}
			return _Instance;
		}
	}

	public int GetTotalPlayCount()
	{
		return totalPlayCount;
	}

	public int GetGemCount(int id)
	{
		return gemCounts[id];
	}

	public void AddGem(int id, int count)
	{
		gemCounts[id] += count;
		if (gemCounts[id] < 0)
		{
			gemCounts[id] = 0;
		}
	}

	public bool IsAdsRemoved()
	{
		return adsRemoved;
	}

	public int GetMaxDistance()
	{
		return maxDistance;
	}

	public int GetPlayerLevel(int index)
	{
		return playerLevels[index];
	}

	public void SetPlayerLevel(int index, int level)
	{
		if (level <= 7 && level >= 0)
		{
			playerLevels[index] = level;
		}
	}

	public bool IsRated()
	{
		return rated;
	}

	public void SetRated(bool rated)
	{
		this.rated = rated;
	}

	public void UpdatePlayData(int addedPlayCount)
	{
		totalPlayCount += addedPlayCount;
		int distance = GameController.Instance.GetDistance();
		if (distance > maxDistance)
		{
			maxDistance = distance;
		}
		if ((bool)ServiceAPI.Instance)
		{
			ServiceAPI.Instance.UpdateWithDistance(GameController.Instance.GetPlayerID(), distance);
		}
		if ((bool)AndroidHub.Instance)
		{
			AndroidHub.Instance.LogEvent_Result(distance.ToString(), GameController.Instance.GetPlayerID().ToString(), Map.Instance.GetMapDataID().ToString());
		}
		int[] array = GameController.Instance.GetGemCounts();
		for (int i = 0; i < gemCounts.Length; i++)
		{
			AddGem(i, array[i]);
		}
	}

	public void SaveData()
	{
		JSONNode jSONNode = JSON.Parse("{}");
		jSONNode[MAX_DISTANCE].AsInt = maxDistance;
		jSONNode[ADS_REMOVED].AsInt = (adsRemoved ? 1 : 0);
		jSONNode[PLAY_COUNT].AsInt = totalPlayCount;
		jSONNode[GPS_LOGIN].AsInt = gpsLoginState;
		jSONNode[RATED].AsInt = (rated ? 1 : 0);
		jSONNode[LINK_IDX].AsInt = linkIdx;
		jSONNode[GAME_LINK_IDX].AsInt = linkGameIdx;
		for (int i = 0; i < 16; i++)
		{
			jSONNode[GEM_COUNTS + i].AsInt = gemCounts[i];
			jSONNode[CHAR_LEVELS + i].AsInt = playerLevels[i];
		}
		string textToEncrypt = jSONNode.ToString();
		string value = Util.xs(textToEncrypt);
		PlayerPrefs.SetString("background_image1", value);
	}

	public void LoadData()
	{
		string text = PlayerPrefs.GetString("background_image1");
		text = ((text.Length != 0) ? Util.xs(text) : "{}");
		JSONNode jSONNode;
		try
		{
			jSONNode = JSON.Parse(text);
		}
		catch (Exception)
		{
			jSONNode = JSON.Parse("{}");
		}
		if (jSONNode == null)
		{
			Util.d("LoadData error!!!!");
			jSONNode = JSON.Parse("{}");
		}
		maxDistance = jSONNode[MAX_DISTANCE].AsInt;
		Util.d("max distance " + maxDistance);
		totalPlayCount = jSONNode[PLAY_COUNT].AsInt;
		Util.d("play count " + totalPlayCount);
		adsRemoved = jSONNode[ADS_REMOVED].AsInt == 1;
		Util.d("ads removed" + adsRemoved);
		gpsLoginState = jSONNode[GPS_LOGIN].AsInt;
		Util.d("gps login" + gpsLoginState);
		rated = jSONNode[RATED].AsInt == 1;
		linkIdx = jSONNode[LINK_IDX].AsInt;
		linkGameIdx = jSONNode[GAME_LINK_IDX].AsInt;
		for (int i = 0; i < 16; i++)
		{
			gemCounts[i] = jSONNode[GEM_COUNTS + i].AsInt;
			int num = jSONNode[CHAR_LEVELS + i].AsInt;
			if (i == 0 && num < 1)
			{
				num = 1;
			}
			if (num > 7)
			{
				num = 7;
			}
			playerLevels[i] = num;
		}
	}

	public bool IsUpgradable(int playerID)
	{
		int num = playerLevels[playerID];
		PlayerData dataByID = PlayerManager.Instance.GetDataByID(playerID);
		switch (num)
		{
		case 7:
			return false;
		case 0:
			if (dataByID.GetCash() >= 1)
			{
				return true;
			}
			break;
		}
		int num2 = gemCounts[playerID];
		int levelupGems = dataByID.GetLevelupGems(num);
		if (num2 >= levelupGems)
		{
			return true;
		}
		return false;
	}

	public void ClearData()
	{
		PlayerPrefs.SetString("background_image1", string.Empty);
		LoadData();
		SaveData();
	}

	public void AddGems(int count)
	{
		for (int i = 0; i < 16; i++)
		{
			gemCounts[i] += count;
		}
	}

	public void UnlockPlayers()
	{
		for (int i = 0; i < 16; i++)
		{
			if (playerLevels[i] == 0)
			{
				playerLevels[i] = 1;
			}
		}
	}

	public void UpdateWithSku(string sku)
	{
		if (sku.Equals("item_no_ads"))
		{
			adsRemoved = true;
			GameController.Instance.UpdateWithAdsOption();
		}
		else if (sku.StartsWith("item_char_"))
		{
			adsRemoved = true;
			GameController.Instance.UpdateWithAdsOption();
			string s = sku.Substring("item_char_".Length);
			int result;
			if (int.TryParse(s, out result) && result >= 0 && result <= 16 && playerLevels[result] == 0)
			{
				Util.d("UpdateWithSku " + result);
				playerLevels[result] = 1;
			}
		}
	}
}
