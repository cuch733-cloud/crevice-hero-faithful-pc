using UnityEngine;

public class BG : MonoBehaviour
{
	public float offset;

	public float speedRate;

	public int order;

	private void Start()
	{
		float num = (float)GameController.SCREEN_WIDTH / 100f;
		float num2 = (float)GameController.SCREEN_HEIGHT / 100f;
		Mesh mesh = base.gameObject.transform.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3((0f - num) / 2f, (0f - num2) / 2f, 0f),
			new Vector3(num / 2f, num2 / 2f, 0f),
			new Vector3(num / 2f, (0f - num2) / 2f, 0f),
			new Vector3((0f - num) / 2f, num2 / 2f, 0f)
		};
		mesh.vertices = vertices;
		LoadTheme(3);
	}

	public void LoadTheme(int themeID)
	{
		base.renderer.material.mainTexture = Resources.Load($"bg_{themeID}_back_0{order}", typeof(Texture2D)) as Texture;
		base.renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
	}

	private void LateUpdate()
	{
		UpdateBG();
	}

	public void UpdateBG()
	{
		base.transform.position = new Vector3(Camera.main.transform.position.x, base.transform.position.y, base.transform.position.z);
		float num = Camera.main.transform.position.x * speedRate + offset;
		base.renderer.material.mainTextureOffset = new Vector2(num * 100f / (float)GameController.SCREEN_WIDTH, 0f);
	}
}
