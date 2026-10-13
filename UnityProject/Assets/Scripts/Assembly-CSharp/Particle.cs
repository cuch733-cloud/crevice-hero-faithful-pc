using UnityEngine;

public class Particle : MonoBehaviour
{
	public delegate void InactiveCB();

	public int type;

	private tk2dSprite sprite;

	private tk2dSpriteAnimator anim;

	public InactiveCB OnInactivated;

	private bool isActive;

	private bool isMoving;

	private float velX;

	private float velY;

	private float updateTime;

	private static float[] fogVelXs = new float[3] { -1f, -0.7f, -0.5f };

	private static float[] skySmokeVelXs = new float[3] { -1.5f, -1f, -0.7f };

	private static float[] velYs = new float[4] { -0.2f, -0.1f, 0.1f, 0.2f };

	private static float[] leafVelYs = new float[2] { -0.8f, -0.9f };

	private static float[] snowVelYs = new float[2] { -1f, -1.1f };

	private float initY;

	private void Start()
	{
	}

	public void InitWith(float x, float y, int type)
	{
		this.type = type;
		initY = y;
		base.gameObject.transform.position = new Vector3(x, y, -1f);
		sprite = base.gameObject.GetComponent<tk2dSprite>();
		anim = GetComponent<tk2dSpriteAnimator>();
		if ((bool)anim)
		{
			anim.Stop();
		}
		switch (type)
		{
		case 0:
			sprite.SetSprite(string.Format("bg_0_fog_{0}", Random.Range(0, 3)));
			velX = fogVelXs[Random.Range(0, fogVelXs.Length)];
			velY = 0f;
			isMoving = true;
			break;
		case 1:
			anim.PlayFromFrame("smokeClip", Random.Range(0, 5));
			isMoving = false;
			break;
		case 2:
			anim.PlayFromFrame("leafClip", Random.Range(0, 5));
			velX = skySmokeVelXs[Random.Range(0, skySmokeVelXs.Length)];
			velY = leafVelYs[Random.Range(0, leafVelYs.Length)];
			isMoving = true;
			break;
		case 3:
			sprite.SetSprite(string.Format("bg_1_glow_0{0}", Random.Range(0, 2)));
			velX = skySmokeVelXs[Random.Range(0, skySmokeVelXs.Length)];
			velY = velYs[Random.Range(0, velYs.Length)];
			isMoving = true;
			break;
		case 4:
			sprite.SetSprite(string.Format("bg_3_flare_{0}", Random.Range(0, 3)));
			velX = skySmokeVelXs[Random.Range(0, skySmokeVelXs.Length)];
			velY = velYs[Random.Range(0, velYs.Length)];
			isMoving = true;
			break;
		case 5:
			sprite.SetSprite(string.Format("bg_2_skysmoke_{0}", Random.Range(0, 3)));
			velX = skySmokeVelXs[Random.Range(0, skySmokeVelXs.Length)];
			velY = velYs[Random.Range(0, velYs.Length)];
			isMoving = true;
			break;
		case 6:
			sprite.SetSprite(string.Format("snow_0{0}", Random.Range(0, 2)));
			velX = 0f;
			velY = snowVelYs[Random.Range(0, snowVelYs.Length)];
			isMoving = true;
			break;
		}
		SetActive(true);
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

	private void Update()
	{
		if (isActive)
		{
			if (base.transform.position.x < Hero.Instance.GetX() - GameController.VIEW_WIDTH)
			{
				base.transform.position = new Vector3(base.transform.position.x + Random.Range(GameController.VIEW_WIDTH * 2f, GameController.VIEW_WIDTH * 2.5f), initY, base.transform.position.z);
			}
			else if (isMoving)
			{
				base.transform.position = new Vector3(base.transform.position.x + velX * Time.deltaTime, base.transform.position.y + velY * Time.deltaTime, base.transform.position.z);
			}
		}
	}
}
