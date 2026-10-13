using UnityEngine;

public class MeshTest : MonoBehaviour
{
	private void Start()
	{
		Mesh mesh = base.gameObject.transform.GetComponent<MeshFilter>().mesh;
		mesh.uv[0].x = 0.5f;
		mesh.uv[0].y = 0.5f;
		mesh.uv[1].x = 0.7f;
		mesh.uv[1].y = 0.7f;
	}

	private void Update()
	{
	}
}
