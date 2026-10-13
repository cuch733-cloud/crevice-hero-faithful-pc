using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip buttonClick;

	public AudioClip jump;

	public AudioClip die;

	public AudioClip shake;

	public AudioClip crash;

	public AudioClip select;

	public AudioClip get;

	public AudioClip skill;

	public AudioClip gift;

	public AudioClip hammer;

	public AudioClip missile;

	public AudioClip soccer;

	public static AudioManager Instance;

	private void Awake()
	{
		Instance = this;
	}
}
