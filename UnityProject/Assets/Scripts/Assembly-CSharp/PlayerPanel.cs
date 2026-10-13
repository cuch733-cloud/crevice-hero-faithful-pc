using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
	public GameObject playerButtonPrefab;

	public GameObject leftArrow;

	public GameObject leftArrowFx;

	public GameObject rightArrow;

	public GameObject rightArrowFx;

	public GameObject selectBox;

	private List<PlayerButton> playerButtons;

	[HideInInspector]
	public PlayerButton prevPlayerButton;

	[HideInInspector]
	public PlayerButton selectedPlayerButton;

	private int page;

	private bool isLoaded;

	public static PlayerPanel Instance;

	private int PAGE_CHAR_COUNT = 8;

	private float nextUpdate;

	private void Awake()
	{
		playerButtons = new List<PlayerButton>();
		isLoaded = false;
		Instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (leftArrowFx.gameObject.activeSelf && Time.time > nextUpdate)
		{
			leftArrowFx.gameObject.GetComponent<Renderer>().enabled = !leftArrowFx.gameObject.GetComponent<Renderer>().enabled;
			nextUpdate = Time.time + 0.5f;
		}
		if (rightArrowFx.gameObject.activeSelf && Time.time > nextUpdate)
		{
			rightArrowFx.gameObject.GetComponent<Renderer>().enabled = !rightArrowFx.gameObject.GetComponent<Renderer>().enabled;
			nextUpdate = Time.time + 0.5f;
		}
	}

	private void OnLeftClick()
	{
		if (GameController.Instance.touchAllowed)
		{
			MoveTo(0);
		}
	}

	private void OnRightClick()
	{
		if (GameController.Instance.touchAllowed)
		{
			MoveTo(1);
		}
	}

	private void MoveTo(int page)
	{
		if (this.page != page)
		{
			this.page = page;
			switch (page)
			{
			case 1:
				base.gameObject.transform.DOMove(new Vector3((float)(-GameController.SCREEN_WIDTH) / GameController.CAMERA_SCALE, 0f, 0f), 0.5f).OnComplete(_Moved);
				leftArrow.SetActive(false);
				rightArrow.SetActive(false);
				leftArrowFx.SetActive(false);
				rightArrowFx.SetActive(false);
				GameController.Instance.touchAllowed = false;
				break;
			case 0:
				base.gameObject.transform.DOMove(new Vector3(0f, 0f, 0f), 0.5f).OnComplete(_Moved);
				leftArrow.SetActive(false);
				rightArrow.SetActive(false);
				leftArrowFx.SetActive(false);
				rightArrowFx.SetActive(false);
				GameController.Instance.touchAllowed = false;
				break;
			}
		}
	}

	private void _Moved()
	{
		leftArrow.SetActive(true);
		rightArrow.SetActive(true);
		GameController.Instance.touchAllowed = true;
	}

	private void Init()
	{
	}

	public void CreatePlayerPanel()
	{
		if (isLoaded)
		{
			return;
		}
		float num = 1.4f;
		float num2 = num * (float)(-PAGE_CHAR_COUNT + 1) / 2f;
		for (int i = 0; i < PAGE_CHAR_COUNT; i++)
		{
			GameObject gameObject = Object.Instantiate(playerButtonPrefab) as GameObject;
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.position = new Vector3(num2 + num * (float)i, 1f, 0f);
			PlayerButton component = gameObject.GetComponent<PlayerButton>();
			component.SetData(i);
			playerButtons.Add(component);
		}
		for (int j = 0; j < PAGE_CHAR_COUNT; j++)
		{
			GameObject gameObject2 = Object.Instantiate(playerButtonPrefab) as GameObject;
			gameObject2.transform.parent = base.gameObject.transform;
			gameObject2.transform.position = new Vector3(num2 + num * (float)j + (float)GameController.SCREEN_WIDTH / GameController.CAMERA_SCALE, 1f, 0f);
			PlayerButton component2 = gameObject2.GetComponent<PlayerButton>();
			if (j + PAGE_CHAR_COUNT < 16)
			{
				component2.SetData(j + PAGE_CHAR_COUNT);
			}
			else
			{
				component2.SetDummy();
			}
			playerButtons.Add(component2);
		}
		int playerID = GameController.Instance.GetPlayerID();
		UpdateSelection(playerID, false);
		if (playerID < PAGE_CHAR_COUNT)
		{
			page = 0;
			leftArrow.SetActive(false);
			leftArrowFx.SetActive(false);
		}
		else
		{
			page = 1;
			base.gameObject.transform.localPosition = new Vector3((float)(-GameController.SCREEN_WIDTH) / GameController.CAMERA_SCALE, 0f, 0f);
			rightArrow.SetActive(false);
			rightArrowFx.SetActive(false);
		}
		isLoaded = true;
	}

	public void UpdateSelection(int id, bool showPopup)
	{
		foreach (PlayerButton playerButton in playerButtons)
		{
			if (playerButton.GetID() == id)
			{
				selectBox.transform.position = playerButton.gameObject.transform.position + new Vector3(0f, 0.3f, 0f);
				playerButton.SetSelected(true);
				if (showPopup)
				{
					ResultMenu.Instance.ShowBuyPopup(playerButton.GetID());
				}
				if (User.Instance.GetPlayerLevel(id) >= 1)
				{
					prevPlayerButton = null;
					GameController.Instance.SetPlayerID(id);
				}
				else
				{
					prevPlayerButton = selectedPlayerButton;
				}
				selectedPlayerButton = playerButton;
			}
			else
			{
				playerButton.SetSelected(false);
			}
		}
	}

	public void UpdatePlayerButtons(bool fromPlay)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (PlayerButton playerButton in playerButtons)
		{
			playerButton.UpdateData();
			if (!fromPlay || playerButton.GetID() < 0)
			{
				continue;
			}
			int gemCount = GameController.Instance.GetGemCount(playerButton.GetID());
			if (gemCount > 0)
			{
				playerButton.ShowGainedGem(gemCount);
				if (playerButton.GetID() < PAGE_CHAR_COUNT)
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
			}
		}
		leftArrowFx.gameObject.SetActive(flag);
		rightArrowFx.gameObject.SetActive(flag2);
	}

	public void UpdatePlayerButton(int id, int addedGems)
	{
		PlayerButton playerButton = playerButtons.Find((PlayerButton b) => b.GetID() == id);
		if (playerButton != null)
		{
			playerButton.ShowAddedGem(addedGems);
		}
	}
}
