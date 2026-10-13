using UnityEngine;

public class Gem : MonoBehaviour
{
	public delegate void InactiveCB();

	public const int GEM_DROP_NONE = 0;

	public const int GEM_DROP_1 = 1;

	public const int GEM_DROP_2 = 2;

	public const int GEM_DROP_INVERVAL = 3;

	public const int GEM_DROP_OUT = 4;

	public const int GEM_DROP_HOLE = 5;

	public const int GEM_DROP_NEXT_HOLE = 6;

	public const int GEM_DROP_RARE = 7;

	public const int GEM_DROP_FULL = 8;

	public const int MAX_GEM_ID = 8;

	public GameObject effectObject;

	private bool isActive;

	private tk2dSpriteAnimator anim;

	private tk2dSpriteAnimator effectAnim;

	public InactiveCB OnInactivated;

	private int id;

	private bool isFollowGem;

	private bool isFollowing;

	private bool isForceFollowGem;

	private float coff;

	private int gemID;

	private bool isHitable;

	private bool isHit;

	private void Awake()
	{
		anim = GetComponent<tk2dSpriteAnimator>();
		effectAnim = effectObject.GetComponent<tk2dSpriteAnimator>();
	}

	public void InitWith(int playerID, float x, float y)
	{
		id = playerID;
		base.gameObject.transform.position = new Vector3(x, y, 0f);
		gemID = PlayerManager.Instance.GetGemIdByID(playerID);
		SetActive(isActive: true);
		tk2dSpriteAnimationClip clipByName = anim.GetClipByName($"gem{gemID}Clip");
		anim.Play(clipByName);
		anim.AnimationCompleted = null;
		isFollowing = false;
		isFollowGem = false;
		coff = 0f;
		effectAnim.gameObject.SetActive(value: false);
		base.renderer.enabled = true;
		isHitable = true;
		isHit = false;
	}

	public void SetHitable(bool hitable)
	{
		isHitable = hitable;
	}

	public int GetID()
	{
		return id;
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

	public void ShowEffect()
	{
		SetActive(isActive: true, callCB: false);
		effectAnim.gameObject.SetActive(value: true);
		effectAnim.Play();
		effectAnim.AnimationCompleted = ShowEffectAnimationCompleted;
	}

	private void ShowEffectAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		effectAnim.gameObject.SetActive(value: false);
	}

	public bool Hit()
	{
		if (isHit)
		{
			return false;
		}
		if (isHitable)
		{
			base.renderer.enabled = false;
			effectAnim.gameObject.SetActive(value: true);
			effectAnim.Play();
			effectAnim.AnimationCompleted = AnimationCompleted;
			isHit = true;
			return true;
		}
		return false;
	}

	private void AnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		SetActive(isActive: false);
	}

	private void UpdateFollowGem()
	{
		if (!isFollowGem)
		{
			return;
		}
		if (isFollowing)
		{
			coff += Time.deltaTime;
			if (coff <= 1f)
			{
				float x = Hero.Instance.transform.position.x * coff + base.transform.position.x * (1f - coff);
				float y = (Hero.Instance.transform.position.y + 0.5f) * coff + base.transform.position.y * (1f - coff);
				base.transform.position = new Vector3(x, y, base.transform.position.z);
			}
		}
		else if (isForceFollowGem)
		{
			isFollowing = true;
			coff = 0.1f;
		}
		else
		{
			float num = base.transform.position.x - Hero.Instance.transform.position.x;
			if (num >= 0f && num <= 2f)
			{
				isFollowing = true;
				coff = 0.1f;
			}
		}
	}

	public void SetFollowGem(bool isFollow, bool isForceFollow)
	{
		isFollowGem = isFollow;
		isForceFollowGem = isForceFollow;
	}

	private void Update()
	{
		if (isActive)
		{
			UpdateFollowGem();
		}
	}
}
