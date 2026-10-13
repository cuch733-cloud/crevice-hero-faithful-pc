using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BuyPopup : MonoBehaviour
{
	public GameObject arrowFrame;

	public GameObject nameObject;

	public GameObject lvObject;

	public GameObject lvupObject;

	public GameObject lvupDetailObject;

	public GameObject skillSpriteObject;

	public GameObject buyObject;

	public GameObject gem1Object;

	public GameObject gem2Object;

	public GameObject gem1TextObject;

	public GameObject gem2TextObject;

	public GameObject skillTextObject;

	public GameObject cooltimeObject;

	public GameObject playerObject;

	public GameObject messageObject;

	public GameObject levelArrowObject;

	public GameObject cooltimeArrowObject;

	public GameObject speedTextObject;

	public GameObject speedUpTextObject;

	public GameObject speedUpArrowObject;

	public GameObject newSpriteObject;

	public GameObject ballSpriteObject;

	private PlayerData data;

	public void Show(int id)
	{
		base.gameObject.SetActive(true);
		data = PlayerManager.Instance.GetDataByID(id);
		tk2dTextMesh component = nameObject.GetComponent<tk2dTextMesh>();
		component.text = data.GetName();
		int playerLevel = User.Instance.GetPlayerLevel(id);
		int gemCount = User.Instance.GetGemCount(id);
		int levelupGems = data.GetLevelupGems(playerLevel);
		bool flag = playerLevel == 0 && data.GetCash() >= 1;
		tk2dTextMesh component2 = lvObject.GetComponent<tk2dTextMesh>();
		component2.text = string.Format("{0} {1}/{2}", Lang.Instance.Get("level"), playerLevel, 7);
		tk2dTextMesh component3 = skillTextObject.GetComponent<tk2dTextMesh>();
		component3.text = data.GetSkillDetail(playerLevel);
		tk2dTextMesh component4 = cooltimeObject.GetComponent<tk2dTextMesh>();
		component4.text = string.Format("{0}: {1}", Lang.Instance.Get("cooltime"), data.GetCooltime(playerLevel));
		tk2dTextMesh component5 = lvupObject.GetComponent<tk2dTextMesh>();
		tk2dTextMesh component6 = lvupDetailObject.GetComponent<tk2dTextMesh>();
		tk2dTextMesh component7 = speedTextObject.GetComponent<tk2dTextMesh>();
		component7.text = string.Format("{0}: {1}", Lang.Instance.Get("speed"), PlayerManager.Instance.GetPlayerSpeed(id, playerLevel));
		tk2dTextMesh component8 = speedUpTextObject.GetComponent<tk2dTextMesh>();
		switch (playerLevel)
		{
		case 0:
			component5.gameObject.SetActive(false);
			component2.text = string.Format("{0} {1}", Lang.Instance.Get("level"), 1);
			component4.text = string.Format("{0}: {1}", Lang.Instance.Get("cooltime"), data.GetCooltime(1));
			component6.gameObject.SetActive(false);
			levelArrowObject.SetActive(false);
			cooltimeArrowObject.SetActive(false);
			speedUpTextObject.SetActive(false);
			speedUpArrowObject.SetActive(false);
			break;
		case 7:
			component5.gameObject.SetActive(false);
			component6.gameObject.SetActive(false);
			levelArrowObject.SetActive(false);
			cooltimeArrowObject.SetActive(false);
			speedUpTextObject.SetActive(false);
			speedUpArrowObject.SetActive(false);
			break;
		default:
			component5.gameObject.SetActive(true);
			component5.text = string.Format("{0}", playerLevel + 1);
			component6.gameObject.SetActive(true);
			component6.text = string.Format("{0}", data.GetCooltime(playerLevel + 1));
			levelArrowObject.SetActive(true);
			cooltimeArrowObject.SetActive(true);
			component8.text = string.Format("{0}", PlayerManager.Instance.GetPlayerSpeed(id, playerLevel + 1));
			speedUpTextObject.SetActive(true);
			speedUpArrowObject.SetActive(true);
			break;
		}
		tk2dSprite component9 = skillSpriteObject.GetComponent<tk2dSprite>();
		string skillSprite = PlayerData.GetSkillSprite(data.GetSkillID());
		if (skillSprite != null)
		{
			component9.SetSprite(skillSprite);
		}
		tk2dSprite component10 = gem1Object.GetComponent<tk2dSprite>();
		tk2dSprite component11 = gem2Object.GetComponent<tk2dSprite>();
		component10.SetSprite(string.Format("gem_{0}_00", data.gemId));
		component11.SetSprite(string.Format("gem_{0}_00", data.gemId));
		tk2dTextMesh component12 = gem1TextObject.GetComponent<tk2dTextMesh>();
		tk2dTextMesh component13 = gem2TextObject.GetComponent<tk2dTextMesh>();
		tk2dSprite component14 = buyObject.GetComponent<tk2dSprite>();
		component12.text = string.Format("{0}", gemCount);
		switch (playerLevel)
		{
		case 0:
			if (flag)
			{
				component11.gameObject.SetActive(false);
				component13.text = Lang.Instance.Get(string.Format("cash{0}", data.GetCash()));
			}
			else
			{
				component11.gameObject.SetActive(true);
				component13.text = string.Format("{0}", levelupGems);
			}
			component14.SetSprite(Lang.Instance.GetResource("popup_buy"));
			component13.gameObject.SetActive(true);
			component14.gameObject.SetActive(true);
			newSpriteObject.SetActive(true);
			break;
		case 7:
			component11.gameObject.SetActive(false);
			component13.gameObject.SetActive(false);
			component14.gameObject.SetActive(false);
			newSpriteObject.SetActive(false);
			break;
		default:
			component13.text = string.Format("{0}", levelupGems);
			component14.SetSprite(Lang.Instance.GetResource("popup_upgrade"));
			component11.gameObject.SetActive(true);
			component13.gameObject.SetActive(true);
			component14.gameObject.SetActive(true);
			newSpriteObject.SetActive(true);
			break;
		}
		if (gemCount < levelupGems && !flag)
		{
			component14.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		}
		else
		{
			component14.color = new Color(1f, 1f, 1f, 1f);
		}
		float num = 1.4f;
		arrowFrame.transform.position = new Vector3(num * (-7.5f + (float)id), arrowFrame.transform.position.y, arrowFrame.transform.position.z);
		tk2dSpriteAnimator component15 = playerObject.GetComponent<tk2dSpriteAnimator>();
		string text = string.Format("char{0}Animation", data.animId);
		Debug.Log(text);
		component15.Library = Resources.Load(text, typeof(tk2dSpriteAnimation)) as tk2dSpriteAnimation;
		component15.Play("stayClip");
		if (id == 14)
		{
			ballSpriteObject.SetActive(true);
		}
		else
		{
			ballSpriteObject.SetActive(false);
		}
		messageObject.gameObject.SetActive(false);
		base.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		base.gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
	}

	private void Hide()
	{
		base.gameObject.SetActive(false);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnBuyClick()
	{
		int iD = data.GetID();
		int playerLevel = User.Instance.GetPlayerLevel(iD);
		int levelupGems = data.GetLevelupGems(playerLevel);
		if (playerLevel == 0 && data.GetCash() >= 1)
		{
			if ((bool)AndroidHub.Instance)
			{
				AndroidHub.Instance.TryBuy(string.Format("item_char_{0}", iD));
			}
		}
		else if (levelupGems <= User.Instance.GetGemCount(iD))
		{
			int num = playerLevel + 1;
			if (num <= 7 && num >= 0)
			{
				User.Instance.AddGem(iD, -levelupGems);
				User.Instance.SetPlayerLevel(iD, num);
				User.Instance.SaveData();
				if ((bool)ServiceAPI.Instance)
				{
					ServiceAPI.Instance.UpdateWithLevelup(iD, num);
				}
				PlayerPanel.Instance.UpdatePlayerButtons(false);
				PlayerPanel.Instance.prevPlayerButton = null;
				Util.LogEvent(string.Format("upgrade: id{0} {1}", iD, num));
			}
			Show(iD);
		}
		else
		{
			ShowMessage(Lang.Instance.Get("not_enough_gem"));
		}
	}

	public void UpdateWithIAB()
	{
		if (data != null)
		{
			User.Instance.SaveData();
			PlayerPanel.Instance.UpdatePlayerButtons(false);
			PlayerPanel.Instance.prevPlayerButton = null;
			Show(data.GetID());
		}
	}

	private void ShowMessage(string message)
	{
		if (!messageObject.gameObject.activeSelf)
		{
			messageObject.gameObject.SetActive(true);
			StartCoroutine(Fade(message));
		}
	}

	private IEnumerator Fade(string message)
	{
		float alpha = 1f;
		tk2dTextMesh mesh = messageObject.GetComponent<tk2dTextMesh>();
		mesh.text = message;
		while (alpha >= 0f)
		{
			mesh.color = new Color(1f, 1f, 1f, alpha);
			alpha -= 0.1f;
			yield return new WaitForSeconds(0.2f);
		}
		messageObject.gameObject.SetActive(false);
	}

	public void OnCloseClick()
	{
		Hide();
		if (PlayerPanel.Instance.prevPlayerButton != null)
		{
			PlayerPanel.Instance.UpdateSelection(PlayerPanel.Instance.prevPlayerButton.GetID(), false);
		}
		else if (PlayerPanel.Instance.selectedPlayerButton != null)
		{
			PlayerPanel.Instance.UpdateSelection(PlayerPanel.Instance.selectedPlayerButton.GetID(), false);
		}
	}
}
