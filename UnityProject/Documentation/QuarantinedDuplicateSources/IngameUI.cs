using UnityEngine;

public class IngameUI : MonoBehaviour
{
	public static IngameUI Instance;

	public GameObject pausePanel;

	public GameObject rightArrow;

	public GameObject leftButton;

	public GameObject rightButton;

	public GameObject jumpButton;

	public GameObject skillButton;

	public GameObject skillObject;

	public GameObject meterObject;

	public GameObject mObject;

	public GameObject pauseButtonObject;

	public GameObject menuButtonObject;

	public GameObject resumeButtonObject;

	public GameObject skillCoolPrefabObject;

	public GameObject skillBackObject;

	private GameObject pauseBG;

	private tk2dTextMesh meterText;

	private tk2dSprite skillSprite;

	private tk2dSpriteAnimator skillBackAnim;

	private tk2dSpriteAnimationClip skillActiveClip;

	private tk2dSpriteAnimationClip skillInactiveClip;

	private Camera uiCamera;

	private Vector3 scaledVec = new Vector3(1.1f, 1.1f, 1f);

	private Vector3 normalVec = new Vector3(1f, 1f, 1f);

	[HideInInspector]
	public bool isLeft;

	[HideInInspector]
	public bool isRight;

	[HideInInspector]
	public bool isJump;

	[HideInInspector]
	public bool isSkill;

	private int maxSkillCount = 10;

	private tk2dSprite[] coolGauges;

	private bool isSkillEnable;

	private void Awake()
	{
		Instance = this;
		pauseBG = pausePanel.transform.Find("pauseBG").gameObject;
		meterText = meterObject.gameObject.GetComponent<tk2dTextMesh>();
		skillSprite = skillObject.gameObject.GetComponent<tk2dSprite>();
		skillBackAnim = skillBackObject.gameObject.GetComponent<tk2dSpriteAnimator>();
		skillActiveClip = skillBackAnim.GetClipByName("skillActiveClip");
		skillInactiveClip = skillBackAnim.GetClipByName("skillInactiveClip");
		rightArrow.animation.wrapMode = WrapMode.Loop;
		pausePanel.SetActive(value: false);
		coolGauges = new tk2dSprite[maxSkillCount];
		for (int i = 0; i < maxSkillCount; i++)
		{
			GameObject gameObject = Object.Instantiate(skillCoolPrefabObject) as GameObject;
			gameObject.transform.parent = skillObject.transform;
			gameObject.transform.localPosition = new Vector3(0.4f + (float)i * 0.13f, 0f, 0f);
			coolGauges[i] = gameObject.GetComponent<tk2dSprite>();
		}
	}

	private void Start()
	{
		uiCamera = GameController.Instance.transform.Find("UICamera").gameObject.GetComponent<Camera>();
	}

	public void SetSkillEnable(bool enable)
	{
		isSkillEnable = enable;
		skillButton.SetActive(isSkillEnable);
	}

	public void InitPlay(PlayerData data)
	{
		int skillID = data.GetSkillID();
		string text = PlayerData.GetSkillSprite(skillID);
		if (text != null)
		{
			skillSprite.SetSprite(text);
		}
		skillBackAnim.Play(skillInactiveClip);
		for (int i = 0; i < maxSkillCount; i++)
		{
			if (i < GameController.Instance.GetMaxCoolCount())
			{
				coolGauges[i].gameObject.SetActive(value: true);
			}
			else
			{
				coolGauges[i].gameObject.SetActive(value: false);
			}
		}
		isSkillEnable = false;
		skillButton.SetActive(isSkillEnable);
	}

	public void UpdateUI()
	{
		meterText.text = $"{GameController.Instance.GetDistance()}";
		GameController.PLAY_STATE playState = GameController.Instance.GetPlayState();
		if (playState == GameController.PLAY_STATE.ST_UP_STOP || playState == GameController.PLAY_STATE.ST_DOWN_PREPARE)
		{
			rightArrow.SetActive(Map.Instance.IsHoleOut());
		}
		int maxCoolCount = GameController.Instance.GetMaxCoolCount();
		for (int i = 0; i < maxCoolCount; i++)
		{
			if (i < GameController.Instance.GetCoolCount())
			{
				coolGauges[i].SetSprite("ui_skill_cooltime_0");
			}
			else
			{
				coolGauges[i].SetSprite("ui_skill_cooltime_1");
			}
		}
		if (Hero.Instance.GetActiveSkill() == -1)
		{
			if (skillBackAnim.CurrentClip != skillInactiveClip)
			{
				skillBackAnim.Play(skillInactiveClip);
			}
		}
		else if (skillBackAnim.CurrentClip != skillActiveClip)
		{
			skillBackAnim.Play(skillActiveClip);
		}
	}

	public void CheckMoveControl()
	{
		int i = 0;
		float num = 1f;
		isRight = false;
		isLeft = false;
		isJump = false;
		isSkill = false;
		for (; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Canceled)
			{
				continue;
			}
			Vector3 vector = uiCamera.ScreenToWorldPoint(touch.position);
			float num2 = leftButton.transform.position.x - vector.x;
			float num3 = leftButton.transform.position.y - vector.y;
			if (num2 * num2 + num3 * num3 <= num * num)
			{
				isLeft = true;
				isRight = false;
			}
			num2 = rightButton.transform.position.x - vector.x;
			num3 = rightButton.transform.position.y - vector.y;
			if (num2 * num2 + num3 * num3 <= num * num)
			{
				isLeft = false;
				isRight = true;
			}
			if (isSkillEnable)
			{
				num2 = skillButton.transform.position.x - vector.x;
				num3 = skillButton.transform.position.y - vector.y;
				if (num2 * num2 + num3 * num3 <= num * num * 1f)
				{
					isSkill = true;
				}
			}
			num2 = jumpButton.transform.position.x - vector.x;
			num3 = jumpButton.transform.position.y - vector.y;
			if (num2 * num2 + num3 * num3 <= num * num * (float)(isSkillEnable ? 1 : 9))
			{
				isJump = true;
			}
		}
		if (isRight)
		{
			rightButton.transform.localScale = scaledVec;
		}
		else
		{
			rightButton.transform.localScale = normalVec;
		}
		if (isLeft)
		{
			leftButton.transform.localScale = scaledVec;
		}
		else
		{
			leftButton.transform.localScale = normalVec;
		}
		if (isJump)
		{
			jumpButton.transform.localScale = scaledVec;
		}
		else
		{
			jumpButton.transform.localScale = normalVec;
		}
		if (isSkill)
		{
			skillButton.transform.localScale = scaledVec;
		}
		else
		{
			skillButton.transform.localScale = normalVec;
		}
	}

	public void OnPauseClick()
	{
		Pause(isPaused: true);
	}

	public void OnResumeClick()
	{
		Pause(isPaused: false);
	}

	public void OnMenuClick()
	{
		GameController.Instance.popup.ShowPopup("quit_play", delegate
		{
			Pause(isPaused: false);
			GameController.Instance.popup.ClosePopup();
			GameController.Instance.QuitPlay();
		}, null);
	}

	private void Pause(bool isPaused)
	{
		GameController.Instance.Pause(isPaused);
		pausePanel.SetActive(isPaused);
		jumpButton.SetActive(!isPaused);
		leftButton.SetActive(!isPaused);
		rightButton.SetActive(!isPaused);
		mObject.SetActive(!isPaused);
		meterObject.SetActive(!isPaused);
		skillObject.SetActive(!isPaused);
		if (isPaused)
		{
			rightArrow.SetActive(value: false);
			pauseBG.renderer.material.mainTexture = Rock.GetTexture();
			menuButtonObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("ui_menu"));
			resumeButtonObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("ui_resume"));
		}
	}
}
