using UnityEngine;

[AddComponentMenu("2D Toolkit/Demo/tk2dDemoButtonController")]
public class tk2dDemoButtonController : MonoBehaviour
{
	private float spinSpeed;

	private void Update()
	{
		base.transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
	}

	private void SpinLeft()
	{
		spinSpeed = 4f;
	}

	private void SpinRight()
	{
		spinSpeed = -4f;
	}

	private void StopSpinning()
	{
		spinSpeed = 0f;
	}
}
