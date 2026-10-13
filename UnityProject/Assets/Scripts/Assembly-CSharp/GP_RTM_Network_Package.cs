using System;
using System.Collections.Generic;
using UnityEngine;

public class GP_RTM_Network_Package
{
	private const int BYTE_LIMIT = 256;

	private string _playerId;

	private byte[] _buffer;

	public string participantId
	{
		get
		{
			return _playerId;
		}
	}

	public byte[] buffer
	{
		get
		{
			return _buffer;
		}
	}

	public GP_RTM_Network_Package(string player, string recievedData)
	{
		_playerId = player;
		Debug.Log("GOOGLE_PLAY_RESULT -> OnMatchDataRecieved " + recievedData);
		_buffer = ConvertStringToByteData(recievedData);
	}

	public static byte[] ConvertStringToByteData(string data)
	{
		if (data == null)
		{
			return null;
		}
		data = data.Replace("endofline", string.Empty);
		if (data.Equals(string.Empty))
		{
			return null;
		}
		string[] array = data.Split(","[0]);
		List<byte> list = new List<byte>();
		string[] array2 = array;
		foreach (string value in array2)
		{
			int num = Convert.ToInt32(value);
			int value2 = ((num >= 0) ? num : (256 + num));
			list.Add(Convert.ToByte(value2));
		}
		return list.ToArray();
	}
}
