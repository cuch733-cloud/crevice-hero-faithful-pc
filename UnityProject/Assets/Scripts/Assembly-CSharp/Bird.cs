using UnityEngine;

public class Bird : MonoBehaviour
{
	public delegate void InactiveCB();

	private tk2dSpriteAnimator anim;

	public InactiveCB OnInactivated;

	private bool isActive;

	private float velX;

	private float velY;

	private float updateTime;

	private static float[] velYs = new float[5] { -0.2f, -0.1f, 0.2f, 0.3f, 0.5f };

	private bool isFlying;

	public void InitWith(float x, float y, bool isFlying)
	{
		this.isFlying = isFlying;
		base.gameObject.transform.position = new Vector3(x, y, -1f);
		anim = base.gameObject.GetComponent<tk2dSpriteAnimator>();
		if (isFlying)
		{
			anim.Play("moveClip");
			UpdateDirection();
		}
		else
		{
			anim.Play("stopClip");
		}
		SetActive(true);
	}

	private void UpdateDirection()
	{
		velX = Random.Range(-6f, -3f);
		velY = velYs[Random.Range(0, velYs.Length - 1)];
		updateTime = Random.Range(1f, 3f);
	}

	public bool IsActive()
	{
		return isActive;
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		if (isFlying)
		{
			base.transform.position = new Vector3(base.transform.position.x + velX * Time.deltaTime, base.transform.position.y + velY * Time.deltaTime, base.transform.position.z);
			if (IsOut())
			{
				SetActive(false);
				return;
			}
			updateTime -= Time.deltaTime;
			if (updateTime < 0f)
			{
				UpdateDirection();
			}
		}
		else if (base.transform.position.x < Camera.main.transform.position.x + (float)GameController.SCREEN_WIDTH / GameController.CAMERA_SCALE / 4f)
		{
			anim.Play("moveClip");
			UpdateDirection();
			isFlying = true;
		}
	}

	private bool IsOut()
	{
		if (base.transform.position.x < Camera.main.transform.position.x - (float)GameController.SCREEN_WIDTH / GameController.CAMERA_SCALE / 2f)
		{
			return true;
		}
		return false;
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
}
