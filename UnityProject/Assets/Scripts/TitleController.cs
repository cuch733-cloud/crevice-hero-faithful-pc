using UnityEngine;

public class TitleController : MonoBehaviour
{
	private void Awake()
	{
		GameController.bgPanel = GameObject.FindGameObjectsWithTag("BG")[0];
		Object.DontDestroyOnLoad(GameController.bgPanel);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Application.LoadLevel("PlayScene");
		}
	}
}
