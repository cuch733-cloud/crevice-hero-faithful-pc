using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	public delegate void downDoneCallback();

	public delegate void upDoneCallback();

	public delegate void stopDoneCallback();

	public delegate void prepareDoneCallback();

	public delegate void fadeCallback();

	private const int MAPDATA_CHANGE_INTERVAL = 3;

	private const int MAPDATA_CHANGE_DISTANCE = 30;

	private const int THEME_CHANGE_INTERVAL = 8;

	public const float SKILL_END_EXTRA_DURATION = 2f;

	private const int GEM_BOX_COUNT = 7;

	public const int WINDOW_WIDTH = 18;

	public const int WINDOW_HEIGHT = 12;

	private const float MAX_VEL_Y = 10f;

	private const float MIN_STOP_DURATION = 0.1f;

	private const int GEM_RANDOM_SIZE = 100;

	public GameObject rockPrefab;

	public GameObject gemPrefab;

	public GameObject dustPrefab;

	public GameObject rockFragPrefab;

	public GameObject birdPrefab;

	public GameObject particlePrefab;

	public GameObject whitePanel;

	private int[] gemDefaultRandom = new int[16]
	{
		15, 14, 12, 10, 9, 8, 7, 6, 5, 4,
		3, 2, 2, 1, 1, 1
	};

	private static int[] pat0 = new int[10] { 1, 2, 3, 2, 1, 0, 0, 0, 0, 0 };

	private static int[] pat1 = new int[10] { 0, 2, 3, 3, 2, 1, 0, 0, 0, 0 };

	private static int[] pat2 = new int[10] { 0, 0, 0, 3, 3, 3, 2, 1, 1, 1 };

	private static int[] pat3 = new int[10] { 0, 0, 0, 0, 1, 1, 2, 2, 2, 2 };

	private static int[] pat4 = new int[10] { 0, 1, 0, 0, 0, 0, 0, 1, 1, 0 };

	private static int[][] _mapPatterns = new int[5][] { pat0, pat1, pat2, pat3, pat4 };

	private static int[][] mapPatterns;

	private int[] gemBoxDists = new int[7] { 150, 250, 350, 450, 550, 650, 750 };

	private bool[] gemBoxGets = new bool[7];

	private int themeID;

	private int patternID;

	private int mapDataID;

	private int INDICATOR_INTERVAL = 100;

	public static float GRID_SCALE = 0.7f;

	private int JUMP_HEIGHT = 2;

	private int PLAYER_HEIGHT = 2;

	private int minHeight = 1;

	private float offsetY;

	private MapData mapData;

	private bool isGemFollow;

	private bool isUpRockSlide;

	private int mapTurn;

	private float downVelYAddVal;

	private float upShakeDurationAddVal;

	private float upStopDurationAddVal;

	private float extraStopDuration;

	[HideInInspector]
	public int baseY;

	private bool isShowHintDusts;

	private List<Rock> upRocks;

	private List<Rock> downRocks;

	private List<Rock> holeRocks;

	private List<Gem> gems;

	private List<Dust> dusts;

	private List<RockFrag> rockFrags;

	private List<Bird> birds;

	private List<Particle> particles;

	private List<MapEvent> mapEvents;

	private ObjectPool rockPool;

	private ObjectPool gemPool;

	private ObjectPool dustPool;

	private ObjectPool rockFragPool;

	private ObjectPool birdPool;

	private ObjectPool particlePool;

	private GemBox gemBox;

	private GameObject worldPanel;

	[HideInInspector]
	public GameObject BGPanel;

	private GameObject rockPanel;

	private GameObject gemPanel;

	private GameObject dustPanel;

	private DistanceIndicator distanceIndicator;

	private DistanceIndicator maxIndicator;

	private MapEffect mapEffect;

	public int downRockLayer;

	public int upRockLayer;

	public int defaultLayer;

	public static Map Instance;

	private int[] gemRandomBasket = new int[100];

	private int gemBasketSize;

	private Rock lastRightRock;

	private Rock lastLeftRock;

	private int titleRockCount;

	private int maxWidth
	{
		get
		{
			return 5;
		}
	}

	private int minWidth
	{
		get
		{
			return 2;
		}
	}

	private int maxHeight
	{
		get
		{
			if (mapData == null)
			{
				return 5;
			}
			return mapData.maxHeight;
		}
	}

	public int maxGridOffsetY
	{
		get
		{
			return 5;
		}
	}

	private float holeHalfPercent
	{
		get
		{
			return (float)mapData.holeHalfPercent * 0.01f;
		}
	}

	private float holeBottomPercent
	{
		get
		{
			return (float)mapData.holeBottomPercent * 0.01f;
		}
	}

	private float holeOutPercent
	{
		get
		{
			return (float)mapData.holeOutPercent * 0.01f;
		}
	}

	private float downVelY
	{
		get
		{
			return mapData.downVelY + downVelYAddVal;
		}
	}

	private float upStopDuration
	{
		get
		{
			return mapData.upStopDuration + upStopDurationAddVal;
		}
	}

	private float downStopDuration
	{
		get
		{
			return mapData.downStopDuration;
		}
	}

	private float upShakeDuration
	{
		get
		{
			return mapData.upShakeDuration + upShakeDurationAddVal;
		}
	}

	private bool IsUpRockSlide()
	{
		return isUpRockSlide;
	}

	public void SetUpRockSlide(bool val)
	{
		isUpRockSlide = val;
	}

	private float GetOffsetY()
	{
		return offsetY;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void InitEventMap()
	{
		mapEvents.Clear();
		mapEvents.Add(new MapEvent(EVENT_TYPE.EVENT_BIRD, false, 1, 10));
		mapEvents.Add(new MapEvent(EVENT_TYPE.EVENT_BIRD, true, 100, 5));
	}

	private void ActivateEvent(MapEvent evt)
	{
		if (evt.eventType != EVENT_TYPE.EVENT_BIRD)
		{
			return;
		}
		Hero player = GameController.Instance.GetPlayer();
		for (int i = 0; i < evt.val; i++)
		{
			Bird bird = CreateBird(player.transform.position.x + 12f, 5f + 0.2f * (float)i, true);
			if (bird != null)
			{
			}
		}
	}

	private void CheckEvent()
	{
		int distance = GameController.Instance.GetDistance();
		MapEvent mapEvent = null;
		foreach (MapEvent mapEvent2 in mapEvents)
		{
			if (mapEvent2.distance < distance)
			{
				mapEvent = mapEvent2;
				break;
			}
		}
		if (mapEvent != null)
		{
			ActivateEvent(mapEvent);
			if (mapEvent.repeat)
			{
				mapEvent.distance += mapEvent.distance + UnityEngine.Random.Range(0, 10);
			}
			else
			{
				mapEvents.Remove(mapEvent);
			}
		}
	}

	public void InitMap()
	{
		Util.d("Map.InitMap");
		upRocks = new List<Rock>();
		downRocks = new List<Rock>();
		holeRocks = new List<Rock>();
		gems = new List<Gem>();
		dusts = new List<Dust>();
		rockFrags = new List<RockFrag>();
		birds = new List<Bird>();
		particles = new List<Particle>();
		mapEvents = new List<MapEvent>();
		offsetY = (float)maxGridOffsetY * GRID_SCALE;
		BGPanel = base.transform.Find("BG").gameObject;
		rockPanel = base.transform.Find("ROCKS").gameObject;
		gemPanel = base.transform.Find("GEMS").gameObject;
		dustPanel = base.transform.Find("DUSTS").gameObject;
		distanceIndicator = base.transform.Find("DistanceIndicator").gameObject.GetComponent<DistanceIndicator>();
		distanceIndicator.Show(false);
		maxIndicator = base.transform.Find("MaxIndicator").gameObject.GetComponent<DistanceIndicator>();
		maxIndicator.Show(false);
		mapEffect = base.transform.Find("MapEffect").gameObject.GetComponent<MapEffect>();
		whitePanel.SetActive(false);
		upRockLayer = LayerMask.NameToLayer("UpRock");
		downRockLayer = LayerMask.NameToLayer("DownRock");
		defaultLayer = LayerMask.NameToLayer("Default");
		rockPool = base.gameObject.AddComponent<ObjectPool>();
		rockPool.Init(rockPrefab, 100, rockPanel.transform);
		gemPool = base.gameObject.AddComponent<ObjectPool>();
		gemPool.Init(gemPrefab, 16, gemPanel.transform);
		dustPool = base.gameObject.AddComponent<ObjectPool>();
		dustPool.Init(dustPrefab, 20, dustPanel.transform);
		rockFragPool = base.gameObject.AddComponent<ObjectPool>();
		rockFragPool.Init(rockFragPrefab, 5, dustPanel.transform);
		birdPool = base.gameObject.AddComponent<ObjectPool>();
		birdPool.Init(birdPrefab, 10, dustPanel.transform);
		particlePool = base.gameObject.AddComponent<ObjectPool>();
		particlePool.Init(particlePrefab, 20, dustPanel.transform);
		gemBox = base.transform.Find("gemBoxSprite").gameObject.GetComponent<GemBox>();
		GenPatterns();
		LoadTheme(-1, 0);
	}

	public void SetPatternID(int patternID)
	{
		if (patternID >= 0 && patternID < mapPatterns.Length)
		{
			this.patternID = patternID;
		}
	}

	private void GenPatterns()
	{
		mapPatterns = new int[_mapPatterns.Length][];
		for (int i = 0; i < _mapPatterns.Length; i++)
		{
			int[] array = _mapPatterns[i];
			int num = 0;
			for (int j = 0; j < array.Length; j++)
			{
				num += array[j];
			}
			mapPatterns[i] = new int[num];
			int num2 = 0;
			for (int k = 0; k < array.Length; k++)
			{
				int num3 = array[k];
				for (int l = 0; l < num3; l++)
				{
					mapPatterns[i][num2] = k + 2;
					num2++;
				}
			}
		}
	}

	public void InitPlay()
	{
		Util.d("Map.InitPlay");
		whitePanel.SetActive(false);
		InitEventMap();
		InitGemRandom();
		mapDataID = 0;
		mapData = MapManager.Instance.GetData(mapDataID);
		SetSlow(false);
		Util.d("InitPlay mapdata: " + mapData.phase);
		offsetY = (float)maxGridOffsetY * GRID_SCALE;
		baseY = 0;
		mapTurn = 0;
		isUpRockSlide = true;
		SetPatternID(0);
		extraStopDuration = 0f;
		isGemFollow = false;
		lastLeftRock = null;
		lastRightRock = null;
		for (int i = 0; i < 7; i++)
		{
			gemBoxGets[i] = false;
		}
		if ((bool)gemBox)
		{
			gemBox.SetActive(false);
		}
		int prevThemeID = themeID;
		themeID = 0;
		LoadTheme(prevThemeID, themeID);
		LoadParticles(themeID);
	}

	public void EndPlay()
	{
		foreach (Bird bird in birds)
		{
			bird.SetActive(false, false);
		}
		birds.Clear();
		RemoveParticles();
		mapEffect.gameObject.SetActive(false);
		if (Hero.Instance != null)
		{
			Hero.Instance.EndPlay();
		}
	}

	private void RemoveParticles()
	{
		foreach (Particle particle in particles)
		{
			particle.SetActive(false, false);
		}
		particles.Clear();
		mapEffect.gameObject.SetActive(false);
	}

	public void LoadParticles(int tID)
	{
		if (tID == -1)
		{
			tID = themeID;
		}
		RemoveParticles();
		switch (tID)
		{
		case 0:
			CreateParticles(0, 4, 20f);
			break;
		case 1:
			CreateParticles(2, 5, 15f);
			CreateParticles(3, 5, 30f);
			break;
		case 2:
			CreateParticles(1, 4, 20f);
			CreateParticles(5, 6, 20f);
			break;
		case 3:
			CreateParticles(4, 6, 20f);
			mapEffect.InitPlay();
			break;
		case 4:
			CreateParticles(6, 20, 10f);
			break;
		}
	}

	private void CreateParticles(int type, int count, float startX)
	{
		switch (type)
		{
		case 0:
		{
			float num6 = startX;
			for (int n = 0; n < count; n++)
			{
				CreateParticle(num6, UnityEngine.Random.Range(0.5f, 3f), type);
				num6 += 4f;
			}
			break;
		}
		case 1:
		{
			float num2 = startX;
			for (int j = 0; j < count; j++)
			{
				CreateParticle(num2, UnityEngine.Random.Range(0.2f, 2f), type);
				num2 += 4f;
			}
			break;
		}
		case 2:
		{
			float num4 = startX;
			for (int l = 0; l < count; l++)
			{
				CreateParticle(num4, GameController.VIEW_HEIGHT * 0.5f + UnityEngine.Random.Range(2.5f, 4f), type);
				num4 += 0.5f;
			}
			break;
		}
		case 3:
		{
			float num7 = startX;
			for (int num8 = 0; num8 < count; num8++)
			{
				CreateParticle(num7, GameController.VIEW_HEIGHT * 0.5f + UnityEngine.Random.Range(1.5f, 2.5f), type);
				num7 += 0.5f;
			}
			break;
		}
		case 4:
		{
			float num5 = startX;
			for (int m = 0; m < count; m++)
			{
				CreateParticle(num5, GameController.VIEW_HEIGHT * 0.5f + UnityEngine.Random.Range(1.5f, 2.5f), type);
				num5 += 0.5f;
			}
			break;
		}
		case 5:
		{
			float num3 = startX;
			for (int k = 0; k < count; k++)
			{
				CreateParticle(num3, GameController.VIEW_HEIGHT * 0.5f + UnityEngine.Random.Range(1.5f, 2.5f), type);
				num3 += 0.5f;
			}
			break;
		}
		case 6:
		{
			float num = startX;
			for (int i = 0; i < count; i++)
			{
				CreateParticle(num, GameController.VIEW_HEIGHT * 0.5f + UnityEngine.Random.Range(0.5f, 4f), type);
				num += 0.5f;
			}
			break;
		}
		}
	}

	private Particle CreateParticle(float x, float y, int type)
	{
		GameObject gameObject = particlePool.Create();
		if (gameObject == null)
		{
			return null;
		}
		Particle part = gameObject.GetComponent<Particle>();
		part.InitWith(x, y, type);
		part.OnInactivated = () =>
		{
			particles.Remove(part);
		};
		particles.Add(part);
		return part;
	}

	private void CreateBirds(Rock rock, int count)
	{
		float num = (float)rock.GetGridWidth() * GRID_SCALE / (float)(count + 1);
		float num2 = (float)rock.GetLeftUpGridX() * GRID_SCALE;
		float num3 = rock.gameObject.transform.position.y - (float)rock.GetGridHeight() * 0.5f * GRID_SCALE;
		for (int i = 0; i < count; i++)
		{
			CreateBird(num2 + num * (float)(i + 1), num3 - 0.1f, false);
		}
	}

	private Bird CreateBird(float x, float y, bool isFlying)
	{
		GameObject gameObject = birdPool.Create();
		if (gameObject == null)
		{
			return null;
		}
		Bird bird = gameObject.GetComponent<Bird>();
		bird.InitWith(x, y, isFlying);
		bird.OnInactivated = () =>
		{
			birds.Remove(bird);
		};
		birds.Add(bird);
		return bird;
	}

	private Dust CreateDust(int type, float x, float y, float velY, float velRot, float duration)
	{
		GameObject gameObject = dustPool.Create();
		if (gameObject == null)
		{
			return null;
		}
		Dust dust = gameObject.GetComponent<Dust>();
		dust.InitWith(type, x, y, velY, velRot, duration);
		dust.OnInactivated = () =>
		{
			dusts.Remove(dust);
		};
		dusts.Add(dust);
		return dust;
	}

	public RockFrag CreateRockFrag(float x, float y)
	{
		GameObject gameObject = rockFragPool.Create();
		if (gameObject == null)
		{
			return null;
		}
		RockFrag frag = gameObject.GetComponent<RockFrag>();
		frag.InitWith(x, y);
		frag.OnInactivated = () =>
		{
			rockFrags.Remove(frag);
		};
		rockFrags.Add(frag);
		return frag;
	}

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
			if (User.Instance.GetPlayerLevel(4) == 0 && UnityEngine.Random.value > 0.5f)
			{
				return 4;
			}
			return gemRandomBasket[UnityEngine.Random.Range(0, gemBasketSize)];
		}
		return UnityEngine.Random.Range(0, 16);
	}

	private Gem CreateGem(int playerID, float x, float y)
	{
		if (playerID == -1)
		{
			playerID = PickGem();
		}
		GameObject gameObject = gemPool.Create();
		if (gameObject == null)
		{
			return null;
		}
		Gem gem = gameObject.GetComponent<Gem>();
		gem.InitWith(playerID, x, y);
		gem.OnInactivated = () =>
		{
			gems.Remove(gem);
		};
		gem.SetFollowGem(isGemFollow, false);
		gems.Add(gem);
		return gem;
	}

	private Rock CreateRock(int gridX, int gridY, int gridWidth, int gridHeight, int baseGridY, bool showIndicator, bool isUp, bool isHole, bool isHalf, bool showHoleHint)
	{
		GameObject gameObject = rockPool.Create();
		if (gameObject == null)
		{
			return null;
		}
		Rock rock = gameObject.GetComponent<Rock>();
		rock.InitWith(gridX, gridY, gridWidth, gridHeight, baseGridY, (!isUp) ? 0f : offsetY, isUp, isHole, isHalf, showHoleHint);
		rock.maxGridOffsetY = maxGridOffsetY;
		if (isUp)
		{
			upRocks.Add(rock);
			rock.gameObject.layer = upRockLayer;
			rock.OnInactivated = () =>
			{
				upRocks.Remove(rock);
			};
		}
		else
		{
			downRocks.Add(rock);
			rock.gameObject.layer = downRockLayer;
			if (showIndicator)
			{
				int num = (int)((float)gridX * GRID_SCALE / (float)INDICATOR_INTERVAL);
				int num2 = (int)((float)(gridX + gridWidth) * GRID_SCALE / (float)INDICATOR_INTERVAL);
				if (num < num2)
				{
					distanceIndicator.Show(true);
					distanceIndicator.SetPosition(num2 * INDICATOR_INTERVAL, ((float)(num2 * INDICATOR_INTERVAL) + rock.GetX()) / 2f, (float)rock.GetLeftUpGridY() * GRID_SCALE);
				}
				int maxDistance = User.Instance.GetMaxDistance();
				if (maxDistance >= 10 && (float)gridX * GRID_SCALE <= (float)maxDistance && (float)maxDistance < (float)(gridX + gridWidth) * GRID_SCALE)
				{
					maxIndicator.Show(true);
					maxIndicator.SetPosition(maxDistance, ((float)maxDistance + rock.GetX()) / 2f, (float)rock.GetLeftUpGridY() * GRID_SCALE);
				}
			}
			if ((bool)gemBox)
			{
				for (int num3 = 0; num3 < 7; num3++)
				{
					if (!gemBoxGets[num3])
					{
						int num4 = gemBoxDists[num3];
						if ((float)gridX * GRID_SCALE <= (float)num4 && (float)num4 < (float)(gridX + gridWidth) * GRID_SCALE)
						{
							gemBox.InitWith(rock.GetX(), (float)rock.GetLeftUpGridY() * GRID_SCALE, num3);
							break;
						}
					}
				}
			}
			if (isHole)
			{
				holeRocks.Add(rock);
			}
		}
		return rock;
	}

	private void TruncateRocks()
	{
		int count = upRocks.Count;
		bool flag = true;
		Rock rock = null;
		for (int num = count - 1; num >= 0; num--)
		{
			Rock rock2 = upRocks[num];
			if (rock2.IsLeftOut(18))
			{
				rock2.SetActive(false);
				upRocks.Remove(rock2);
			}
			else if (flag && rock2.IsLeftOut(9))
			{
				flag = false;
				rock2.SetActive(false);
				upRocks.Remove(rock2);
			}
		}
		count = downRocks.Count;
		flag = true;
		for (int num2 = count - 1; num2 >= 0; num2--)
		{
			Rock rock3 = downRocks[num2];
			if (rock3.IsLeftOut(18))
			{
				rock3.SetActive(false);
				downRocks.Remove(rock3);
			}
			else if (flag && rock3.IsLeftOut(9))
			{
				float num3 = rock3.GetLeftUpGridY();
				if ((bool)rock3.rightRock)
				{
					rock3.SetGridY(rock3.rightRock.GetLeftUpGridY() + 4);
				}
				else
				{
					rock3.SetGridY(maxHeight + 1);
				}
				flag = false;
			}
		}
	}

	private Rock GetLastHoleRock()
	{
		int num = 0;
		Rock result = null;
		foreach (Rock holeRock in holeRocks)
		{
			if (holeRock.IsActive() && holeRock.GetLeftUpGridX() > num)
			{
				num = holeRock.GetLeftUpGridX();
				result = holeRock;
			}
		}
		return result;
	}

	private void AppendRocks()
	{
		int num = 0;
		if ((bool)lastRightRock)
		{
			Rock lastHoleRock = GetLastHoleRock();
			int posX = lastRightRock.GetLeftUpGridX() + lastRightRock.GetGridWidth();
			int gridWidth = lastRightRock.GetGridWidth();
			int gridHeight = lastRightRock.GetGridHeight2();
			MakeSubRocks(true, true, posX, num, gridWidth, gridHeight, Hero.Instance.GetActiveSkill());
		}
	}

	private int GetRockWidth()
	{
		int[] array = mapPatterns[patternID];
		int num = UnityEngine.Random.Range(0, array.Length);
		return array[num];
	}

	private void MakeSubRocks(bool isRight, bool isAppend, int posX, int baseY, int prevWidth, int prevHeight, int skillID)
	{
		int num = -1;
		bool flag = false;
		Rock rock = null;
		Rock rock2 = null;
		bool flag2 = !isRight;
		int num2 = 0;
		Rock lastHoleRock = GetLastHoleRock();
		int num3 = 0;
		if ((bool)lastHoleRock)
		{
			num3 = lastRightRock.GetLeftUpGridX() + lastRightRock.GetGridWidth() - (lastHoleRock.GetLeftUpGridX() + lastHoleRock.GetGridWidth());
		}
		int num4 = 0;
		if (isRight)
		{
			num4 = ((GameController.Instance.GetSlamCount() <= 2) ? 8 : ((!(UnityEngine.Random.value < holeOutPercent)) ? ((int)UnityEngine.Random.Range(5.4f, 16.199999f) - num3) : ((int)UnityEngine.Random.Range(14.400001f, 21.6f) - num3)));
			if (num4 < 0)
			{
				num4 = 0;
			}
		}
		GemData gemData = mapData.gemData;
		int num5 = gemData.turns[mapTurn % 5];
		int num6 = 0;
		int num7 = 0;
		int num8 = -1;
		while (num7 < 36)
		{
			bool flag3 = UnityEngine.Random.value < holeHalfPercent;
			int num9 = 2;
			num9 = ((num6 != num) ? GetRockWidth() : 2);
			int num10 = ((num6 % 3 != 0) ? (JUMP_HEIGHT - 1) : JUMP_HEIGHT);
			int num11 = UnityEngine.Random.Range(Mathf.Max(minHeight, prevHeight - JUMP_HEIGHT), Mathf.Min(maxHeight, prevHeight + num10 + 1));
			if (num11 == prevHeight)
			{
				num11 = ((!((float)num11 >= (float)(minHeight + maxHeight) * 0.5f)) ? (num11 + 1) : (num11 - 1));
			}
			if (flag2 && (float)num7 > 27f)
			{
				num11 = prevHeight + 3;
				flag2 = false;
			}
			flag = false;
			if (isRight)
			{
				if (num7 >= num4)
				{
					flag = true;
					num4 = ((!(UnityEngine.Random.value < holeOutPercent)) ? (num4 + (num9 + (int)UnityEngine.Random.Range(5.4f, 18f))) : (num4 + (num9 + (int)UnityEngine.Random.Range(18f, 27f))));
				}
				else
				{
					flag = false;
				}
			}
			int num12;
			if (flag)
			{
				num12 = 12 - num11 - PLAYER_HEIGHT + (flag3 ? 1 : 0);
				if (num11 == maxHeight)
				{
					flag3 = true;
					Util.d("set half");
				}
			}
			else
			{
				num12 = 12 - num11;
			}
			if (!isRight)
			{
				posX -= num9;
			}
			bool showHoleHint = false;
			rock = CreateRock(posX, 12 + maxGridOffsetY, num9, num12 + maxGridOffsetY, baseY, false, true, flag, flag3, showHoleHint);
			num12 = num11;
			if (flag || num6 != num)
			{
				rock2 = CreateRock(posX, num11, num9, num12 + maxGridOffsetY, baseY, isRight, false, flag, false, showHoleHint);
				if ((bool)rock2)
				{
					rock2.counterRock = rock;
					if (num8 >= 0 && num6 == 1 && isRight)
					{
						gemBox.InitWith(rock2.GetX(), (float)rock2.GetLeftUpGridY() * GRID_SCALE, num8);
					}
				}
				if (isRight)
				{
					if (lastRightRock != null)
					{
						lastRightRock.rightRock = rock2;
					}
					lastRightRock = rock2;
				}
				else
				{
					lastLeftRock = rock2;
				}
			}
			if (skillID == 7)
			{
				if (isRight && num2 <= 5)
				{
					float num13 = 1f;
					while (num13 <= (float)(num9 - 1) && num2 <= 5)
					{
						Gem gem = CreateGem(-1, ((float)posX + num13) * GRID_SCALE, (float)(num11 + 1) * GRID_SCALE);
						num13++;
						num2++;
						StartCoroutine(ShowGemEffect(gem, 0.1f * (float)(num2 + 1)));
					}
				}
			}
			else
			{
				bool flag4 = false;
				if (num5 == 3 && isRight && num2 <= 6)
				{
					if (num6 % 3 == 0)
					{
						flag4 = true;
					}
				}
				else if (num5 == 5)
				{
					if (flag)
					{
						flag4 = true;
					}
				}
				else if (num5 != 6)
				{
					if (num5 == 8 && num2 <= 6)
					{
						flag4 = true;
					}
					else
					{
						switch (num5)
						{
						case 1:
							if (num6 == 2)
							{
								flag4 = true;
							}
							break;
						case 2:
							if (isRight && (num6 == 2 || num6 == 4))
							{
								flag4 = true;
							}
							break;
						}
					}
				}
				if (flag4)
				{
					CreateGem(-1, ((float)posX + (float)num9 * 0.5f) * GRID_SCALE, (float)(num11 + 1) * GRID_SCALE);
					num2++;
				}
			}
			if (isRight)
			{
				posX += num9;
			}
			num7 += num9;
			prevWidth = num9;
			prevHeight = num11;
			num6++;
		}
	}

	public void GemBoxBurst(float x, float y, int gemBoxID)
	{
		int num = gemBoxID + 5;
		float num2 = 20f;
		float num3 = (180f - num2 * 2f) / (float)(num - 1);
		Util.d("GemBoxBurst" + gemBoxID + "," + num3);
		gemBoxGets[gemBoxID] = true;
		float num4 = 0.6f;
		for (int i = 0; i < num; i++)
		{
			Gem gem = CreateGem(-1, x + num4 * Mathf.Cos(num2 * ((float)Math.PI / 180f)), y + num4 * Mathf.Sin(num2 * ((float)Math.PI / 180f)));
			if ((bool)gem)
			{
				gem.SetHitable(false);
				StartCoroutine(SetFollowGemLater(i, x, y, num2, num4, gem));
			}
			num2 += num3;
		}
	}

	private IEnumerator SetFollowGemLater(int index, float x, float y, float deg, float fromLen, Gem gem)
	{
		float toLen = 2.5f;
		float len = fromLen;
		float diff = (toLen - fromLen) * 0.19999999f;
		while (len < toLen)
		{
			gem.gameObject.transform.position = new Vector3(x + len * Mathf.Cos(deg * ((float)Math.PI / 180f)), y + len * Mathf.Sin(deg * ((float)Math.PI / 180f)), gem.gameObject.transform.position.z);
			len += diff;
			yield return new WaitForSeconds(0.04f);
		}
		if (index > 0)
		{
			yield return new WaitForSeconds((float)index * 0.02f);
		}
		gem.ShowEffect();
		yield return new WaitForSeconds(0.1f);
		gem.SetHitable(true);
		gem.SetFollowGem(true, true);
	}

	private IEnumerator ShowGemEffect(Gem gem, float delay)
	{
		gem.SetActive(false, false);
		yield return new WaitForSeconds(delay);
		gem.ShowEffect();
	}

	private void MakeGemIndex(List<int> holeIndex, List<int> gemIndex, int gemDropType, bool isRight)
	{
		switch (gemDropType)
		{
		case 0:
			break;
		case 1:
			if (isRight)
			{
				gemIndex.Add(UnityEngine.Random.Range(2, 5));
			}
			break;
		case 2:
			gemIndex.Add(UnityEngine.Random.Range(2, 5));
			break;
		case 3:
		{
			int num = UnityEngine.Random.Range(2, 4);
			for (int j = 2; j < 12; j += num)
			{
				gemIndex.Add(j);
			}
			break;
		}
		case 4:
			if (isRight)
			{
				gemIndex.Add(UnityEngine.Random.Range(4, 7));
			}
			break;
		case 5:
		{
			foreach (int item in holeIndex)
			{
				gemIndex.Add(item);
			}
			break;
		}
		case 6:
		{
			foreach (int item2 in holeIndex)
			{
				gemIndex.Add((!isRight) ? (item2 - 1) : (item2 + 1));
			}
			break;
		}
		case 7:
			break;
		case 8:
			if (isRight)
			{
				for (int i = 0; i < 12; i++)
				{
					gemIndex.Add(i);
				}
			}
			break;
		}
	}

	public void LogRocks()
	{
		Rock rock = GetCollideRock(false);
		while (rock != null && rock.IsActive())
		{
			Util.d("log " + rock.GetLeftUpGridX());
			rock = rock.rightRock;
		}
	}

	public void UpdateRockTheme()
	{
		foreach (Rock upRock in upRocks)
		{
			upRock.UpdateTheme(themeID);
		}
		foreach (Rock downRock in downRocks)
		{
			downRock.UpdateTheme(themeID);
		}
	}

	public void MakeRocks(bool doClean, int skillID = -1)
	{
		int posX = 0;
		int num = 0;
		int prevHeight = 0;
		int prevWidth = 0;
		Rock rock = null;
		Rock rock2 = null;
		holeRocks.Clear();
		if (doClean)
		{
			rock = GetCollideRock(true);
			foreach (Rock upRock in upRocks)
			{
				if (upRock != rock)
				{
					upRock.SetActive(false);
				}
			}
			if (rock != null)
			{
				rock.UpdateTheme(themeID);
			}
			upRocks.Clear();
			rock2 = GetCollideRock(false);
			foreach (Rock downRock in downRocks)
			{
				if (downRock != rock2)
				{
					downRock.SetActive(false);
				}
			}
			if (rock2 != null)
			{
				rock2.UpdateTheme(themeID);
			}
			downRocks.Clear();
			int count = gems.Count;
			for (int num2 = count - 1; num2 >= 0; num2--)
			{
				gems[num2].SetActive(false);
			}
			gems.Clear();
			if ((bool)rock2)
			{
				downRocks.Add(rock2);
				upRocks.Add(rock);
				num = rock2.GetLeftUpGridX();
				posX = num + rock2.GetGridWidth();
				prevWidth = rock2.GetGridWidth();
				prevHeight = rock2.GetGridHeight2();
				lastRightRock = rock2;
			}
			else
			{
				Util.d("no collideRock");
			}
			if (isUpRockSlide && (bool)rock)
			{
				rock.SetOffsetAfter((!rock.IsHalf()) ? ((float)(-PLAYER_HEIGHT) * GRID_SCALE) : ((float)(-PLAYER_HEIGHT) * GRID_SCALE * 0.5f), 0.1f, 0.2f);
			}
		}
		isUpRockSlide = true;
		mapTurn++;
		MakeSubRocks(true, false, posX, baseY, prevWidth, prevHeight, skillID);
		prevWidth = 0;
		prevHeight = 0;
		if ((bool)rock2)
		{
			prevWidth = rock2.GetGridWidth();
			prevHeight = rock2.GetGridHeight2();
		}
		posX = num;
		MakeSubRocks(false, false, posX, baseY, prevWidth, prevHeight, skillID);
		if (maxIndicator.gameObject.transform.position.x < Hero.Instance.gameObject.transform.position.x)
		{
			maxIndicator.gameObject.SetActive(false);
		}
		if (distanceIndicator.gameObject.transform.position.x < Hero.Instance.gameObject.transform.position.x)
		{
			distanceIndicator.gameObject.SetActive(false);
		}
	}

	public void SetExtraStopDuration(float duration)
	{
		extraStopDuration = duration;
	}

	public void UpdateInDown(float deltaTime, downDoneCallback callback)
	{
		offsetY -= downVelY * deltaTime;
		isShowHintDusts = false;
		if (offsetY <= 0f)
		{
			offsetY = 0f;
			int num = 0;
			foreach (Rock downRock in downRocks)
			{
				if (downRock.IsInCamera())
				{
					if (num % 2 == 1)
					{
						CreateRockFrag(downRock.gameObject.transform.position.x, downRock.gameObject.transform.position.y + (float)downRock.GetGridHeight() * GRID_SCALE * 0.5f);
					}
					num++;
				}
			}
			callback();
		}
		foreach (Rock upRock in upRocks)
		{
			upRock.SetOffsetY(offsetY);
		}
	}

	public void UpdateInUp(float deltaTime, upDoneCallback callback)
	{
		offsetY += downVelY * deltaTime;
		if (offsetY >= (float)maxGridOffsetY * GRID_SCALE)
		{
			offsetY = (float)maxGridOffsetY * GRID_SCALE;
			ShowHintDusts();
			callback();
		}
		foreach (Rock upRock in upRocks)
		{
			if ((bool)upRock)
			{
				upRock.SetOffsetY(offsetY);
			}
		}
		UpdateEffect();
	}

	public void ShowHintDusts()
	{
		isShowHintDusts = true;
		bool flag = Hero.Instance.GetActiveSkill() == 3;
		if (!flag && !GameController.Instance.IsShowHint())
		{
			return;
		}
		foreach (Rock holeRock in holeRocks)
		{
			if (holeRock.IsActive())
			{
				if (flag)
				{
					StartCoroutine(CreateHintDusts(holeRock.counterRock));
				}
				holeRock.UpdateEffectObject(true);
				if ((bool)holeRock.counterRock)
				{
					holeRock.counterRock.UpdateEffectObject(true);
				}
			}
		}
	}

	private void UpdateEffect()
	{
		if (Time.frameCount % 500 == 0)
		{
			StartCoroutine(Util.Shake(BGPanel, 1f, 0.1f, 1f, 1f));
		}
		else if (Time.frameCount % 100 == 0)
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane));
			Vector3 vector2 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));
			for (int i = 0; i < 2; i++)
			{
				float x = UnityEngine.Random.Range(vector.x, vector2.x);
				float y = UnityEngine.Random.Range(vector2.y - 1f, vector2.y);
				CreateDust(3, x, y, -1f, 0f, 2f);
			}
		}
	}

	public void UpdateInStop(float elapseTime, stopDoneCallback callback)
	{
		if (elapseTime >= upStopDuration + extraStopDuration)
		{
			callback();
		}
		else
		{
			UpdateEffect();
		}
	}

	private IEnumerator CreateHintDusts(Rock rock)
	{
		while (isShowHintDusts)
		{
			if (rock.IsInCamera())
			{
				float xPos = UnityEngine.Random.Range((float)rock.GetLeftUpGridX() * GRID_SCALE, (float)(rock.GetLeftUpGridX() + rock.GetGridWidth()) * GRID_SCALE);
				CreateDust(2, xPos, rock.gameObject.transform.position.y - (float)rock.GetGridHeight() * GRID_SCALE * 0.5f, -1f, 0f, 1.5f);
			}
			yield return new WaitForSeconds(0.05f);
		}
		yield return null;
	}

	public void UpdateInDownStop(float elapseTime, stopDoneCallback callback)
	{
		if (elapseTime >= downStopDuration)
		{
			callback();
		}
	}

	public int GetMapDataID()
	{
		return mapDataID;
	}

	public void CheckMapData()
	{
		int distance = GameController.Instance.GetDistance();
		int slamCount = GameController.Instance.GetSlamCount();
		int num = (int)(((float)slamCount / 3f + (float)(distance / 30)) * 0.5f);
		if (mapDataID != num)
		{
			mapDataID = num;
			mapData = MapManager.Instance.GetData(mapDataID);
			mapTurn = 0;
			Util.d("UpdateMapData: " + mapData.phase);
		}
	}

	public void UpdateTheme(int slamCount)
	{
		Debug.Log("UpdateTheme");
		int num = 0;
		bool flag = GameController.Instance.GetPlayCount() % 2 == 0;
		if (mapDataID < 0 || mapDataID > 2)
		{
			num = ((mapDataID >= 3 && mapDataID <= 5) ? (flag ? 1 : 3) : ((mapDataID < 6 || mapDataID > 8) ? ((!flag) ? 1 : 3) : 2));
		}
		if (num != themeID)
		{
			LoadTheme(themeID, num);
			LoadParticles(num);
		}
	}

	public void UpdateMap()
	{
		if ((bool)lastRightRock && lastRightRock.IsRightIn())
		{
			TruncateRocks();
			AppendRocks();
		}
		CheckEvent();
		CheckMapData();
	}

	public IEnumerator PrepareRockDown(prepareDoneCallback callback)
	{
		foreach (Rock rock in upRocks)
		{
			StartCoroutine(Util.Shake(rock.gameObject, upShakeDuration, 0.2f, 1f, 0.5f));
			if (rock.IsInCamera())
			{
				StartCoroutine(CreateDusts(rock));
			}
		}
		yield return new WaitForSeconds(upShakeDuration + 0.2f);
		foreach (Rock rock2 in holeRocks)
		{
			if (rock2.IsActive())
			{
				rock2.UpdateEffectObject(false);
				if ((bool)rock2.counterRock)
				{
					rock2.counterRock.UpdateEffectObject(false);
				}
			}
		}
		callback();
	}

	private IEnumerator CreateDusts(Rock rock)
	{
		CreateDust(1, rock.gameObject.transform.position.x, rock.gameObject.transform.position.y - (float)rock.GetGridHeight() * GRID_SCALE * 0.5f + 0.5f, -1f, 0f, 1.5f);
		yield return new WaitForSeconds(0.5f);
		CreateDust(1, rock.gameObject.transform.position.x + 1f, rock.gameObject.transform.position.y - (float)rock.GetGridHeight() * GRID_SCALE * 0.5f + 0.5f, -1f, 0f, 1.5f);
		yield return null;
	}

	public Rock GetCollideRock(bool isUp)
	{
		List<Rock> list = ((!isUp) ? downRocks : upRocks);
		Hero player = GameController.Instance.GetPlayer();
		foreach (Rock item in list)
		{
			if (item.IsActive() && item.HavePlayer(player))
			{
				return item;
			}
		}
		return null;
	}

	public void CleanUp()
	{
		foreach (Rock upRock in upRocks)
		{
			upRock.SetActive(false);
		}
		foreach (Rock downRock in downRocks)
		{
			downRock.SetActive(false);
		}
		upRocks.Clear();
		downRocks.Clear();
		holeRocks.Clear();
		foreach (Gem gem in gems)
		{
			gem.SetActive(false, false);
		}
		gems.Clear();
		foreach (Dust dust in dusts)
		{
			dust.SetActive(false, false);
		}
		dusts.Clear();
	}

	public void UpdateTitleRocks()
	{
		int count = upRocks.Count;
		baseY = 0;
		for (int num = count - 1; num >= 0; num--)
		{
			Rock rock = upRocks[num];
			if (rock.IsLeftOut())
			{
				rock.SetActive(false);
				upRocks.Remove(rock);
			}
		}
		count = downRocks.Count;
		for (int num2 = count - 1; num2 >= 0; num2--)
		{
			Rock rock2 = downRocks[num2];
			if (rock2.IsLeftOut())
			{
				rock2.SetActive(false);
				downRocks.Remove(rock2);
			}
		}
		Rock rock3 = null;
		while (lastRightRock == null || lastRightRock.IsRightIn())
		{
			int num3 = ((!lastRightRock) ? 2 : lastRightRock.GetGridHeight2());
			int gridX = ((!lastRightRock) ? ((int)((float)(-GameController.SCREEN_WIDTH) / GameController.CAMERA_SCALE)) : (lastRightRock.GetLeftUpGridX() + lastRightRock.GetGridWidth()));
			int gridWidth = UnityEngine.Random.Range(minWidth, maxWidth);
			int num4 = UnityEngine.Random.Range(Mathf.Max(minHeight, num3 - JUMP_HEIGHT), Mathf.Min(maxHeight, num3 + JUMP_HEIGHT));
			rock3 = CreateRock(gridX, 12 + maxGridOffsetY, gridWidth, 12 - num4 + maxGridOffsetY, baseY, false, true, false, false, false);
			lastRightRock = CreateRock(gridX, num4, gridWidth, num4 + maxGridOffsetY, baseY, false, false, false, false, false);
			titleRockCount++;
		}
		if (titleRockCount > 5)
		{
			CreateBirds(rock3, 3);
			titleRockCount = 0;
		}
		if (Time.frameCount % 300 == 0)
		{
			StartCoroutine(Util.Shake(BGPanel, 1f, 0.1f, 1f, 1f));
		}
	}

	public bool IsHoleOut()
	{
		foreach (Rock holeRock in holeRocks)
		{
			if (holeRock.IsActive() && (holeRock.IsInCamera() || (float)holeRock.GetLeftUpGridX() * GRID_SCALE < Hero.Instance.GetX()))
			{
				return false;
			}
		}
		return true;
	}

	public Rock GetOutScreenHoleRock()
	{
		Rock result = null;
		float num = 1000f;
		foreach (Rock holeRock in holeRocks)
		{
			if (holeRock.IsActive() && holeRock.IsInCamera())
			{
				return null;
			}
			float num2 = holeRock.GetX() - Hero.Instance.GetX();
			if (num2 < num)
			{
				result = holeRock;
				num = num2;
			}
		}
		return result;
	}

	public void SetFollowGem(bool isFollow)
	{
		foreach (Gem gem in gems)
		{
			if (gem.IsActive())
			{
				gem.SetFollowGem(isFollow, false);
			}
		}
		isGemFollow = isFollow;
	}

	public void LoadTheme(int prevThemeID, int themeID)
	{
		if (prevThemeID == themeID)
		{
			return;
		}
		Rock.LoadTheme(themeID);
		string[] array = new string[3] { "bg00", "bg01", "bg02" };
		if (themeID != 4)
		{
			string[] array2 = array;
			foreach (string text in array2)
			{
				BG component = BGPanel.transform.Find(text).GetComponent<BG>();
				component.LoadTheme(themeID);
			}
		}
		this.themeID = themeID;
	}

	public void Fade(bool dark, fadeCallback callback)
	{
		StartCoroutine(_fade(dark, callback));
	}

	private IEnumerator _fade(bool dark, fadeCallback callback)
	{
		whitePanel.SetActive(true);
		tk2dSprite spr = whitePanel.GetComponent<tk2dSprite>();
		if (dark)
		{
			float alpha = 0f;
			while (alpha < 1f)
			{
				spr.color = new Color(0f, 0f, 0f, alpha);
				alpha += 0.1f;
				yield return new WaitForSeconds(0.04f);
			}
		}
		else
		{
			float alpha2 = 1f;
			while (alpha2 > 0f)
			{
				spr.color = new Color(0f, 0f, 0f, alpha2);
				alpha2 -= 0.08f;
				yield return new WaitForSeconds(0.04f);
			}
			whitePanel.SetActive(false);
		}
		callback();
	}

	private IEnumerator _blink(int count)
	{
		bool show = true;
		for (int i = 0; i < count; i++)
		{
			whitePanel.SetActive(show);
			show = !show;
			yield return new WaitForSeconds(0.08f);
		}
		whitePanel.SetActive(true);
		tk2dSprite spr = whitePanel.GetComponent<tk2dSprite>();
		float alpha = 1f;
		while (alpha > 0f)
		{
			spr.color = new Color(1f, 1f, 1f, alpha);
			alpha -= 0.04f;
			yield return new WaitForSeconds(0.04f);
		}
		spr.color = new Color(1f, 1f, 1f, 1f);
		whitePanel.SetActive(false);
	}

	public void Blink()
	{
		StartCoroutine(_blink(4));
	}

	public void InitRockOffset()
	{
		foreach (Rock upRock in upRocks)
		{
			upRock.SetOffsetY(0f);
		}
	}

	public void SetSlow(bool isSlow)
	{
		Util.d("SetSlow " + isSlow);
		if (isSlow)
		{
			downVelYAddVal = (0f - mapData.downVelY) * 0.5f;
			upShakeDurationAddVal = mapData.upShakeDuration * 1.5f;
			upStopDurationAddVal = mapData.upStopDuration * 1.5f;
		}
		else
		{
			downVelYAddVal = 0f;
			upShakeDurationAddVal = 0f;
			upStopDurationAddVal = 0f;
		}
	}

	public void DoHulk()
	{
		StartCoroutine(Util.Shake(Camera.main.gameObject, 0.1f, 0.1f, 1f, 1f));
		GameController.Instance.ChangeStateForce(GameController.PLAY_STATE.ST_HOLD);
		Invoke("_DoHulk", 0.5f);
	}

	private void _DoHulk()
	{
		GameController.Instance.ChangeStateForce(GameController.PLAY_STATE.ST_UP);
	}
}
