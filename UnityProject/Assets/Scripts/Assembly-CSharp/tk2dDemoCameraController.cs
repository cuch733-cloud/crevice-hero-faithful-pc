using System.Collections;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Demo/tk2dDemoCameraController")]
public class tk2dDemoCameraController : MonoBehaviour
{
	public Transform listItems;

	public Transform endOfListItems;

	private Vector3 listTopPos = Vector3.zero;

	private Vector3 listBottomPos = Vector3.zero;

	private bool listAtTop = true;

	private bool transitioning;

	public Transform[] rotatingObjects = new Transform[0];

	private void Start()
	{
		listTopPos = listItems.localPosition;
		listBottomPos = listTopPos - endOfListItems.localPosition;
	}

	private IEnumerator MoveListTo(Vector3 from, Vector3 to)
	{
		transitioning = true;
		float time = 0.5f;
		for (float t = 0f; t < time; t += Time.deltaTime)
		{
			float nt = Mathf.Clamp01(t / time);
			nt = Mathf.SmoothStep(0f, 1f, nt);
			listItems.localPosition = Vector3.Lerp(from, to, nt);
			yield return 0;
		}
		listItems.localPosition = to;
		transitioning = false;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !transitioning && !Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)))
		{
			if (listAtTop)
			{
				StartCoroutine(MoveListTo(listTopPos, listBottomPos));
			}
			else
			{
				StartCoroutine(MoveListTo(listBottomPos, listTopPos));
			}
			listAtTop = !listAtTop;
		}
		Transform[] array = rotatingObjects;
		foreach (Transform transform in array)
		{
			transform.Rotate(Random.insideUnitSphere, Time.deltaTime * 360f);
		}
	}
}
