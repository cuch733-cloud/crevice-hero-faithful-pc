using UnityEngine;

public class GemBox : MonoBehaviour
{
	private tk2dSpriteAnimator anim;

	private int gemID;

	private bool isActive;

	private bool isHit;

	private void Awake()
	{
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	public void InitWith(float x, float y, int gemID)
	{
		Util.d("GemBox");
		this.gemID = gemID;
		base.gameObject.transform.position = new Vector3(x, y, 0f);
		SetActive(true);
		tk2dSpriteAnimationClip clipByName = anim.GetClipByName("gemboxClip");
		anim.Play(clipByName);
		anim.AnimationCompleted = null;
		base.renderer.enabled = true;
		isHit = false;
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

	public void Hit()
	{
		if (IsActive() && !isHit)
		{
			tk2dSpriteAnimationClip clipByName = anim.GetClipByName("gemboxBurstClip");
			anim.Play(clipByName);
			anim.AnimationCompleted = AnimationCompleted;
			Map.Instance.GemBoxBurst(base.transform.position.x, base.transform.position.y, gemID);
			isHit = true;
		}
	}

	private void AnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		anim.AnimationCompleted = null;
		SetActive(false);
	}

	private void Update()
	{
		if (isActive)
		{
			int activeSkill = Hero.Instance.GetActiveSkill();
			if ((activeSkill == 8 || activeSkill == 5) && Hero.Instance.GetX() > base.transform.position.x)
			{
				Hit();
			}
		}
	}
}
