using UnityEngine;

[AddComponentMenu("2D Toolkit/Demo/tk2dDemoRuntimeSpriteController")]
public class tk2dDemoRuntimeSpriteController : MonoBehaviour
{
	public Texture2D runtimeTexture;

	public Texture2D texturePackerTexture;

	public TextAsset texturePackerExportFile;

	public GameObject destroyOnStart;

	private tk2dBaseSprite spriteInstance;

	private tk2dSpriteCollectionData spriteCollectionInstance;

	private void Start()
	{
		if (destroyOnStart != null)
		{
			Object.Destroy(destroyOnStart);
		}
	}

	private void Update()
	{
	}

	private void DestroyData()
	{
		if (spriteInstance != null)
		{
			Object.Destroy(spriteInstance.gameObject);
		}
		if (spriteCollectionInstance != null)
		{
			Object.Destroy(spriteCollectionInstance.gameObject);
		}
	}

	private void DoDemoTexturePacker(tk2dSpriteCollectionSize spriteCollectionSize)
	{
		if (GUILayout.Button("Import"))
		{
			DestroyData();
			spriteCollectionInstance = tk2dSpriteCollectionData.CreateFromTexturePacker(spriteCollectionSize, texturePackerExportFile.text, texturePackerTexture);
			GameObject gameObject = new GameObject("sprite");
			gameObject.transform.localPosition = new Vector3(-1f, 0f, 0f);
			spriteInstance = gameObject.AddComponent<tk2dSprite>();
			spriteInstance.SetSprite(spriteCollectionInstance, "sun");
			gameObject = new GameObject("sprite2");
			gameObject.transform.parent = spriteInstance.transform;
			gameObject.transform.localPosition = new Vector3(2f, 0f, 0f);
			tk2dSprite tk2dSprite2 = gameObject.AddComponent<tk2dSprite>();
			tk2dSprite2.SetSprite(spriteCollectionInstance, "2dtoolkit_logo");
			gameObject = new GameObject("sprite3");
			gameObject.transform.parent = spriteInstance.transform;
			gameObject.transform.localPosition = new Vector3(1f, 1f, 0f);
			tk2dSprite2 = gameObject.AddComponent<tk2dSprite>();
			tk2dSprite2.SetSprite(spriteCollectionInstance, "button_up");
			gameObject = new GameObject("sprite4");
			gameObject.transform.parent = spriteInstance.transform;
			gameObject.transform.localPosition = new Vector3(1f, -1f, 0f);
			tk2dSprite2 = gameObject.AddComponent<tk2dSprite>();
			tk2dSprite2.SetSprite(spriteCollectionInstance, "Rock");
		}
	}

	private void DoDemoRuntimeSpriteCollection(tk2dSpriteCollectionSize spriteCollectionSize)
	{
		if (GUILayout.Button("Use Full Texture"))
		{
			DestroyData();
			Rect region = new Rect(0f, 0f, runtimeTexture.width, runtimeTexture.height);
			GameObject gameObject = tk2dSprite.CreateFromTexture(anchor: new Vector2(region.width / 2f, region.height / 2f), texture: runtimeTexture, size: spriteCollectionSize, region: region);
			spriteInstance = gameObject.GetComponent<tk2dSprite>();
			spriteCollectionInstance = spriteInstance.Collection;
		}
		if (GUILayout.Button("Extract Region)"))
		{
			DestroyData();
			Rect region2 = new Rect(79f, 243f, 215f, 200f);
			GameObject gameObject2 = tk2dSprite.CreateFromTexture(anchor: new Vector2(region2.width / 2f, region2.height / 2f), texture: runtimeTexture, size: spriteCollectionSize, region: region2);
			spriteInstance = gameObject2.GetComponent<tk2dSprite>();
			spriteCollectionInstance = spriteInstance.Collection;
		}
		if (GUILayout.Button("Extract multiple Sprites"))
		{
			DestroyData();
			string[] names = new string[3] { "Extracted region", "Another region", "Full sprite" };
			Rect[] array = new Rect[3]
			{
				new Rect(79f, 243f, 215f, 200f),
				new Rect(256f, 0f, 64f, 64f),
				new Rect(0f, 0f, runtimeTexture.width, runtimeTexture.height)
			};
			Vector2[] anchors = new Vector2[3]
			{
				new Vector2(array[0].width / 2f, array[0].height / 2f),
				new Vector2(0f, array[1].height),
				new Vector2(0f, array[1].height)
			};
			spriteCollectionInstance = tk2dSpriteCollectionData.CreateFromTexture(runtimeTexture, spriteCollectionSize, names, array, anchors);
			GameObject gameObject3 = new GameObject("sprite");
			gameObject3.transform.localPosition = new Vector3(-1f, 0f, 0f);
			spriteInstance = gameObject3.AddComponent<tk2dSprite>();
			spriteInstance.SetSprite(spriteCollectionInstance, 0);
			gameObject3 = new GameObject("sprite2");
			gameObject3.transform.parent = spriteInstance.transform;
			gameObject3.transform.localPosition = new Vector3(2f, 0f, 0f);
			tk2dSprite tk2dSprite2 = gameObject3.AddComponent<tk2dSprite>();
			tk2dSprite2.SetSprite(spriteCollectionInstance, "Another region");
		}
	}

	private void OnGUI()
	{
		tk2dSpriteCollectionSize spriteCollectionSize = tk2dSpriteCollectionSize.Explicit(5f, 640f);
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical("box");
		GUILayout.Label("Runtime Sprite Collection");
		DoDemoRuntimeSpriteCollection(spriteCollectionSize);
		GUILayout.EndVertical();
		GUILayout.BeginVertical("box");
		GUILayout.Label("Texture Packer Import");
		DoDemoTexturePacker(spriteCollectionSize);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}
