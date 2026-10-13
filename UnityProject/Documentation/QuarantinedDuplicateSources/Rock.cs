using System.Collections;
using UnityEngine;

public class Rock : MonoBehaviour
{
	public delegate void InactiveCB();

	public InactiveCB OnInactivated;

	private int leftUpGridX;

	private int leftUpGridY;

	private int gridWidth;

	private int gridHeight;

	public int maxGridOffsetY;

	private float width;

	private float height;

	private float x;

	private float y;

	private float offsetY;

	private float offsetY2;

	private int gridBaseY;

	private bool isUp;

	private bool isHalf;

	private bool isHole;

	private bool showHint;

	private bool isActive;

	private float nextWidth;

	private float nextHeight;

	private float nextRemainTime;

	[HideInInspector]
	public Rock rightRock;

	[HideInInspector]
	public Rock leftRock;

	[HideInInspector]
	public Rock counterRock;

	public GameObject floorObject;

	public GameObject effectObject;

	private static Texture currentTexture;

	private static Texture currentCeilTexture;

	private static Texture currentFloorTexture;

	private static float[] ceilHeights = new float[5] { 0.1f, 0.17f, 0.16f, 0.17f, 0.1f };

	private static float[] floorHeights = new float[5] { 0.1f, 0.2f, 0.16f, 0.17f, 0.1f };

	private static int currentThemeID;

	private bool isHammered;

	public float GetX()
	{
		return x;
	}

	public int GetLeftUpGridX()
	{
		return leftUpGridX;
	}

	public int GetLeftUpGridY()
	{
		return leftUpGridY;
	}

	public int GetGridWidth()
	{
		return gridWidth;
	}

	public int GetGridHeight()
	{
		return gridHeight;
	}

	public int GetGridHeight2()
	{
		return gridHeight - maxGridOffsetY;
	}

	public float GetBaseY()
	{
		return gridBaseY;
	}

	public bool IsUp()
	{
		return isUp;
	}

	public bool IsHalf()
	{
		return isHalf;
	}

	public bool IsHole()
	{
		return isHole;
	}

	private void Start()
	{
	}

	public bool IsActive()
	{
		return isActive;
	}

	public void SetActive(bool isActive)
	{
		this.isActive = isActive;
		base.gameObject.SetActive(isActive);
	}

	public static void LoadTheme(int themeID)
	{
		currentTexture = Resources.Load($"bg_{themeID}_stage", typeof(Texture2D)) as Texture;
		currentCeilTexture = Resources.Load($"bg_{themeID}_stage_ceiling", typeof(Texture2D)) as Texture;
		currentFloorTexture = Resources.Load($"bg_{themeID}_stage_floor", typeof(Texture2D)) as Texture;
		currentThemeID = themeID;
	}

	public static Texture GetTexture()
	{
		return currentTexture;
	}

	public void Demolish()
	{
		InitWith(leftUpGridX, leftUpGridY, gridWidth, (!isHalf) ? (gridHeight - 2) : (gridHeight - 1), gridBaseY, 0f, isUp, isHole, isHalf: false, showHint: false);
	}

	public void DemolishUp()
	{
		if (!isHammered)
		{
			float num = offsetY;
			InitWith(leftUpGridX, leftUpGridY, gridWidth, gridHeight - 2, gridBaseY, 0f, isUp, isHole, isHalf: false, showHint: false);
			SetOffsetY(num);
			isHammered = true;
		}
	}

	public void DemolishDown()
	{
		if (!isHammered)
		{
			InitWith(leftUpGridX, leftUpGridY - 1, gridWidth, gridHeight - 1, gridBaseY, 0f, isUp, isHole, isHalf: false, showHint: false);
			isHammered = true;
		}
	}

	public void SetGridY(int newGridY)
	{
		InitWith(leftUpGridX, newGridY, gridWidth, gridHeight, gridBaseY, offsetY, isUp, isHole, isHalf, showHint: false);
	}

	public void InitWith(int leftUpGridX, int leftUpGridY, int gridWidth, int gridHeight, int gridBaseY, float offsetY, bool isUp, bool isHole, bool isHalf, bool showHint)
	{
		this.leftUpGridX = leftUpGridX;
		this.leftUpGridY = leftUpGridY;
		this.gridWidth = gridWidth;
		this.gridHeight = gridHeight;
		this.gridBaseY = gridBaseY;
		x = ((float)leftUpGridX + (float)gridWidth * 0.5f) * Map.GRID_SCALE;
		y = ((float)leftUpGridY - (float)gridHeight * 0.5f + (float)gridBaseY) * Map.GRID_SCALE;
		this.isUp = isUp;
		this.isHole = isHole;
		this.isHalf = isHalf;
		this.showHint = showHint;
		rightRock = null;
		leftRock = null;
		counterRock = null;
		offsetY2 = 0f;
		SetOffsetY(offsetY);
		nextRemainTime = -1f;
		BoxCollider2D component = base.gameObject.transform.GetComponent<BoxCollider2D>();
		width = (float)gridWidth * Map.GRID_SCALE;
		height = (float)gridHeight * Map.GRID_SCALE;
		component.size = new Vector2(width, height);
		Mesh mesh = base.gameObject.transform.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3((0f - width) * 0.5f, (0f - height) * 0.5f, 0f),
			new Vector3(width * 0.5f, height * 0.5f, 0f),
			new Vector3(width * 0.5f, (0f - height) * 0.5f, 0f),
			new Vector3((0f - width) * 0.5f, height * 0.5f, 0f)
		};
		mesh.vertices = vertices;
		float cAMERA_SCALE = GameController.CAMERA_SCALE;
		float num = cAMERA_SCALE * width / (float)GameController.SCREEN_WIDTH;
		float num2 = cAMERA_SCALE * height / (float)GameController.SCREEN_HEIGHT;
		float num3 = cAMERA_SCALE * x / (float)GameController.SCREEN_WIDTH;
		float num4 = num3;
		float num5 = cAMERA_SCALE * (y + offsetY) / (float)GameController.SCREEN_HEIGHT;
		if (base.renderer.material.mainTexture != currentTexture)
		{
			base.renderer.material.mainTexture = currentTexture;
		}
		base.renderer.material.mainTextureOffset = new Vector2(num4, num5);
		base.renderer.material.mainTextureScale = new Vector2(num, num2);
		base.renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
		mesh.RecalculateBounds();
		SetActive(isActive: true);
		UpdateFloorObject();
		UpdateEffectObject(showHint);
		isHammered = false;
	}

	public void UpdateEffectObject(bool showHint)
	{
		this.showHint = showHint;
		if (showHint && isHole)
		{
			effectObject.SetActive(value: true);
			tk2dSprite component = effectObject.GetComponent<tk2dSprite>();
			if (isUp)
			{
				component.SetSprite("actor_03_skill_fx_0");
				component.transform.localPosition = new Vector3(0f, (0f - (float)gridHeight * 0.5f) * Map.GRID_SCALE, 0f);
				float a = 1f;
				switch (GameController.Instance.GetSlamCount())
				{
				case 0:
					a = 1f;
					break;
				case 1:
					a = 0.6f;
					break;
				case 2:
					a = 0.3f;
					break;
				}
				component.color = new Color(1f, 1f, 1f, a);
			}
			else
			{
				component.SetSprite("actor_03_skill_fx_1");
				component.transform.localPosition = new Vector3(0f, (float)gridHeight * 0.5f * Map.GRID_SCALE, 0f);
			}
		}
		else
		{
			effectObject.SetActive(value: false);
		}
	}

	public void UpdateFloorObject()
	{
		Mesh mesh = floorObject.transform.GetComponent<MeshFilter>().mesh;
		float num;
		float num2;
		if (isUp)
		{
			num = (0f - height) * 0.5f + ceilHeights[currentThemeID];
			num2 = (0f - height) * 0.5f;
		}
		else
		{
			num = height * 0.5f;
			num2 = height * 0.5f - floorHeights[currentThemeID];
		}
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3((0f - width) * 0.5f, num2, 0f),
			new Vector3(width * 0.5f, num, 0f),
			new Vector3(width * 0.5f, num2, 0f),
			new Vector3((0f - width) * 0.5f, num, 0f)
		};
		mesh.vertices = vertices;
		float cAMERA_SCALE = GameController.CAMERA_SCALE;
		float num3 = cAMERA_SCALE * width / 145f;
		float num4 = 1f;
		Texture texture = ((!isUp) ? currentFloorTexture : currentCeilTexture);
		if (floorObject.renderer.material.mainTexture != texture)
		{
			floorObject.renderer.material.mainTexture = texture;
		}
		floorObject.renderer.material.mainTextureOffset = new Vector2(0f, 0f);
		floorObject.renderer.material.mainTextureScale = new Vector2(num3, num4);
		floorObject.renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
		mesh.RecalculateBounds();
	}

	public void UpdateTheme(int themeID)
	{
		if (base.renderer.material.mainTexture != currentTexture)
		{
			base.renderer.material.mainTexture = currentTexture;
			UpdateFloorObject();
		}
	}

	public void SetOffsetY2(float offsetY2)
	{
		this.offsetY2 = offsetY2;
	}

	public float GetOffsetY2()
	{
		return offsetY2;
	}

	public void SetOffsetY(float offsetY)
	{
		this.offsetY = offsetY;
		base.gameObject.transform.position = new Vector3(x, y + offsetY + offsetY2, -1f);
	}

	public bool HavePlayer(Hero player)
	{
		float num = player.gameObject.transform.position.x;
		if (num >= (float)leftUpGridX * Map.GRID_SCALE && num <= (float)(leftUpGridX + gridWidth) * Map.GRID_SCALE)
		{
			return true;
		}
		return false;
	}

	public void SetOffsetAfter(float offset, float after, float during)
	{
		StartCoroutine(SetOffsetY2(offset, after, during));
	}

	private IEnumerator SetOffsetY2(float offset, float after, float during)
	{
		offsetY2 = 0f;
		yield return new WaitForSeconds(after);
		float vel = offset / during;
		while (offsetY2 > offset)
		{
			offsetY2 += vel * Time.deltaTime;
			yield return null;
		}
		offsetY2 = offset;
		isHole = false;
	}

	public bool IsLeftOut(int buffer = 0)
	{
		if ((float)(leftUpGridX + gridWidth + buffer) * Map.GRID_SCALE < Camera.main.transform.position.x - GameController.VIEW_WIDTH / 2f)
		{
			return true;
		}
		return false;
	}

	public bool IsRightIn()
	{
		if ((float)(leftUpGridX + gridWidth) * Map.GRID_SCALE < Camera.main.transform.position.x + GameController.VIEW_WIDTH / 2f)
		{
			return true;
		}
		return false;
	}

	public bool IsInCamera()
	{
		if ((float)(leftUpGridX + gridWidth) * Map.GRID_SCALE < Camera.main.transform.position.x - GameController.VIEW_WIDTH / 2f)
		{
			return false;
		}
		if ((float)leftUpGridX * Map.GRID_SCALE > Camera.main.transform.position.x + GameController.VIEW_WIDTH / 2f)
		{
			return false;
		}
		return true;
	}
}
