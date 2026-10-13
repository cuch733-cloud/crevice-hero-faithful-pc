using UnityEngine;
using UnityEngine.Advertisements;

public class ResultMenu : MonoBehaviour
{
	private const string LINK_RATE = "rate";

	private const string LINK_AD = "ad1";

	private const string LINK_GAME = "game";

	private const string LINK_GIFT = "gift";

	private const string LINK_TAP = "ts";

	private const string LINK_TEMPLAR = "te";

	private const int GEM_RANDOM_SIZE = 150;

	public GameObject playButtonObject;

	public GameObject rankingButtonObject;

	public GameObject achievementButtonObject;

	public GameObject gamecenterButtonObject;

	public GameObject gpButton;

	public GameObject maxDistObject;

	public GameObject distObject;

	public GameObject newRecordObject;

	public GameObject toUpdateObject;

	public GameObject creditButton;

	public GameObject optionButton;

	private GameObject buyPopupPanel;

	private GameObject charPanel;

	private BuyPopup buyPopup;

	private PlayerPanel playerPanel;

	private tk2dTextMesh resultMeterText;

	private tk2dTextMesh resultMaxMeterText;

	public static ResultMenu Instance;

	public AdsButton adsLinkObject;

	public GiftBox giftLinkObject;

	public tk2dSprite gameLinkSprite;

	public GameObject rateLinkObject;

	private string[] LINK_GAMES = new string[2] { "te", "ts" };

	private string[] LINK_ARR = new string[5] { "gift", "game", "ad1", "rate", null };

	private string linkID;

	private int[] gemDefaultRandom = new int[16]
	{
		5, 5, 5, 5, 5, 5, 5, 5, 10, 15,
		15, 15, 15, 5, 20, 15
	};

	private int[] gemRandomBasket = new int[150];

	private int gemBasketSize;

	private void InitGemRandom()
	{
		int num = 0;
		for (int i = 0; i < 16; i++)
		{
			if (User.Instance.playerLevels[i] != 7)
			{
				for (int j = num; j < num + gemDefaultRandom[i]; j++)
				{
					gemRandomBasket[j] = i;
				}
				num += gemDefaultRandom[i];
			}
		}
		gemBasketSize = num;
	}

	private int PickGem()
	{
		if (gemBasketSize > 0)
		{
			return gemRandomBasket[Random.Range(0, gemBasketSize)];
		}
		return Random.Range(0, 16);
	}

	private void Awake()
	{
		Instance = this;
		Init();
	}

	private void Init()
	{
		resultMeterText = base.transform.Find("meterText").gameObject.GetComponent<tk2dTextMesh>();
		resultMaxMeterText = base.transform.Find("maxMeterText").gameObject.GetComponent<tk2dTextMesh>();
		buyPopupPanel = base.transform.Find("BUYPOPUP").gameObject;
		charPanel = base.transform.Find("CHAR_PANEL").gameObject;
		playerPanel = charPanel.GetComponent<PlayerPanel>();
		buyPopup = buyPopupPanel.GetComponent<BuyPopup>();
		gamecenterButtonObject.SetActive(false);
		InitGemRandom();
	}

	public void ShowResult(bool fromPlay)
	{
		playerPanel.CreatePlayerPanel();
		buyPopupPanel.SetActive(false);
		resultMeterText.text = string.Format("{0}", GameController.Instance.GetDistance());
		resultMaxMeterText.text = string.Format("{0}", User.Instance.GetMaxDistance());
		playerPanel.UpdatePlayerButtons(fromPlay);
		if (fromPlay && GameController.Instance.GetDistance() == User.Instance.GetMaxDistance())
		{
			newRecordObject.SetActive(true);
			newRecordObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("result_04"));
		}
		else
		{
			newRecordObject.SetActive(false);
		}
		ShowLink(fromPlay);
	}

	public void ShowBuyPopup(int id)
	{
		buyPopup.Show(id);
	}

	public bool IsBuyPopupOpen()
	{
		return buyPopup.gameObject.activeSelf;
	}

	public void CloseBuyPopup()
	{
		buyPopup.OnCloseClick();
	}

	public void UpdateLang()
	{
		maxDistObject.GetComponent<tk2dTextMesh>().text = Lang.Instance.Get("max_distance");
		distObject.GetComponent<tk2dTextMesh>().text = Lang.Instance.Get("distance");
		toUpdateObject.GetComponent<tk2dTextMesh>().text = Lang.Instance.Get("to_be_updated");
		playButtonObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("result_00"));
	}

	public void UpdateWithIAB()
	{
		buyPopup.UpdateWithIAB();
	}

	public void UpdateGooglePlayButton(bool show)
	{
		if (gpButton != null)
		{
			gpButton.SetActive(show);
		}
	}

	public void OnAdsClick()
	{
		GameController.Instance.PlaySound(AudioManager.Instance.select);
		if (!Advertisement.IsReady("rewardedVideoZone"))
		{
			return;
		}
		Advertisement.Show("rewardedVideoZone", new ShowOptions
		{
			resultCallback = (ShowResult result) =>
			{
				GameController.Instance.PlaySound(AudioManager.Instance.gift);
				int num = PickGem();
				int num2 = PickGem();
				if (num2 == num)
				{
					num2 = (num2 + 1) % 16;
				}
				User.Instance.AddGem(num, 20);
				User.Instance.AddGem(num2, 20);
				playerPanel.UpdatePlayerButton(num, 20);
				playerPanel.UpdatePlayerButton(num2, 20);
				User.Instance.SaveData();
				adsLinkObject.OnAdsEnd(num, 20, num2, 20);
				GameController.Instance.isJustAdsWatch = true;
				Util.LogEvent("adslink");
			}
		});
	}

	public void OnRateClick()
	{
		GameController.Instance.PlaySound(AudioManager.Instance.select);
		Application.OpenURL("market://details?id=com.iamagame.crevicehero");
		User.Instance.SetRated(true);
		Util.LogEvent("rate");
	}

	public void OnGiftClick()
	{
		GameController.Instance.PlaySound(AudioManager.Instance.gift);
		int id = PickGem();
		User.Instance.AddGem(id, 5);
		playerPanel.UpdatePlayerButton(id, 5);
		User.Instance.SaveData();
		giftLinkObject.OnClick(id, 5);
		Util.LogEvent("giftlink");
	}

	public void OnResultGameLink()
	{
		GameController.Instance.PlaySound(AudioManager.Instance.select);
		if (linkID == "ts")
		{
			Application.OpenURL("market://details?id=com.grayger.game.tapspace");
			Util.LogEvent("gamelink_tapspace");
		}
		else if (linkID == "te")
		{
			Application.OpenURL("market://details?id=com.iamagame.knight");
			Util.LogEvent("gamelink_templar");
		}
	}

	private void ShowLink(bool isShow)
	{
		Util.d("ShowLink " + isShow);
		adsLinkObject.gameObject.SetActive(false);
		gameLinkSprite.gameObject.SetActive(false);
		rateLinkObject.SetActive(false);
		giftLinkObject.gameObject.SetActive(false);
		if (!isShow)
		{
			return;
		}
		do
		{
			linkID = LINK_ARR[User.Instance.linkIdx % LINK_ARR.Length];
			User.Instance.linkIdx = (User.Instance.linkIdx + 1) % LINK_ARR.Length;
			if (linkID == "ad1")
			{
				adsLinkObject.gameObject.SetActive(true);
				adsLinkObject.Show();
				Util.LogEvent("adslink_0");
				break;
			}
			if (linkID == "gift")
			{
				giftLinkObject.gameObject.SetActive(true);
				giftLinkObject.Show();
				Util.LogEvent("giftlink_0");
				break;
			}
			if (linkID == "rate")
			{
				if (!User.Instance.IsRated())
				{
					rateLinkObject.SetActive(true);
					Util.LogEvent("rate_0");
					break;
				}
				linkID = "gift";
				giftLinkObject.gameObject.SetActive(true);
				giftLinkObject.Show();
				Util.LogEvent("giftlink_0");
				break;
			}
			if (linkID == "game")
			{
				linkID = LINK_GAMES[User.Instance.linkGameIdx % LINK_GAMES.Length];
				User.Instance.linkGameIdx = (User.Instance.linkGameIdx + 1) % LINK_GAMES.Length;
				if (linkID == "ts")
				{
					gameLinkSprite.gameObject.SetActive(true);
					gameLinkSprite.SetSprite("ad_tapspace");
					Util.LogEvent("gamelink_tapspace_0");
				}
				else
				{
					gameLinkSprite.gameObject.SetActive(true);
					gameLinkSprite.SetSprite("ad_templar");
					Util.LogEvent("gamelink_templar_0");
				}
				break;
			}
		}
		while (linkID != null);
	}
}
