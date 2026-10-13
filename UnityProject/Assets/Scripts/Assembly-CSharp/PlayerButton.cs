using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerButton : MonoBehaviour
{
	public GameObject platformPrefab;

	public GameObject gemPrefab;

	public GameObject gemCountPrefab;

	public GameObject playerPrefab;

	public GameObject lockedPrefab;

	public GameObject infoPrefab;

	public GameObject newPrefab;

	public GameObject gemEffectPrefab;

	public GameObject qmarkPrefab;

	public GameObject ballPrefab;

	private PlayerData charData;

	private tk2dSprite playerSprite;

	private tk2dSprite platformSprite;

	private tk2dSprite lockedPlayer;

	private tk2dSpriteAnimator gemAnim;

	private tk2dTextMesh gemCountText;

	private tk2dSpriteAnimator anim;

	private tk2dSpriteAnimator gemEffectAnim;

	private bool isSelected;

	private int curCount;

	private void Awake()
	{
		playerSprite = playerPrefab.GetComponent<tk2dSprite>();
		anim = playerPrefab.GetComponent<tk2dSpriteAnimator>();
		platformSprite = platformPrefab.GetComponent<tk2dSprite>();
		gemAnim = gemPrefab.GetComponent<tk2dSpriteAnimator>();
		gemEffectAnim = gemEffectPrefab.GetComponent<tk2dSpriteAnimator>();
		gemCountText = gemCountPrefab.GetComponent<tk2dTextMesh>();
		lockedPlayer = lockedPrefab.GetComponent<tk2dSprite>();
	}

	public int GetID()
	{
		if (charData == null)
		{
			return -1;
		}
		return charData.GetID();
	}

	public void SetDummy()
	{
		charData = null;
		anim.gameObject.SetActive(false);
		lockedPlayer.gameObject.SetActive(true);
		qmarkPrefab.SetActive(true);
		lockedPlayer.gameObject.SetActive(false);
		gemAnim.gameObject.SetActive(false);
		gemCountText.gameObject.SetActive(false);
		gemEffectAnim.gameObject.SetActive(false);
		platformSprite.SetSprite("result_prop_01");
		infoPrefab.SetActive(false);
		newPrefab.SetActive(false);
		ballPrefab.SetActive(false);
	}

	public void SetData(int id)
	{
		charData = PlayerManager.Instance.GetDataByID(id);
		Bounds bounds = playerSprite.CurrentSprite.GetBounds();
		bool flag = User.Instance.GetPlayerLevel(id) >= 1;
		qmarkPrefab.SetActive(false);
		if (flag)
		{
			anim.gameObject.SetActive(true);
			platformSprite.gameObject.SetActive(true);
			lockedPlayer.gameObject.SetActive(false);
			string path = string.Format("char{0}Animation", charData.animId);
			anim.Library = Resources.Load(path, typeof(tk2dSpriteAnimation)) as tk2dSpriteAnimation;
			anim.Play("stayClip");
			if (!isSelected)
			{
				anim.Stop();
			}
			platformSprite.SetSprite("result_prop_00");
		}
		else
		{
			anim.gameObject.SetActive(false);
			platformSprite.gameObject.SetActive(false);
			lockedPlayer.gameObject.SetActive(true);
			lockedPlayer.SetSprite(string.Format("result_char_{0:D2}_0", id));
			infoPrefab.SetActive(false);
		}
		if (User.Instance.IsUpgradable(id))
		{
			newPrefab.SetActive(true);
		}
		else
		{
			newPrefab.SetActive(false);
		}
		gemAnim.gameObject.SetActive(false);
		gemCountText.gameObject.SetActive(false);
		gemEffectAnim.gameObject.SetActive(false);
		if (id == 14)
		{
			ballPrefab.SetActive(true);
		}
		else
		{
			ballPrefab.SetActive(false);
		}
		curCount = 0;
	}

	public void UpdateData()
	{
		if (charData != null)
		{
			SetData(charData.GetID());
		}
	}

	public void SetSelected(bool selected)
	{
		if (charData == null)
		{
			return;
		}
		isSelected = selected;
		infoPrefab.SetActive(false);
		if (anim.gameObject.activeSelf)
		{
			if (selected && User.Instance.GetPlayerLevel(charData.GetID()) >= 1)
			{
				anim.Play();
				infoPrefab.SetActive(true);
			}
			else
			{
				anim.StopAndResetFrame();
			}
		}
	}

	public void OnClick()
	{
		if (charData == null || !GameController.Instance.touchAllowed)
		{
			return;
		}
		if (User.Instance.GetPlayerLevel(charData.GetID()) >= 1)
		{
			int iD = PlayerPanel.Instance.selectedPlayerButton.GetID();
			bool flag = iD == charData.GetID();
			PlayerPanel.Instance.UpdateSelection(charData.GetID(), flag);
			if (flag)
			{
				anim.Stop();
			}
		}
		else
		{
			PlayerPanel.Instance.UpdateSelection(charData.GetID(), true);
		}
	}

	private IEnumerator ShowGem(int count)
	{
		yield return new WaitForSeconds(0.1f);
		float duration = ((count <= 20) ? (0.1f * (float)count) : 2f);
		float delay = duration / (float)(count + 1);
		for (int i = 0; i <= count; i++)
		{
			gemCountText.text = string.Format("+{0}", i);
			yield return new WaitForSeconds(delay);
		}
		Sequence mySequence = DOTween.Sequence();
		mySequence.Append(gemCountText.gameObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.2f));
		mySequence.Append(gemCountText.gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
		gemEffectAnim.gameObject.SetActive(true);
		gemEffectAnim.Play();
		gemEffectAnim.AnimationCompleted = GemEffectCompleted;
	}

	private void GemEffectCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		gemEffectAnim.gameObject.SetActive(false);
	}

	public void ShowGainedGem(int count)
	{
		if (count > 0)
		{
			gemAnim.Play(string.Format("gem{0}Clip", charData.gemId));
			gemAnim.Stop();
			gemAnim.gameObject.SetActive(true);
			gemCountText.gameObject.SetActive(true);
			curCount = count;
			StartCoroutine(ShowGem(count));
		}
	}

	public void ShowAddedGem(int count)
	{
		if (count > 0)
		{
			gemAnim.Play(string.Format("gem{0}Clip", charData.gemId));
			gemAnim.Stop();
			gemAnim.gameObject.SetActive(true);
			gemCountText.gameObject.SetActive(true);
			StartCoroutine(ShowGem(curCount + count));
		}
	}
}
