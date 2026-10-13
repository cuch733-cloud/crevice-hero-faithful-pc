using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	private List<GameObject> objs;

	private int avIndex;

	private GameObject prefab;

	private int max;

	public void Init(GameObject prefab, int max, Transform parentTf)
	{
		this.max = max;
		objs = new List<GameObject>();
		for (int i = 0; i < max; i++)
		{
			GameObject gameObject = Object.Instantiate(prefab) as GameObject;
			gameObject.transform.parent = parentTf;
			gameObject.SetActive(false);
			objs.Add(gameObject);
		}
		avIndex = 0;
	}

	public GameObject Create()
	{
		GameObject result = null;
		for (int i = 0; i < max; i++)
		{
			int num = (i + avIndex) % max;
			if (!objs[num].activeSelf)
			{
				result = objs[num];
				avIndex = num + 1;
				break;
			}
		}
		return result;
	}

	public void CleanUp()
	{
		objs.Clear();
		avIndex = 0;
	}
}
