using UnityEngine;

public class Button : MonoBehaviour
{
	private static Texture2D _staticRectTexture;

	private static GUIStyle _staticRectStyle;

	private Rect position;

	private void Start()
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		position = new Rect(vector.x, vector.y, 10f, 10f);
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (_staticRectTexture == null)
		{
			_staticRectTexture = new Texture2D(1, 1);
		}
		if (_staticRectStyle == null)
		{
			_staticRectStyle = new GUIStyle();
		}
		_staticRectTexture.SetPixel(0, 0, Color.black);
		_staticRectTexture.Apply();
		_staticRectStyle.normal.background = _staticRectTexture;
		GUI.Box(position, GUIContent.none, _staticRectStyle);
	}
}
