using UnityEngine;

public class RockFrag : MonoBehaviour
{
	public delegate void InactiveCB();

	private tk2dSpriteAnimator anim;

	private bool isActive;

	public InactiveCB OnInactivated;

	private void Awake()
	{
		anim = base.gameObject.GetComponent<tk2dSpriteAnimator>();
	}

	private void Start()
	{
	}

	public void InitWith(float x, float y)
	{
		base.gameObject.transform.position = new Vector3(x, y, -1f);
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
		if (isActive)
		{
			anim.Play();
			anim.AnimationCompleted = AnimationCompleted;
		}
		else if (callCB && OnInactivated != null)
		{
			OnInactivated();
		}
	}

	private void AnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		SetActive(false);
	}
}
