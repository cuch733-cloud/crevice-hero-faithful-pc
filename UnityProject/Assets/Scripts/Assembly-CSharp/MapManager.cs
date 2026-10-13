using System;
using UnityEngine;

public class MapManager
{
	private MapData[] mapData;

	private GemData[] gemData;

	private static MapManager _Instance;

	public static MapManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new MapManager();
			}
			return _Instance;
		}
	}

	public void Load()
	{
		Util.d("MapManager.Load");
		TextAsset textAsset = Resources.Load("gem") as TextAsset;
		try
		{
			string[,] array = CsvParser.SplitCsvGrid(Util.dc(textAsset.text));
			int num = array.GetUpperBound(1) - 1;
			this.gemData = new GemData[num - 1];
			for (int i = 1; i < num; i++)
			{
				GemData gemData = new GemData();
				gemData.id = int.Parse(array[0, i]);
				gemData.type = int.Parse(array[1, i]);
				gemData.turns[0] = int.Parse(array[2, i]);
				gemData.turns[1] = int.Parse(array[3, i]);
				gemData.turns[2] = int.Parse(array[4, i]);
				gemData.turns[3] = int.Parse(array[5, i]);
				gemData.turns[4] = int.Parse(array[6, i]);
				this.gemData[i - 1] = gemData;
			}
		}
		catch (Exception)
		{
			GameController.Instance.ExitOnError("GemData.Load");
			return;
		}
		TextAsset textAsset2 = Resources.Load("map") as TextAsset;
		try
		{
			string[,] array2 = CsvParser.SplitCsvGrid(Util.dc(textAsset2.text));
			int num2 = array2.GetUpperBound(1) - 1;
			this.mapData = new MapData[num2 - 1];
			for (int j = 1; j < num2; j++)
			{
				MapData mapData = new MapData();
				mapData.phase = int.Parse(array2[0, j]);
				mapData.downVelY = float.Parse(array2[1, j]);
				mapData.upStopDuration = float.Parse(array2[2, j]);
				mapData.upShakeDuration = float.Parse(array2[3, j]);
				mapData.downStopDuration = float.Parse(array2[4, j]);
				mapData.holeHalfPercent = int.Parse(array2[5, j]);
				mapData.holeLeftPercent = int.Parse(array2[6, j]);
				mapData.holeOutPercent = int.Parse(array2[7, j]);
				mapData.holeBottomPercent = int.Parse(array2[8, j]);
				mapData.gemPattern = int.Parse(array2[9, j]);
				mapData.gemData = this.gemData[mapData.gemPattern];
				mapData.maxHeight = int.Parse(array2[10, j]);
				mapData.offsetY = int.Parse(array2[11, j]);
				this.mapData[j - 1] = mapData;
			}
		}
		catch (Exception)
		{
			GameController.Instance.ExitOnError("MapData.Load");
		}
	}

	public MapData GetData(int id)
	{
		if (id >= mapData.Length)
		{
			return mapData[mapData.Length - 1];
		}
		return mapData[id];
	}
}
