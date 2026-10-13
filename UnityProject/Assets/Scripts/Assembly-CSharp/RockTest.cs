using UnityEngine;

public class RockTest : MonoBehaviour
{
	public float width;

	public float height;

	private void Start()
	{
		Mesh mesh = base.gameObject.transform.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3((0f - width) / 2f, (0f - height) / 2f, 0f),
			new Vector3(width / 2f, height / 2f, 0f),
			new Vector3(width / 2f, (0f - height) / 2f, 0f),
			new Vector3((0f - width) / 2f, height / 2f, 0f)
		};
		mesh.vertices = vertices;
		float num = 100f;
		float x = num * width / 1280f;
		float y = num * height / 800f;
		float num2 = num * base.transform.position.x / 1280f;
		float x2 = num2;
		float y2 = num * base.transform.position.y / 800f;
		base.renderer.material.mainTextureOffset = new Vector2(x2, y2);
		base.renderer.material.mainTextureScale = new Vector2(x, y);
		base.renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
		mesh.RecalculateBounds();
	}

	private void Update()
	{
	}
}
