using UnityEngine;

public class MapEffect : MonoBehaviour
{
	public GameObject[] objects;

	private float leftLimit;

	private float rightLimit;

	public float objectWidth;

	private float prevCameraX;

	private float xpos;

	private void Start()
	{
		rightLimit = objectWidth;
	}

	public void InitPlay()
	{
		prevCameraX = 2f;
		xpos = 0f;
		base.transform.position = new Vector3(0f, base.transform.position.y, base.transform.position.z);
		base.gameObject.SetActive(true);
	}

	private void Update()
	{
		xpos -= Camera.main.transform.position.x - prevCameraX;
		if ((double)xpos < -12.8)
		{
			xpos += 12.8f;
		}
		base.transform.position = new Vector3(xpos, base.transform.position.y, base.transform.position.z);
		prevCameraX = Camera.main.transform.position.x;
	}
}
