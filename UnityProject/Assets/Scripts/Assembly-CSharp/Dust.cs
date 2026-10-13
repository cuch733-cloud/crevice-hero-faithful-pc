using UnityEngine;

public class Dust : MonoBehaviour
{
	public delegate void InactiveCB();

	private float velY;

	private float velRot;

	private float velAlpha;

	private float duration;

	private float age;

	private bool isActive;

	public InactiveCB OnInactivated;

	private tk2dSprite sprite;

	private static string[] arr1 = new string[3] { "fx_4_00", "fx_4_01", "fx_4_02" };

	private static string[] arr2 = new string[3] { "fx_0_00", "fx_0_01", "fx_0_02" };

	private static string[] arr3 = new string[2] { "fx_6_00", "fx_6_01" };

	private static float[] velArr = new float[3] { -3f, -2.5f, -2f };

	private static float[] rotVelArr = new float[4] { -200f, -100f, 100f, 200f };

	private int index;

	private int type;

	private float initY;

	private void Awake()
	{
		sprite = base.gameObject.GetComponent<tk2dSprite>();
		base.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 90));
	}

	public void InitWith(int type, float x, float y, float velY, float velRot, float duration)
	{
		this.type = type;
		if (type == 1)
		{
			this.velRot = rotVelArr[Random.Range(0, rotVelArr.Length)];
		}
		this.duration = duration;
		switch (type)
		{
		case 1:
			index = Random.Range(0, arr1.Length);
			sprite.SetSprite(arr1[index]);
			break;
		case 2:
			index = Random.Range(0, arr2.Length);
			sprite.SetSprite(arr2[index]);
			break;
		default:
			index = Random.Range(0, arr3.Length);
			sprite.SetSprite(arr3[index]);
			break;
		}
		this.velY = velArr[index];
		age = 0f;
		velAlpha = 1f / duration;
		sprite.color = Color.white;
		base.gameObject.transform.position = new Vector3(x, y, 0f);
		initY = y;
		SetActive(true);
	}

	public bool IsActive()
	{
		return isActive;
	}

	public void SetActive(bool isActive, bool callCB = true)
	{
		this.isActive = isActive;
		base.gameObject.SetActive(isActive);
		if (callCB && !isActive && OnInactivated != null)
		{
			OnInactivated();
		}
	}

	public void Update()
	{
		if (isActive)
		{
			age += Time.deltaTime;
			Vector3 position = base.gameObject.transform.position;
			base.gameObject.transform.position = new Vector3(position.x, initY - 2f * age * age, position.z);
			if (type == 1)
			{
				base.gameObject.transform.Rotate(new Vector3(0f, 0f, velRot * Time.deltaTime));
			}
			if (type == 1)
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - velAlpha * Time.deltaTime);
			}
			else if (age >= duration * 0.5f)
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - velAlpha * Time.deltaTime);
			}
			if (age >= duration)
			{
				SetActive(false);
			}
		}
	}
}
