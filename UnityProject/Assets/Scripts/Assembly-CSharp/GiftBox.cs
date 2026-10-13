using DG.Tweening;
using UnityEngine;

public class GiftBox : MonoBehaviour
{
	public GameObject giftSprite;

	public tk2dTextMesh countText;

	public tk2dSpriteAnimator gemAnim;

	public tk2dSpriteAnimator fxAnim;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnClick(int id, int count)
	{
		giftSprite.SetActive(false);
		fxAnim.gameObject.SetActive(true);
		gemAnim.gameObject.SetActive(true);
		countText.gameObject.SetActive(true);
		fxAnim.AnimationCompleted = (tk2dSpriteAnimator a, tk2dSpriteAnimationClip c) =>
		{
			fxAnim.gameObject.SetActive(false);
		};
		fxAnim.Play();
		PlayerData dataByID = PlayerManager.Instance.GetDataByID(id);
		gemAnim.Play(string.Format("gem{0}Clip", dataByID.gemId));
		gemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
		gemAnim.transform.DOScale(new Vector3(2f, 2f, 1f), 0.5f);
		countText.text = count.ToString();
		countText.transform.localScale = new Vector3(1f, 1f, 1f);
		countText.transform.DOScale(new Vector3(2f, 2f, 1f), 0.5f);
	}

	public void Show()
	{
		giftSprite.SetActive(true);
		fxAnim.gameObject.SetActive(false);
		gemAnim.gameObject.SetActive(false);
		countText.gameObject.SetActive(false);
	}
}
