using UnityEngine;

public class tk2dTileMapDemoFollowCam : MonoBehaviour
{
	private tk2dCamera cam;

	public Transform target;

	public float followSpeed = 1f;

	public float minZoomSpeed = 20f;

	public float maxZoomSpeed = 40f;

	public float maxZoomFactor = 0.6f;

	private void Awake()
	{
		cam = GetComponent<tk2dCamera>();
	}

	private void FixedUpdate()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = Vector3.MoveTowards(position, target.position, followSpeed * Time.deltaTime);
		position2.z = position.z;
		base.transform.position = position2;
		Rigidbody component = target.GetComponent<Rigidbody>();
		if (component != null && cam != null)
		{
			float magnitude = component.velocity.magnitude;
			float t = Mathf.Clamp01((magnitude - minZoomSpeed) / (maxZoomSpeed - minZoomSpeed));
			float num = Mathf.Lerp(1f, maxZoomFactor, t);
			cam.ZoomFactor = Mathf.MoveTowards(cam.ZoomFactor, num, 0.2f * Time.deltaTime);
		}
	}
}
