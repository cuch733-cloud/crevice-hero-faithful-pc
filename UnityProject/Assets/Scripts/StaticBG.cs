using UnityEngine;

public class StaticBG : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.position = new Vector3(Camera.main.transform.position.x, base.transform.position.y, base.transform.position.z);
	}
}
