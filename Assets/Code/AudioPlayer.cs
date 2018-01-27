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

	#region Inspector Fields
	/// <summary>
	/// Background music for regular gameplay.
	/// </summary>
	[Tooltip("Background music for regular gameplay.")]
	[SerializeField]
	private AudioClip _backgroundMusic;

	/// <summary>
	/// Background music for chase scenes.
	/// </summary>
	[Tooltip("Background music for chase scenes.")]
	[SerializeField]
	private AudioClip _chaseMusic;
	#endregion

	#region Properties
	/// <summary>
	/// The audio source from which background music will be played.
	/// </summary>
	public static AudioSource MusicSource;

	/// <summary>
	/// The audio source from which ingame music will be played.
	/// </summary>
	public static AudioSource IngameMusicSource;

	/// <summary>
	/// The audio source from which the effects will be played.
	/// </summary>
	public static AudioSource EffectsSource;
	#endregion

	#region Life Cycle
	private void Awake()
	{
		IngameMusicSource = GetComponents<AudioSource>()[0];
		EffectsSource = GetComponents<AudioSource>()[1];
		MusicSource = GetComponents<AudioSource>()[2];

		MusicSource.loop = true;
	}
	#endregion

	#region Methods
	/// <summary>
	/// Play background music.
	/// </summary>
	/// <param name="chaseMusic">Play the chase music, else play the regular music.</param>
	public void PlayMusic(bool chaseMusic = false)
	{
		if (chaseMusic)
		{
			MusicSource.clip = _chaseMusic;
			MusicSource.Play();
		}
		else
		{
			MusicSource.clip = _backgroundMusic;
			MusicSource.Play();
		}
	}
	#endregion
}