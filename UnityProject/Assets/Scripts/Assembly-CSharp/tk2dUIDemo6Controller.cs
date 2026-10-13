using System.Collections.Generic;
using UnityEngine;

public class tk2dUIDemo6Controller : tk2dUIBaseDemoController
{
	private class ItemDef
	{
		public string name = string.Empty;

		public int score = 10;

		public int time = 200;

		public Color color = Color.white;
	}

	public tk2dUILayout prefabItem;

	private float itemStride;

	public tk2dUIScrollableArea scrollableArea;

	public tk2dTextMesh numItemsTextMesh;

	private List<ItemDef> allItems = new List<ItemDef>();

	private List<Transform> cachedContentItems = new List<Transform>();

	private List<Transform> unusedContentItems = new List<Transform>();

	private int firstCachedItem = -1;

	private int maxVisibleItems;

	private int numToAdd = 100;

	private void OnEnable()
	{
		scrollableArea.OnScroll += OnScroll;
	}

	private void OnDisable()
	{
		scrollableArea.OnScroll -= OnScroll;
	}

	private void Start()
	{
		prefabItem.transform.parent = null;
		DoSetActive(prefabItem.transform, false);
		itemStride = (prefabItem.GetMaxBounds() - prefabItem.GetMinBounds()).x;
		maxVisibleItems = Mathf.CeilToInt(scrollableArea.VisibleAreaLength / itemStride) + 1;
		float num = 0f;
		for (int i = 0; i < maxVisibleItems; i++)
		{
			tk2dUILayout tk2dUILayout2 = Object.Instantiate(prefabItem) as tk2dUILayout;
			tk2dUILayout2.transform.parent = scrollableArea.contentContainer.transform;
			tk2dUILayout2.transform.localPosition = new Vector3(num, 0f, 0f);
			DoSetActive(tk2dUILayout2.transform, false);
			unusedContentItems.Add(tk2dUILayout2.transform);
			num += itemStride;
		}
		SetItemCount(100);
	}

	private void CustomizeListObject(Transform contentRoot, int itemId)
	{
		contentRoot.Find("Name").GetComponent<tk2dTextMesh>().text = allItems[itemId].name;
		contentRoot.Find("Score").GetComponent<tk2dTextMesh>().text = "Score: " + allItems[itemId].score;
		contentRoot.Find("Time").GetComponent<tk2dTextMesh>().text = "Time: " + allItems[itemId].time;
		contentRoot.Find("Portrait").GetComponent<tk2dBaseSprite>().color = allItems[itemId].color;
		contentRoot.localPosition = new Vector3((float)itemId * itemStride, 0f, 0f);
	}

	private void SetItemCount(int numItems)
	{
		if (numItems < allItems.Count)
		{
			allItems.RemoveRange(numItems, allItems.Count - numItems);
		}
		else
		{
			for (int i = allItems.Count; i < numItems; i++)
			{
				string[] array = new string[10] { "Ba", "Po", "Re", "Zu", "Meh", "Ra'", "B'k", "Adam", "Ben", "George" };
				string[] array2 = new string[28]
				{
					"Hoopler", "Hysleria", "Yeinydd", "Nekmit", "Novanoid", "Toog1t", "Yboiveth", "Resaix", "Voquev", "Yimello",
					"Oleald", "Digikiki", "Nocobot", "Morath", "Toximble", "Rodrup", "Chillaid", "Brewtine", "Surogou", "Winooze",
					"Hendassa", "Ekcle", "Noelind", "Animepolis", "Tupress", "Jeren", "Yoffa", "Acaer"
				};
				string text = string.Format("[{0}] {1} {2}", i, array[Random.Range(0, array.Length)], array2[Random.Range(0, array2.Length)]);
				Color color = new Color32((byte)Random.Range(192, 255), (byte)Random.Range(192, 255), (byte)Random.Range(192, 255), byte.MaxValue);
				ItemDef itemDef = new ItemDef();
				itemDef.name = text;
				itemDef.color = color;
				itemDef.time = Random.Range(10, 1000);
				itemDef.score = itemDef.time * Random.Range(0, 30) / 60;
				allItems.Add(itemDef);
			}
		}
		UpdateListGraphics();
		numItemsTextMesh.text = "COUNT: " + numItems;
	}

	private void OnScroll(tk2dUIScrollableArea scrollableArea)
	{
		UpdateListGraphics();
	}

	private void UpdateListGraphics()
	{
		float num = scrollableArea.Value * (scrollableArea.ContentLength - scrollableArea.VisibleAreaLength);
		int num2 = Mathf.FloorToInt(num / itemStride);
		float num3 = (float)allItems.Count * itemStride;
		if (!Mathf.Approximately(num3, scrollableArea.ContentLength))
		{
			if (num3 < scrollableArea.VisibleAreaLength)
			{
				scrollableArea.Value = 0f;
				for (int i = 0; i < cachedContentItems.Count; i++)
				{
					DoSetActive(cachedContentItems[i], false);
					unusedContentItems.Add(cachedContentItems[i]);
				}
				cachedContentItems.Clear();
				firstCachedItem = -1;
				num2 = 0;
			}
			scrollableArea.ContentLength = num3;
			if (scrollableArea.ContentLength > 0f)
			{
				scrollableArea.Value = num / (scrollableArea.ContentLength - scrollableArea.VisibleAreaLength);
			}
		}
		int num4 = Mathf.Min(num2 + maxVisibleItems, allItems.Count);
		while (firstCachedItem >= 0 && firstCachedItem < num2)
		{
			firstCachedItem++;
			DoSetActive(cachedContentItems[0], false);
			unusedContentItems.Add(cachedContentItems[0]);
			cachedContentItems.RemoveAt(0);
			if (cachedContentItems.Count == 0)
			{
				firstCachedItem = -1;
			}
		}
		while (firstCachedItem >= 0 && firstCachedItem + cachedContentItems.Count > num4)
		{
			DoSetActive(cachedContentItems[cachedContentItems.Count - 1], false);
			unusedContentItems.Add(cachedContentItems[cachedContentItems.Count - 1]);
			cachedContentItems.RemoveAt(cachedContentItems.Count - 1);
			if (cachedContentItems.Count == 0)
			{
				firstCachedItem = -1;
			}
		}
		if (firstCachedItem < 0)
		{
			firstCachedItem = num2;
			int num5 = Mathf.Min(firstCachedItem + maxVisibleItems, allItems.Count);
			for (int j = firstCachedItem; j < num5; j++)
			{
				Transform transform = unusedContentItems[0];
				cachedContentItems.Add(transform);
				unusedContentItems.RemoveAt(0);
				CustomizeListObject(transform, j);
				DoSetActive(transform, true);
			}
			return;
		}
		while (firstCachedItem > num2)
		{
			firstCachedItem--;
			Transform transform2 = unusedContentItems[0];
			unusedContentItems.RemoveAt(0);
			cachedContentItems.Insert(0, transform2);
			CustomizeListObject(transform2, firstCachedItem);
			DoSetActive(transform2, true);
		}
		while (firstCachedItem + cachedContentItems.Count < num4)
		{
			Transform transform3 = unusedContentItems[0];
			unusedContentItems.RemoveAt(0);
			CustomizeListObject(transform3, firstCachedItem + cachedContentItems.Count);
			cachedContentItems.Add(transform3);
			DoSetActive(transform3, true);
		}
	}

	private void AddMoreItems()
	{
		SetItemCount(allItems.Count + Random.Range(numToAdd / 10, numToAdd));
		numToAdd *= 2;
	}

	private void ResetItems()
	{
		numToAdd = 100;
		SetItemCount(3);
	}
}
