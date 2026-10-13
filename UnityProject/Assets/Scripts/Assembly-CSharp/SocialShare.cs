using UnityEngine;

public class SocialShare : MonoBehaviour
{
	public GameObject distanceObj;

	public GameObject shotSpriteRenderer;

	public GameObject menuButtonObject;

	public static SocialShare Instance;

	private Sprite shotSprite;

	private void Awake()
	{
		Instance = this;
		Util.d("SocialShare awake");
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		tk2dTextMesh component = distanceObj.GetComponent<tk2dTextMesh>();
		component.text = string.Format("{0}m", GameController.Instance.GetDistance());
		menuButtonObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("ui_menu"));
		if (GameController.Instance.screenShot != null)
		{
			SpriteRenderer component2 = shotSpriteRenderer.GetComponent<SpriteRenderer>();
			GameController.Instance.screenShot.Apply();
			shotSprite = Sprite.Create(GameController.Instance.screenShot, new Rect(0f, 0f, GameController.Instance.screenShot.width, GameController.Instance.screenShot.height), new Vector2(0.5f, 0.5f));
			component2.sprite = shotSprite;
			float num = 324f / (float)GameController.Instance.screenShot.width;
			component2.transform.localScale = new Vector3(num, num, 1f);
		}
	}

	public void OnFBClick()
	{
		if (GameController.Instance.screenShot != null)
		{
			string message = string.Format("Crevice Hero run {0}m! http://iamagame.com/crevice #crevicehero", GameController.Instance.GetDistance());
			SPShareUtility.FacebookShare(message, GameController.Instance.screenShot);
			Util.LogEvent("share_fb");
			Util.d("FacebookShare");
		}
	}

	public void OnTwitterClick()
	{
		if (GameController.Instance.screenShot != null)
		{
			string status = string.Format("Crevice Hero run {0}m! http://iamagame.com/crevice #crevicehero", GameController.Instance.GetDistance());
			SPShareUtility.TwitterShare(status, GameController.Instance.screenShot);
			Util.LogEvent("share_twitter");
			Util.d("TwitterShare");
		}
	}

	public void OnResumeClick()
	{
		Map.Instance.Fade(false, () =>
		{
		});
		base.gameObject.SetActive(false);
		GameController.Instance.ShowResult(true);
	}
}
