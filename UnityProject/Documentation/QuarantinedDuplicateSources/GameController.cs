using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Advertisements;

public class GameController : MonoBehaviour
{
	public enum STATE
	{
		ST_TITLE,
		ST_RESULT,
		ST_CREDIT,
		ST_OPTION,
		ST_PLAY,
		ST_SHARE,
		ST_TUTORIAL
	}

	public enum PLAY_STATE
	{
		ST_UP_STOP,
		ST_DOWN_PREPARE,
		ST_DOWN,
		ST_DOWN_STOP,
		ST_UP,
		ST_HOLD
	}

	public GoogleAnalyticsV3 googleAnalytics;

	public GameObject creditBackButtonObject;

	public GameObject optionBackButtonObject;

	public GameObject langObject;

	public GameObject langButtonObject;

	public GameObject noAdsButtonObject;

	public GameObject restoreButtonObject;

	public GameObject versionObject;

	public GameObject tutorialPanel;

	public GameObject sharePanel;

	private tk2dTextMesh tutorialLoadingText;

	public static GameObject bgPanel;

	public static GameController Instance;

	public static int SCREEN_WIDTH;

	public static int SCREEN_HEIGHT;

	public static float VIEW_WIDTH;

	public static float VIEW_HEIGHT;

	public static float CAMERA_SCALE;

	private Hero player;

	private Map map;

	private bool isTutorialScreenDone;

	[HideInInspector]
	public bool touchAllowed;

	private STATE state;

	private PLAY_STATE playState;

	private bool isPaused;

	private int distance;

	private int playCount;

	private int[] gemCounts = new int[16];

	private int slamCount;

	private int coolCount;

	private int maxCoolCount;

	private int skillValue;

	private int skillID;

	public float skillDuration;

	private int playerID;

	private PlayerData data;

	private float elapseTime;

	private float bg1Time;

	private bool isPlayReady;

	private GameObject titlePanel;

	private tk2dTextMesh titleLoadingText;

	private GameObject darkPanel;

	private GameObject creditPanel;

	private tk2dSprite soundSprite;

	private tk2dSprite vibrationSprite;

	private GameObject optionPanel;

	private GameObject resultPanel;

	private GameObject worldPanel;

	private GameObject uiPanel;

	private GameObject wallCamera;

	[HideInInspector]
	public Popup popup;

	private AudioSource bgAudioSource1;

	private AudioSource bgAudioSource2;

	public tk2dSpriteAnimator ballAnim;

	public tk2dSpriteAnimator missileAnim1;

	public tk2dSpriteAnimator missileAnim2;

	private bool isScreenShotTaken;

	private bool isGooglePlayInited;

	private bool touchToStartShow;

	private InterstitialAd interstitial;

	private bool isAdsDirty = true;

	private bool isAdsShowRequested;

	public bool isJustAdsWatch;

	public Texture2D screenShot;

	public RenderTexture rt;

	private bool isScreenShotDirty;

	public STATE GetState()
	{
		return state;
	}

	public int GetDistance()
	{
		return distance;
	}

	public int GetSlamCount()
	{
		return slamCount;
	}

	public int GetCoolCount()
	{
		return coolCount;
	}

	public int GetMaxCoolCount()
	{
		return maxCoolCount;
	}

	public Hero GetPlayer()
	{
		return player;
	}

	public int[] GetGemCounts()
	{
		return gemCounts;
	}

	public int GetGemCount(int playerID)
	{
		return gemCounts[playerID];
	}

	public PLAY_STATE GetPlayState()
	{
		return playState;
	}

	public bool IsShowHint()
	{
		return slamCount <= 2;
	}

	public int GetPlayCount()
	{
		return playCount;
	}

	private void Awake()
	{
		Instance = this;
		if ((bool)bgPanel)
		{
			bgPanel.transform.parent = base.transform;
		}
		SCREEN_WIDTH = 1280;
		SCREEN_HEIGHT = 800;
		CAMERA_SCALE = 100f;
		AudioListener.volume = 0f;
		VIEW_WIDTH = (float)SCREEN_WIDTH / CAMERA_SCALE;
		VIEW_HEIGHT = (float)SCREEN_HEIGHT / CAMERA_SCALE;
		User.Instance.LoadData();
		LoadOption();
		Lang.Instance.Load();
		InitPanels();
		bg1Time = -1f;
		DOTween.Init(recycleAllByDefault: true, useSafeMode: false);
		if (Advertisement.isSupported)
		{
			Advertisement.Initialize("50898");
		}
		Util.d("lang:" + Application.systemLanguage);
	}

	private void LoadOption()
	{
		Option.Instance.Load();
		int index = Option.Instance.Get(OptionData.LAST_PLAYER);
		if (User.Instance.GetPlayerLevel(index) >= 1)
		{
			playerID = index;
		}
		else
		{
			playerID = 0;
		}
	}

	private void Start()
	{
		PlayerManager.Instance.Load();
		MapManager.Instance.Load();
		InitGooglePlay();
		InitAds();
		InitSocial();
		Init();
		elapseTime = 0f;
		state = STATE.ST_TITLE;
		playState = PLAY_STATE.ST_UP_STOP;
		ShowTitle();
		touchAllowed = true;
	}

	private void InitPlay()
	{
		UnityEngine.Random.seed = Time.frameCount + (int)Time.time;
		map.CleanUp();
		map.InitPlay();
		map.MakeRocks(doClean: false);
	}

	private void InitUI()
	{
		Pause(isPaused: false);
	}

	private void InitPanels()
	{
		titlePanel = base.transform.Find("TITLE").gameObject;
		titleLoadingText = titlePanel.transform.Find("loadingSprite").GetComponent<tk2dTextMesh>();
		resultPanel = base.transform.Find("RESULT").gameObject;
		creditPanel = base.transform.Find("CREDIT").gameObject;
		uiPanel = base.transform.Find("UI").gameObject;
		darkPanel = base.transform.Find("DARK").gameObject;
		optionPanel = base.transform.Find("OPTION").gameObject;
		soundSprite = optionPanel.transform.Find("soundButton").gameObject.GetComponent<tk2dSprite>();
		vibrationSprite = optionPanel.transform.Find("vibrateButton").gameObject.GetComponent<tk2dSprite>();
		restoreButtonObject.SetActive(value: false);
		worldPanel = base.transform.Find("WORLD").gameObject;
		player = worldPanel.transform.Find("player").GetComponent<Hero>();
		ballAnim = worldPanel.transform.Find("Ball").GetComponent<tk2dSpriteAnimator>();
		ballAnim.gameObject.SetActive(value: false);
		missileAnim1 = worldPanel.transform.Find("Missile1").GetComponent<tk2dSpriteAnimator>();
		missileAnim1.gameObject.SetActive(value: false);
		missileAnim2 = worldPanel.transform.Find("Missile2").GetComponent<tk2dSpriteAnimator>();
		missileAnim2.gameObject.SetActive(value: false);
		popup = base.transform.Find("POPUP").gameObject.GetComponent<Popup>();
		tutorialLoadingText = tutorialPanel.transform.Find("loadingSprite").GetComponent<tk2dTextMesh>();
		tutorialPanel.SetActive(value: false);
		versionObject.GetComponent<tk2dTextMesh>().text = $"ver.{CurrentBundleVersion.version}";
		bgAudioSource1 = base.gameObject.GetComponents<AudioSource>()[0];
		bgAudioSource2 = base.gameObject.GetComponents<AudioSource>()[1];
		int width = Screen.width;
		int height = Screen.height;
		int num = Screen.height * 2 / 3;
		int height2 = num;
		rt = new RenderTexture(width, height, 24);
		screenShot = new Texture2D(num, height2, TextureFormat.RGB24, mipmap: false);
	}

	private void LoadData()
	{
	}

	private void Init()
	{
		map = Map.Instance;
		map.InitMap();
		playCount = 0;
		bool flag = Option.Instance.Get(OptionData.SOUND) == 1;
		bool flag2 = Option.Instance.Get(OptionData.VIBRATION) == 1;
		AudioListener.volume = (flag ? 1 : 0);
		soundSprite.SetSprite((!flag) ? "result_sound_off" : "result_sound_on");
		vibrationSprite.SetSprite((!flag2) ? "result_vibe_off" : "result_vibe_on");
		if (flag)
		{
			bgAudioSource1.Play();
		}
		player.Show(show: false);
		if (Option.Instance.Get(OptionData.LANG) != 0)
		{
			UpdateLang();
		}
		UpdateWithAdsOption();
	}

	private void StartPlay()
	{
		if (state != STATE.ST_PLAY)
		{
			state = STATE.ST_PLAY;
			distance = 0;
			slamCount = 0;
			coolCount = 0;
			for (int i = 0; i < gemCounts.Length; i++)
			{
				gemCounts[i] = 0;
			}
			tk2dUIManager.Instance.UseMultiTouch = true;
			elapseTime = 0f;
			isScreenShotDirty = true;
			playState = PLAY_STATE.ST_UP_STOP;
			InitPlay();
			player.InitPlayer(playerID);
			data = PlayerManager.Instance.GetDataByID(playerID);
			maxCoolCount = data.GetCooltime(User.Instance.GetPlayerLevel(playerID));
			skillValue = data.GetSkillValue(User.Instance.GetPlayerLevel(playerID));
			skillID = data.GetSkillID();
			skillDuration = -1f;
			IngameUI.Instance.InitPlay(data);
			ShowWorld();
			InitUI();
			player.Show(show: true);
			map.Fade(dark: false, delegate
			{
			});
			map.ShowHintDusts();
			isScreenShotTaken = false;
		}
	}

	private bool UpdateCameraOnEnd()
	{
		Rock outScreenHoleRock = map.GetOutScreenHoleRock();
		Util.d("rock out1");
		if ((bool)outScreenHoleRock)
		{
			Util.d("rock out");
			float endValue = outScreenHoleRock.GetX() - 2f;
			Camera.main.transform.DOMoveX(endValue, 1f);
			return true;
		}
		return false;
	}

	private bool EndPlay(bool fromPlay)
	{
		player.Show(show: false);
		bool result = false;
		if (fromPlay)
		{
			result = UpdateCameraOnEnd();
		}
		map.EndPlay();
		if (fromPlay)
		{
			playCount++;
			User.Instance.UpdatePlayData(1);
			User.Instance.SaveData();
		}
		return result;
	}

	public void QuitPlay()
	{
		map.InitRockOffset();
		for (int i = 0; i < gemCounts.Length; i++)
		{
			gemCounts[i] = 0;
		}
		distance = 0;
		EndPlay(fromPlay: false);
		ShowResult(fromPlay: false);
	}

	public int GetPlayerID()
	{
		return playerID;
	}

	public void ChangeStateForce(PLAY_STATE state)
	{
		ChangeState(state);
	}

	private void ChangeState(PLAY_STATE state)
	{
		playState = state;
		elapseTime = 0f;
		switch (state)
		{
		case PLAY_STATE.ST_DOWN_PREPARE:
			PlaySound(AudioManager.Instance.shake);
			StartCoroutine(map.PrepareRockDown(delegate
			{
				ChangeState(PLAY_STATE.ST_DOWN);
			}));
			break;
		case PLAY_STATE.ST_DOWN:
			break;
		case PLAY_STATE.ST_DOWN_STOP:
			PlaySound(AudioManager.Instance.crash);
			Vibrate();
			if (!player.IsDead())
			{
				Invoke("ShowSafe", 0.1f);
				CheckCapture();
			}
			else if (!isScreenShotTaken)
			{
				CheckCapture();
			}
			break;
		}
	}

	private void ShowSafe()
	{
		player.ShowSafe();
	}

	private void UpdateInStop(float deltaTime)
	{
		map.UpdateInStop(elapseTime, delegate
		{
			ChangeState(PLAY_STATE.ST_DOWN_PREPARE);
		});
	}

	private void UpdateInDownPrepare(float deltaTime)
	{
	}

	private void UpdateInDown(float deltaTime)
	{
		map.UpdateInDown(deltaTime, delegate
		{
			ChangeState(PLAY_STATE.ST_DOWN_STOP);
			StartCoroutine(Util.Shake(Camera.main.gameObject, 0.2f, 0.3f, 1f, 1f));
		});
	}

	private void UpdateInDownStop(float deltaTime)
	{
		if (player.IsDead())
		{
			ChangeState(PLAY_STATE.ST_UP_STOP);
			StartCoroutine(Util.Shake(Camera.main.gameObject, 0.2f, 0.3f, 1f, 1f));
			if (EndPlay(fromPlay: true))
			{
				ShowResult(fromPlay: true);
			}
			else
			{
				ShowResult(fromPlay: true);
			}
			return;
		}
		map.UpdateInDownStop(elapseTime, delegate
		{
			ChangeState(PLAY_STATE.ST_UP);
			if (!player.IsDead())
			{
				slamCount++;
				bool flag = false;
				if (!player.IsSkillActive())
				{
					coolCount++;
					if (coolCount == maxCoolCount)
					{
						flag = true;
					}
				}
				if (flag)
				{
					StartSkillPreMap(skillID);
				}
				if (!flag && skillID != 13)
				{
					map.UpdateTheme(slamCount);
				}
				map.MakeRocks(doClean: true, (!flag) ? (-1) : skillID);
				if (flag)
				{
					StartSkill(skillID);
				}
			}
		});
	}

	private void UpdateInUp(float deltaTime)
	{
		map.UpdateInUp(deltaTime, delegate
		{
			ChangeState(PLAY_STATE.ST_UP_STOP);
		});
	}

	private void UpdateSkill()
	{
		int activeSkill = player.GetActiveSkill();
		if (activeSkill != -1 && activeSkill != 6 && activeSkill != 5)
		{
			skillDuration -= Time.deltaTime;
			if (activeSkill != 7)
			{
				player.UpdateSkill();
			}
			if (skillDuration <= 0f)
			{
				EndSkill(activeSkill);
			}
		}
	}

	public void StartSkillPreMap(int skillID)
	{
		switch (skillID)
		{
		case 7:
			break;
		case 2:
			break;
		case 3:
			break;
		case 5:
			break;
		case 4:
			map.SetPatternID(3);
			break;
		case 9:
			map.SetPatternID(3);
			break;
		case 1:
			break;
		case 13:
			map.LoadTheme(-1, 4);
			map.LoadParticles(4);
			break;
		case 6:
			break;
		case 8:
			break;
		case 11:
			break;
		case 12:
			break;
		case 14:
			break;
		case 10:
			break;
		}
	}

	private void SetScreenShotDirty()
	{
		isScreenShotDirty = true;
		Util.d("SetScreenShotDirty");
	}

	public void StartSkill(int skillID)
	{
		coolCount = 0;
		player.SetActiveSkill(skillID);
		isScreenShotDirty = true;
		switch (skillID)
		{
		case 7:
			skillDuration = 1f;
			Invoke("DoCapture", 0.5f);
			break;
		case 2:
			Invoke("DoCapture", 0.5f);
			skillDuration = skillValue;
			break;
		case 3:
			Invoke("DoCapture", 0.5f);
			skillDuration = skillValue;
			break;
		case 5:
			Invoke("DoCapture", 0.5f);
			skillDuration = skillValue;
			map.SetExtraStopDuration(skillDuration + 2f);
			map.SetFollowGem(isFollow: true);
			break;
		case 4:
		case 9:
			skillDuration = skillValue;
			Invoke("DoCapture", 1f);
			map.SetExtraStopDuration(skillDuration + 2f);
			break;
		case 1:
			skillDuration = skillValue;
			Invoke("DoCapture", 2f);
			map.SetExtraStopDuration(skillDuration + 2f);
			break;
		case 13:
			skillDuration = skillValue;
			Invoke("DoCapture", 2f);
			map.SetExtraStopDuration(skillDuration + 2f);
			break;
		case 6:
			Invoke("DoCapture", 1f);
			break;
		case 8:
			Invoke("DoCapture", 2f);
			skillDuration = skillValue;
			player.UpdateFlyTarget(map.GetCollideRock(isUp: false));
			map.SetExtraStopDuration(skillDuration + 2f);
			map.SetFollowGem(isFollow: true);
			break;
		case 10:
			skillDuration = skillValue;
			break;
		case 11:
			skillDuration = skillValue;
			break;
		case 12:
			skillDuration = skillValue;
			IngameUI.Instance.SetSkillEnable(enable: true);
			break;
		case 14:
			skillDuration = skillValue;
			break;
		case 15:
			skillDuration = skillValue;
			IngameUI.Instance.SetSkillEnable(enable: true);
			break;
		case 16:
			skillDuration = skillValue;
			IngameUI.Instance.SetSkillEnable(enable: true);
			break;
		}
	}

	public void EndSkill(int skillID)
	{
		player.SetActiveSkill(-1);
		switch (skillID)
		{
		case 2:
			break;
		case 3:
			elapseTime = 0f;
			break;
		case 5:
			map.SetExtraStopDuration(0f);
			elapseTime = 0f;
			map.SetFollowGem(isFollow: false);
			break;
		case 4:
		case 9:
			map.SetExtraStopDuration(0f);
			elapseTime = 0f;
			map.SetPatternID(0);
			break;
		case 1:
			map.SetExtraStopDuration(0f);
			elapseTime = 0f;
			break;
		case 13:
			map.SetExtraStopDuration(0f);
			map.UpdateTheme(slamCount);
			map.UpdateRockTheme();
			elapseTime = 0f;
			break;
		case 6:
			map.SetExtraStopDuration(0f);
			elapseTime = 0f;
			Util.d("extra 0");
			break;
		case 8:
			map.SetExtraStopDuration(0f);
			elapseTime = 0f;
			map.SetFollowGem(isFollow: false);
			break;
		case 10:
			elapseTime = 0f;
			break;
		case 11:
			elapseTime = 0f;
			break;
		case 12:
			IngameUI.Instance.SetSkillEnable(enable: false);
			elapseTime = 0f;
			break;
		case 14:
			elapseTime = 0f;
			break;
		case 15:
			IngameUI.Instance.SetSkillEnable(enable: false);
			elapseTime = 0f;
			break;
		case 16:
			IngameUI.Instance.SetSkillEnable(enable: false);
			elapseTime = 0f;
			break;
		case 7:
			break;
		}
	}

	private void Update()
	{
		switch (state)
		{
		case STATE.ST_PLAY:
			UpdatePlay();
			break;
		case STATE.ST_TUTORIAL:
			UpdateTutorial();
			break;
		case STATE.ST_TITLE:
			UpdateTitle();
			break;
		case STATE.ST_RESULT:
			UpdateResult();
			break;
		case STATE.ST_CREDIT:
			UpdateCredit();
			break;
		case STATE.ST_OPTION:
			UpdateOption();
			break;
		case STATE.ST_SHARE:
			UpdateShare();
			break;
		}
	}

	private void UpdateCamera(float x, float y)
	{
		Camera.main.transform.position = new Vector3(x, y, Camera.main.transform.position.z);
	}

	private void InitGooglePlay()
	{
		if (GameConfig.useGA)
		{
			if ((bool)ServiceAPI.Instance && User.Instance.gpsLoginState != 2)
			{
				ServiceAPI.Instance.Init(delegate
				{
					isGooglePlayInited = true;
					User.Instance.gpsLoginState = 1;
				}, delegate
				{
					isGooglePlayInited = true;
					User.Instance.gpsLoginState = 2;
				});
			}
			else
			{
				isGooglePlayInited = true;
			}
		}
		else
		{
			isGooglePlayInited = true;
		}
	}

	private bool IsTitleLoaded()
	{
		if (!isGooglePlayInited && User.Instance.gpsLoginState == 0)
		{
			return false;
		}
		if ((bool)AndroidHub.Instance)
		{
			if (AndroidHub.Instance.IsBillDone())
			{
				return true;
			}
			return false;
		}
		return true;
	}

	private IEnumerator blinkText(tk2dTextMesh textMesh)
	{
		float alpha = 1f;
		float vel = 2f;
		while (textMesh.gameObject.activeInHierarchy)
		{
			alpha += 0.05f * vel;
			if (alpha >= 1f)
			{
				alpha = 1f;
				vel = -2f;
			}
			else if (alpha <= 0f)
			{
				alpha = 0f;
				vel = 2f;
			}
			textMesh.color = new Color(1f, 1f, 1f, alpha);
			yield return new WaitForSeconds(0.05f);
		}
		yield return null;
	}

	private void UpdateTitle()
	{
		bool flag = IsTitleLoaded();
		if (flag && !touchToStartShow)
		{
			titleLoadingText.text = "TOUCH TO START";
			StartCoroutine(blinkText(titleLoadingText));
			touchToStartShow = true;
			if ((bool)ServiceAPI.Instance)
			{
				ServiceAPI.Instance.UpdateWithCurrent();
			}
		}
		if (flag && Input.GetMouseButtonDown(0))
		{
			map.EndPlay();
			ShowTutorial();
			return;
		}
		float x = Camera.main.transform.position.x + Time.deltaTime * 2f;
		float y = Camera.main.transform.position.y;
		UpdateCamera(x, y);
		map.UpdateTitleRocks();
	}

	private void UpdateTutorial()
	{
		if (Input.GetMouseButtonDown(0))
		{
			StartPlay();
		}
	}

	private void UpdateCredit()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ShowResult(fromPlay: false);
		}
	}

	private void UpdateOption()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ShowResult(fromPlay: false);
		}
	}

	private void UpdateResult()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (popup.IsOpen())
			{
				popup.NoClicked();
				return;
			}
			if (ResultMenu.Instance.IsBuyPopupOpen())
			{
				ResultMenu.Instance.CloseBuyPopup();
				return;
			}
			popup.ShowPopup("quit_game", delegate
			{
				Application.Quit();
			}, null);
		}
		if (isPlayReady)
		{
			isPlayReady = false;
			StartPlay();
		}
	}

	private void UpdateShare()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SocialShare.Instance.OnResumeClick();
		}
	}

	private void UpdatePlay()
	{
		if (popup.IsOpen() && Input.GetKeyDown(KeyCode.Escape))
		{
			popup.NoClicked();
			return;
		}
		if (isPaused)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				IngameUI.Instance.OnResumeClick();
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			IngameUI.Instance.OnPauseClick();
			return;
		}
		float deltaTime = Time.deltaTime;
		elapseTime += deltaTime;
		UpdateSkill();
		switch (playState)
		{
		case PLAY_STATE.ST_UP_STOP:
			UpdateInStop(deltaTime);
			break;
		case PLAY_STATE.ST_DOWN_PREPARE:
			UpdateInDownPrepare(deltaTime);
			break;
		case PLAY_STATE.ST_DOWN:
			UpdateInDown(deltaTime);
			break;
		case PLAY_STATE.ST_DOWN_STOP:
			UpdateInDownStop(deltaTime);
			break;
		case PLAY_STATE.ST_UP:
			UpdateInUp(deltaTime);
			break;
		}
		if (!player.IsDead())
		{
			float x = player.gameObject.transform.position.x + 2f;
			UpdateCamera(x, VIEW_HEIGHT / 2f + (float)map.baseY);
		}
		float x2 = player.GetX();
		if ((float)distance < x2)
		{
			distance = (int)x2;
		}
		map.UpdateMap();
		IngameUI.Instance.UpdateUI();
		player.UpdatePlayer();
	}

	private void Clear()
	{
	}

	private void ShowTitle()
	{
		state = STATE.ST_TITLE;
		titlePanel.SetActive(value: true);
		worldPanel.SetActive(value: true);
		uiPanel.SetActive(value: false);
		resultPanel.SetActive(value: false);
		creditPanel.SetActive(value: false);
		optionPanel.SetActive(value: false);
		sharePanel.SetActive(value: false);
	}

	private void ShowOption()
	{
		state = STATE.ST_OPTION;
		titlePanel.SetActive(value: false);
		uiPanel.SetActive(value: false);
		creditPanel.SetActive(value: false);
		optionPanel.SetActive(value: true);
		resultPanel.transform.DOMoveY(VIEW_HEIGHT, 0.5f).OnComplete(_ShowOption);
		optionPanel.transform.position = new Vector3(0f, 0f - VIEW_HEIGHT, 0f);
		optionPanel.transform.DOMoveY(0f, 0.5f);
		touchAllowed = false;
	}

	private void _ShowOption()
	{
		resultPanel.SetActive(value: false);
		touchAllowed = true;
	}

	private void ShowCredit()
	{
		if (state != STATE.ST_CREDIT)
		{
			state = STATE.ST_CREDIT;
			titlePanel.SetActive(value: false);
			uiPanel.SetActive(value: false);
			creditPanel.SetActive(value: true);
			optionPanel.SetActive(value: false);
			resultPanel.transform.DOMoveY(VIEW_HEIGHT, 0.5f).OnComplete(_ShowCredit);
			creditPanel.transform.position = new Vector3(0f, 0f - VIEW_HEIGHT, 0f);
			creditPanel.transform.DOMoveY(0f, 0.5f);
			touchAllowed = false;
		}
	}

	private void _ShowCredit()
	{
		resultPanel.SetActive(value: false);
		touchAllowed = true;
	}

	private void _ShowResultPanel()
	{
		creditPanel.SetActive(value: false);
		optionPanel.SetActive(value: false);
		touchAllowed = true;
	}

	public void ShowResult(bool fromPlay)
	{
		if (state != STATE.ST_RESULT)
		{
			darkPanel.SetActive(value: true);
			if (state == STATE.ST_CREDIT)
			{
				state = STATE.ST_RESULT;
				titlePanel.SetActive(value: false);
				uiPanel.SetActive(value: false);
				resultPanel.SetActive(value: true);
				optionPanel.SetActive(value: false);
				worldPanel.SetActive(value: true);
				ResultMenu.Instance.ShowResult(fromPlay: false);
				resultPanel.transform.DOMoveY(0f, 0.5f).OnComplete(_ShowResultPanel);
				creditPanel.transform.DOMoveY(0f - VIEW_HEIGHT, 0.5f);
			}
			else if (state == STATE.ST_OPTION)
			{
				state = STATE.ST_RESULT;
				titlePanel.SetActive(value: false);
				uiPanel.SetActive(value: false);
				resultPanel.SetActive(value: true);
				creditPanel.SetActive(value: false);
				resultPanel.transform.DOMoveY(0f, 0.5f).OnComplete(_ShowResultPanel);
				optionPanel.transform.DOMoveY(0f - VIEW_HEIGHT, 0.5f);
			}
			else
			{
				state = STATE.ST_RESULT;
				titlePanel.SetActive(value: false);
				uiPanel.SetActive(value: false);
				resultPanel.SetActive(value: true);
				creditPanel.SetActive(value: false);
				optionPanel.SetActive(value: false);
				sharePanel.SetActive(value: false);
				ResultMenu.Instance.ShowResult(fromPlay);
				LoadAds();
			}
			isPlayReady = false;
		}
	}

	private void ShowWorld()
	{
		titlePanel.SetActive(value: false);
		worldPanel.SetActive(value: true);
		uiPanel.SetActive(value: true);
		resultPanel.SetActive(value: false);
		creditPanel.SetActive(value: false);
		optionPanel.SetActive(value: false);
		darkPanel.SetActive(value: false);
		tutorialPanel.SetActive(value: false);
		sharePanel.SetActive(value: false);
	}

	private void ShowShare()
	{
		if (state != STATE.ST_SHARE)
		{
			state = STATE.ST_SHARE;
			uiPanel.SetActive(value: false);
			map.Fade(dark: true, delegate
			{
				SocialShare.Instance.Show();
			});
		}
	}

	private void ShowTutorial()
	{
		if (state != STATE.ST_TUTORIAL)
		{
			state = STATE.ST_TUTORIAL;
			titlePanel.SetActive(value: false);
			uiPanel.SetActive(value: false);
			resultPanel.SetActive(value: false);
			creditPanel.SetActive(value: false);
			optionPanel.SetActive(value: false);
			worldPanel.SetActive(value: false);
			tutorialPanel.SetActive(value: true);
			tutorialLoadingText.text = "TOUCH TO START";
			StartCoroutine(blinkText(tutorialLoadingText));
			isTutorialScreenDone = true;
		}
	}

	public void Pause(bool isPaused)
	{
		this.isPaused = isPaused;
		Time.timeScale = ((!isPaused) ? 1 : 0);
		if (Option.Instance.Get(OptionData.SOUND) == 1)
		{
			if (isPaused)
			{
				bgAudioSource1.Pause();
				bg1Time = bgAudioSource1.time;
			}
			else if (bg1Time > 0f)
			{
				bgAudioSource1.time = bg1Time;
				bgAudioSource1.Play();
				bg1Time = -1f;
			}
		}
	}

	public void OnCreditBackClick()
	{
		if (touchAllowed)
		{
			PlaySound(AudioManager.Instance.select);
			ShowResult(fromPlay: false);
		}
	}

	public void OnOptionBackClick()
	{
		if (touchAllowed)
		{
			PlaySound(AudioManager.Instance.select);
			ShowResult(fromPlay: false);
		}
	}

	public void OnSoundClick()
	{
		if (touchAllowed)
		{
			PlaySound(AudioManager.Instance.select);
			bool flag = Option.Instance.Get(OptionData.SOUND) == 1;
			flag = !flag;
			if (flag && !bgAudioSource1.isPlaying)
			{
				bgAudioSource1.Play();
			}
			Option.Instance.Set(OptionData.SOUND, flag ? 1 : 0);
			soundSprite.SetSprite((!flag) ? "result_sound_off" : "result_sound_on");
			AudioListener.volume = (flag ? 1 : 0);
		}
	}

	public void OnVibrationClick()
	{
		if (touchAllowed)
		{
			PlaySound(AudioManager.Instance.select);
			bool flag = Option.Instance.Get(OptionData.VIBRATION) == 1;
			flag = !flag;
			Option.Instance.Set(OptionData.VIBRATION, flag ? 1 : 0);
			vibrationSprite.SetSprite((!flag) ? "result_vibe_off" : "result_vibe_on");
		}
	}

	public void OnLanguageClick()
	{
		if (touchAllowed)
		{
			PlaySound(AudioManager.Instance.select);
			int num = Option.Instance.Get(OptionData.LANG);
			num = (num + 1) % 2;
			Util.d("Lang" + num);
			Option.Instance.Set(OptionData.LANG, num);
			Lang.Instance.Load();
			UpdateLang();
		}
	}

	private void UpdateLang()
	{
		creditBackButtonObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("result_05"));
		optionBackButtonObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("result_05"));
		langObject.GetComponent<tk2dTextMesh>().text = Lang.Instance.Get("language");
		langButtonObject.GetComponent<tk2dTextMesh>().text = Lang.Instance.Get("language_val");
		restoreButtonObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("restore"));
		ResultMenu.Instance.UpdateLang();
	}

	public void OnPlayClick()
	{
		if (!touchAllowed)
		{
			return;
		}
		PlaySound(AudioManager.Instance.select);
		map.Fade(dark: true, delegate
		{
			if (isTutorialScreenDone)
			{
				if (IsShowAds() && IsAdsTurn())
				{
					if (!ShowAds())
					{
						isPlayReady = true;
					}
				}
				else
				{
					isPlayReady = true;
				}
			}
			else
			{
				ShowTutorial();
			}
		});
	}

	public void OnOptionClick()
	{
		if (touchAllowed)
		{
			PlaySound(AudioManager.Instance.select);
			ShowOption();
		}
	}

	public void OnCreditClick()
	{
		if (touchAllowed)
		{
			PlaySound(AudioManager.Instance.select);
			ShowCredit();
		}
	}

	public void OnGooglePlayButton()
	{
		if ((bool)ServiceAPI.Instance)
		{
			PlaySound(AudioManager.Instance.select);
			ServiceAPI.Instance.OnGooglePlayButton();
		}
	}

	public void OnLeaderboardButton()
	{
		if ((bool)ServiceAPI.Instance)
		{
			PlaySound(AudioManager.Instance.select);
			ServiceAPI.Instance.OnLeaderboardButton();
		}
	}

	public void OnAchievementButton()
	{
		if ((bool)ServiceAPI.Instance)
		{
			PlaySound(AudioManager.Instance.select);
			ServiceAPI.Instance.OnAchievementButton();
		}
	}

	public void SetPlayerID(int id)
	{
		Util.d("SetPlayerID " + id);
		playerID = id;
		Option.Instance.Set(OptionData.LAST_PLAYER, id);
	}

	public void PlaySound(AudioClip clip, float volumeScale = 1f)
	{
		if (Option.Instance.Get(OptionData.SOUND) == 1)
		{
			bgAudioSource2.PlayOneShot(clip, volumeScale);
		}
	}

	public void AddGem(int id, int cnt)
	{
		gemCounts[id] += cnt;
	}

	private void InitAds()
	{
		Util.d("InitAds1");
		if (IsShowAds())
		{
			Util.d("InitAds2");
			if (interstitial != null)
			{
				interstitial.Destroy();
				interstitial = null;
			}
			interstitial = new InterstitialAd("ca-app-pub-5743065732431244/8355173771");
			interstitial.AdClosed += OnAdsClosed;
			interstitial.AdFailedToLoad += OnAdsFailed;
		}
	}

	private void InitSocial()
	{
	}

	private bool IsAdsTurn()
	{
		if (isJustAdsWatch)
		{
			isJustAdsWatch = false;
			return false;
		}
		int totalPlayCount = User.Instance.GetTotalPlayCount();
		if (totalPlayCount < 5)
		{
			return false;
		}
		return totalPlayCount % GameConfig.ADS_SHOW_INTERVAL == 0;
	}

	private void LoadAds()
	{
		isAdsShowRequested = false;
		if (IsShowAds() && IsAdsTurn() && isAdsDirty)
		{
			InitAds();
			Util.d("LoadAds1");
			AdRequest request = new AdRequest.Builder().Build();
			interstitial.LoadAd(request);
			isAdsDirty = false;
			isPlayReady = false;
			Util.d("LoadAds2");
		}
	}

	private bool ShowAds()
	{
		if (interstitial == null)
		{
			return false;
		}
		if (interstitial.IsLoaded())
		{
			Util.d("ShowAds");
			interstitial.Show();
			isAdsDirty = true;
			isAdsShowRequested = true;
		}
		else
		{
			Util.d("SkipAds");
			isPlayReady = true;
		}
		return true;
	}

	public void OnAdsClosed(object sender, EventArgs args)
	{
		isPlayReady = true;
		isAdsShowRequested = false;
		Util.d("OnAdsClosed");
	}

	public void OnAdsFailed(object sender, AdFailedToLoadEventArgs args)
	{
		if (isAdsShowRequested)
		{
			isPlayReady = true;
			isAdsShowRequested = false;
		}
		isAdsDirty = true;
	}

	public void Vibrate()
	{
		if (Option.Instance.Get(OptionData.VIBRATION) == 1)
		{
			Handheld.Vibrate();
		}
	}

	private void OnApplicationQuit()
	{
		Option.Instance.Save();
		User.Instance.SaveData();
		PlayerPrefs.Save();
		if ((bool)AndroidHub.Instance)
		{
			AndroidHub.Instance.Release();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		Util.d("OnApplicationPause " + pauseStatus);
		if (state == STATE.ST_PLAY && !isPaused)
		{
			IngameUI.Instance.OnPauseClick();
		}
	}

	private bool IsShowAds()
	{
		if (Application.isEditor)
		{
			return false;
		}
		if (User.Instance.IsAdsRemoved())
		{
			return false;
		}
		return GameConfig.SHOW_ADS;
	}

	private void TestShowAds()
	{
		if (IsShowAds())
		{
			ShowAds();
		}
	}

	public void ExitOnError(string msg)
	{
		Util.d("ExitOnError: " + msg);
		Application.Quit();
	}

	public void OnTapSpaceButton()
	{
		Util.LogEvent(string.Format("url {0}", "market://details?id=com.grayger.game.tapspace"));
		Application.OpenURL("market://details?id=com.grayger.game.tapspace");
	}

	public void OnTemplarButton()
	{
		Util.LogEvent(string.Format("url {0}", "market://details?id=com.iamagame.knight"));
		Application.OpenURL("market://details?id=com.iamagame.knight");
	}

	public void OnImgButton()
	{
		Util.LogEvent(string.Format("url {0}", "http://iamagame.com"));
		Application.OpenURL("http://iamagame.com");
	}

	public void OnNoAdsClick()
	{
		if ((bool)AndroidHub.Instance)
		{
			AndroidHub.Instance.TryBuy("item_no_ads");
		}
	}

	public void UpdateWithAdsOption()
	{
		noAdsButtonObject.SetActive(!User.Instance.IsAdsRemoved());
	}

	public void OnRestoreClick()
	{
		if ((bool)AndroidHub.Instance)
		{
			AndroidHub.Instance.RestoreCompletedTransactions();
		}
	}

	public void CheckCapture()
	{
		if (!isScreenShotDirty)
		{
		}
	}

	public void DoCapture()
	{
	}

	public void DoCapture1()
	{
	}

	public IEnumerator TakeScreenShot()
	{
		yield return new WaitForEndOfFrame();
		int resWidth = Screen.width;
		int resHeight = Screen.height;
		int clipWidth = Screen.height * 2 / 3;
		int clipHeight = clipWidth;
		Vector3 heroPos = Camera.main.WorldToScreenPoint(Hero.Instance.gameObject.transform.position + new Vector3(0f, 1f, 0f));
		int clipX = (int)(heroPos.x - (float)(clipWidth / 2));
		int clipY = (int)(heroPos.y - (float)(clipHeight / 2));
		if (clipY < 0)
		{
			clipY = 0;
		}
		Camera.main.targetTexture = rt;
		Camera.main.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(clipX, clipY, clipWidth, clipHeight), 0, 0);
		Camera.main.targetTexture = null;
		RenderTexture.active = null;
		Util.d("TakeScreenShot");
		isScreenShotDirty = false;
		isScreenShotTaken = true;
	}

	public void SaveScreenShot()
	{
		if (!(screenShot == null))
		{
			byte[] bytes = screenShot.EncodeToPNG();
			string text = fileName(Convert.ToInt32(screenShot.width), Convert.ToInt32(screenShot.height));
			string text2 = Application.persistentDataPath + "/Snapshots/" + text;
			Util.d("screen shot!! " + text2);
			File.WriteAllBytes(text2, bytes);
		}
	}

	private string fileName(int width, int height)
	{
		return string.Format("screen_{0}x{1}_{2}.png", width, height, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}
}
