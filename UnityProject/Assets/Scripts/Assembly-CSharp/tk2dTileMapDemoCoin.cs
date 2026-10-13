using UnityEngine;

public class tk2dTileMapDemoCoin : MonoBehaviour
{
	public tk2dSpriteAnimator animator;

	private void Awake()
	{
		if (animator == null)
		{
			Debug.LogError("Coin - Assign animator in the inspector before proceeding.");
			base.enabled = false;
		}
		else
		{
			animator.enabled = false;
		}
	}

	private void OnBecameInvisible()
	{
		if (animator.enabled)
		{
			animator.enabled = false;
		}
	}

	private void OnBecameVisible()
	{
		if (!animator.enabled)
		{
			animator.enabled = true;
		}
	}
}
