using UnityEngine;

public class DistanceIndicator : MonoBehaviour
{
	public GameObject textObject;

	public int type;

	private tk2dTextMesh distanceText;

	private void Awake()
	{
		distanceText = textObject.GetComponent<tk2dTextMesh>();
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}

	public void SetPosition(int value, float x, float y)
	{
		base.transform.position = new Vector3(x, y, base.transform.position.z);
		distanceText.text = string.Format("{0}", value);
	}
}
