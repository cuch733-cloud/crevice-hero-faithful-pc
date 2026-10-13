using UnityEngine;

public class AndroidHub : MonoBehaviour
{
	private AndroidJavaObject activity;

	private AndroidJavaClass unityPlayerClass;

	private bool isHub;

	private bool isBillDone;

	private bool isBuyRequested;

	public static AndroidHub Instance;

	public bool IsBillDone()
	{
		return isBillDone;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Util.d("AndroidHub.Start");
		isHub = false;
		isBillDone = true;
		isBuyRequested = false;
		if (!Application.isEditor)
		{
			isBillDone = false;
			Util.d("c0");
			unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			Util.d("c1");
			activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
			isHub = true;
			if (GameConfig.USE_FLURRY)
			{
				InitFlurry();
			}
			InitBill();
		}
	}

	public void Release()
	{
		if (isHub)
		{
			Util.d("AndroidHub.Release");
			if (activity != null)
			{
				activity.Call("ReleaseAll");
				Util.d("AndroidHub.Release Done");
			}
		}
	}

	private void InitFlurry()
	{
		if (isHub && activity != null)
		{
			activity.Call("InitFlurry", "4Y2MPM88ZMRCYQ46QZ8G");
			Util.d("InitFlurry Done");
		}
	}

	public void LogEvent(string log)
	{
		if (isHub && GameConfig.USE_FLURRY && activity != null)
		{
			activity.Call("LogEvent", log);
			Util.d("Flurry log: " + log);
		}
	}

	public void LogEvent_Result(string distance, string playerID, string mapID)
	{
		if (isHub && GameConfig.USE_FLURRY && activity != null)
		{
			activity.Call("LogEvent_CH", distance, playerID, mapID);
			Util.d("Flurry log: " + distance + "," + playerID + "," + mapID);
		}
	}

	private void InitBill()
	{
		if (isHub)
		{
			Util.d("InitBill");
			if (activity != null)
			{
				activity.Call("InitBill", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3Kc+WyMMHyFBVCetm3s39fprGYl0ZnakbwhpqCeCZZM7V9Hl1fJhEbGe7xW+1iIncKLpEG6j2sCkmRm9la4pCjrGwY4d0L2vLo0c2u92Wdb7rQvrZyTYc+0VkZfHHa4bER0/XlQnPOLX/SQLDupvBcn1LowBxwJdOiiKFOkCztoW/TV9xzisbLv12QajQ94lqh8ns63szTuTkIsyf/kyT9UPoDuNdhj7hdRleK+ZA7/zTFbGHVe9PNfNxw6f8PjVb2B3QUBMPvPopAIVlfW6BXBIrBasqTBNjXSR/u/kMwlKM2w56AhLT1a1GS7d49P1awVi2KtLlxsVEPqXlFI+7QIDAQAB");
				Util.d("InitBill Done");
			}
		}
	}

	private void BillUtilSuccess(string message)
	{
		Util.d("BillUtilSuccess: " + message);
		isBillDone = true;
	}

	private void BillUtilError(string message)
	{
		Util.d("BillUtilError: " + message);
		isBillDone = true;
	}

	private void CheckPurchaseDone(string skuList)
	{
		Util.d("CheckPurchaseDone sku list:" + skuList);
		char[] separator = new char[1] { ':' };
		string[] array = skuList.Split(separator);
		string[] array2 = array;
		foreach (string sku in array2)
		{
			User.Instance.UpdateWithSku(sku);
		}
		isBillDone = true;
	}

	private void BuyMultipleDone(string skuList)
	{
		Util.d("BuyMultipleDone sku list:" + skuList);
		char[] separator = new char[1] { ':' };
		string[] array = skuList.Split(separator);
		string[] array2 = array;
		foreach (string sku in array2)
		{
			User.Instance.UpdateWithSku(sku);
		}
		isBillDone = true;
	}

	public void TryBuy(string sku)
	{
		if (isHub && !isBuyRequested)
		{
			Util.d("TryBuy " + sku);
			isBuyRequested = true;
			if (activity != null)
			{
				activity.Call("TryBuy", sku);
			}
		}
	}

	private void BuyDone(string sku)
	{
		Util.d("BuyDone sku:" + sku);
		Util.LogEvent(string.Format("buy {0}", sku));
		User.Instance.UpdateWithSku(sku);
		if (sku.Equals("item_no_ads"))
		{
			GameController.Instance.UpdateWithAdsOption();
		}
		else
		{
			ResultMenu.Instance.UpdateWithIAB();
		}
		isBuyRequested = false;
	}

	private void BillUtilPurchaseError(string message)
	{
		Util.d("BillUtilPurchaseError: " + message);
		isBuyRequested = false;
	}

	private void ResultBuyItem(string result)
	{
		Util.d("ResultBuyItem " + result);
		isBuyRequested = false;
	}

	public void RestoreCompletedTransactions()
	{
	}

	public void ResultRestoreItem(string sku)
	{
		Util.d("RestoreDone sku:" + sku);
		Util.LogEvent(string.Format("restore {0}", sku));
		User.Instance.UpdateWithSku(sku);
		if (sku.Equals("item_no_ads"))
		{
			GameController.Instance.UpdateWithAdsOption();
		}
		else
		{
			ResultMenu.Instance.UpdateWithIAB();
		}
		isBuyRequested = false;
	}
}
