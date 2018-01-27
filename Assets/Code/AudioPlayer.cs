using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	#region Singleton

	private static AudioPlayer _instance;

	public static AudioPlayer Instance
	{
		get { return _instance ?? (_instance = FindObjectOfType<AudioPlayer>()); }
		set { _instance = value; }
	}

	#endregion
	#region Fields

	private AudioSource _audioSource;
	#endregion

	#region Life Cycle

	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	#endregion

	#region Methods

	/// <summary>
	/// Plays the given sound clip
	/// </summary>
	public void Play(AudioClip clip)
	{
		_audioSource.PlayOneShot(clip);
	}

	#endregion
}