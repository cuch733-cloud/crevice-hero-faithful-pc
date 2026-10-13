using UnityEngine;

public class Splash : MonoBehaviour
{
	private void Start()
	{
		LoadScene();
	}

	private void LoadScene()
	{
		Application.LoadLevel("PlayScene");
	}

	private void Update()
	{
	}
}
