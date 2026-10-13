using UnityEngine;

public class tk2dTileMapDemoPlayer : MonoBehaviour
{
	public tk2dTextMesh textMesh;

	public tk2dTextMesh textMeshLabel;

	private Vector3 textMeshOffset;

	private bool textInitialized;

	public float addForceLimit = 1f;

	public float amount = 500f;

	public float torque = 50f;

	private tk2dSprite sprite;

	private int score;

	private float forceWait;

	private float moveX;

	private bool AllowAddForce
	{
		get
		{
			return forceWait < 0f;
		}
	}

	private void Awake()
	{
		sprite = GetComponent<tk2dSprite>();
		if (textMesh == null || textMesh.transform.parent != base.transform)
		{
			Debug.LogError("Text mesh must be assigned and parented to player.");
			base.enabled = false;
		}
		textMeshOffset = textMesh.transform.position - base.transform.position;
		textMesh.transform.parent = null;
		textMeshLabel.text = "instructions";
		textMeshLabel.Commit();
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer)
		{
			textMesh.text = "LEFT ARROW / RIGHT ARROW";
		}
		else
		{
			textMesh.text = "TAP LEFT / RIGHT SIDE OF SCREEN";
		}
		textMesh.Commit();
		Application.targetFrameRate = 60;
	}

	private void Update()
	{
		forceWait -= Time.deltaTime;
		string text = ((!AllowAddForce) ? "player_disabled" : "player");
		if (sprite.CurrentSprite.name != text)
		{
			sprite.SetSprite(text);
		}
		if (AllowAddForce)
		{
			float num = 0f;
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				num = 1f;
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				num = -1f;
			}
			for (int i = 0; i < Input.touchCount; i++)
			{
				if (Input.touches[i].phase == TouchPhase.Began)
				{
					num = Mathf.Sign(Input.touches[i].position.x - (float)Screen.width * 0.5f);
					break;
				}
			}
			if (num != 0f)
			{
				if (!textInitialized)
				{
					textMeshLabel.text = "score";
					textMeshLabel.Commit();
					textMesh.text = score.ToString();
					textMesh.Commit();
					textInitialized = true;
				}
				moveX = num;
			}
		}
		textMesh.transform.position = base.transform.position + textMeshOffset;
	}

	private void FixedUpdate()
	{
		if (AllowAddForce && moveX != 0f)
		{
			forceWait = addForceLimit;
			Rigidbody component = GetComponent<Rigidbody>();
			Rigidbody2D component2 = GetComponent<Rigidbody2D>();
			if (component != null)
			{
				component.AddForce(new Vector3(moveX * amount, amount, 0f) * Time.deltaTime, ForceMode.Impulse);
				component.AddTorque(new Vector3(0f, 0f, (0f - moveX) * torque) * Time.deltaTime, ForceMode.Impulse);
			}
			else if (component2 != null)
			{
				component2.AddForce(new Vector2(moveX * amount, amount) * Time.deltaTime * 50f);
				component2.AddTorque((0f - moveX) * torque * Time.deltaTime * 20f);
			}
			moveX = 0f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Object.Destroy(other.gameObject);
		score++;
		textMesh.text = score.ToString();
		textMesh.Commit();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Object.Destroy(other.gameObject);
		score++;
		textMesh.text = score.ToString();
		textMesh.Commit();
	}
}
