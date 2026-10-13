using UnityEngine;

public class Popup : MonoBehaviour
{
	public delegate void OnYesClick();

	public delegate void OnNoClick();

	public GameObject yesObject;

	public GameObject noObject;

	public tk2dTextMesh messageTextObject;

	public OnYesClick onYesClick;

	public OnNoClick onNoClick;

	private tk2dTextMesh messageText;

	private bool isOpen;

	private void Start()
	{
	}

	public bool IsOpen()
	{
		return isOpen;
	}

	public void ShowPopup(string messageID, OnYesClick cYes, OnNoClick cNo)
	{
		yesObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("ui_yes"));
		noObject.GetComponent<tk2dSprite>().SetSprite(Lang.Instance.GetResource("ui_no"));
		onYesClick = cYes;
		onNoClick = cNo;
		base.gameObject.SetActive(true);
		if (messageText == null)
		{
			messageText = messageTextObject.GetComponent<tk2dTextMesh>();
		}
		messageText.text = Lang.Instance.Get(messageID);
		isOpen = true;
	}

	public void ClosePopup()
	{
		base.gameObject.SetActive(false);
		isOpen = false;
	}

	private void YesClicked()
	{
		onYesClick();
	}

	public void NoClicked()
	{
		if (onNoClick == null)
		{
			ClosePopup();
		}
		else
		{
			onNoClick();
		}
	}
}
