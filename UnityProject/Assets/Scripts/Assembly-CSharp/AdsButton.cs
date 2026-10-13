using DG.Tweening;
using UnityEngine;

public class AdsButton : MonoBehaviour
{
	public GameObject adsSprite;

	public tk2dTextMesh countText1;

	public tk2dSpriteAnimator gemAnim1;

	public tk2dTextMesh countText2;

	public tk2dSpriteAnimator gemAnim2;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnAdsEnd(int id1, int count1, int id2, int count2)
	{
		adsSprite.SetActive(false);
		gemAnim1.gameObject.SetActive(true);
		gemAnim2.gameObject.SetActive(true);
		countText1.gameObject.SetActive(true);
		countText2.gameObject.SetActive(true);
		PlayerData dataByID = PlayerManager.Instance.GetDataByID(id1);
		gemAnim1.Play(string.Format("gem{0}Clip", dataByID.gemId));
		gemAnim1.transform.localScale = new Vector3(1f, 1f, 1f);
		gemAnim1.transform.DOScale(new Vector3(2f, 2f, 1f), 0.5f);
		countText1.text = count1.ToString();
		countText1.transform.localScale = new Vector3(1f, 1f, 1f);
		countText1.transform.DOScale(new Vector3(2f, 2f, 1f), 0.5f);
		PlayerData dataByID2 = PlayerManager.Instance.GetDataByID(id2);
		gemAnim2.Play(string.Format("gem{0}Clip", dataByID2.gemId));
		gemAnim2.transform.localScale = new Vector3(1f, 1f, 1f);
		gemAnim2.transform.DOScale(new Vector3(2f, 2f, 1f), 0.5f);
		countText2.text = count2.ToString();
		countText2.transform.localScale = new Vector3(1f, 1f, 1f);
		countText2.transform.DOScale(new Vector3(2f, 2f, 1f), 0.5f);
	}

	public void Show()
	{
		adsSprite.SetActive(true);
		gemAnim1.gameObject.SetActive(false);
		gemAnim2.gameObject.SetActive(false);
		countText1.gameObject.SetActive(false);
		countText2.gameObject.SetActive(false);
	}
}
