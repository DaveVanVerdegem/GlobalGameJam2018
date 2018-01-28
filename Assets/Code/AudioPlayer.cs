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

	#region Fields
	/// <summary>
	/// List of enemies that are currently chasing the player.
	/// </summary>
	private List<Enemy> _chasingEnemies = new List<Enemy>();
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
		if (chaseMusic && MusicSource.clip != _chaseMusic)
		{
			MusicSource.clip = _chaseMusic;
			MusicSource.Play();
		}
		else if (!chaseMusic && MusicSource.clip != _backgroundMusic)
		{
			MusicSource.clip = _backgroundMusic;
			MusicSource.Play();
		}
	}

	/// <summary>
	/// Track an extra enemy to update the background music.
	/// </summary>
	/// <param name="enemy">Enemy to track.</param>
	public void AddChasingEnemy(Enemy enemy)
	{
		if (_chasingEnemies.Contains(enemy))
			return;

		// Add enemy to the list.
		_chasingEnemies.Add(enemy);

		// Update the music.
		UpdateMusic();
	}

	/// <summary>
	/// Remove an enemy to update the background music.
	/// </summary>
	/// <param name="enemy">Enemy to remove.</param>
	public void RemoveChasingEnemy(Enemy enemy)
	{
		if (!_chasingEnemies.Contains(enemy))
			return;

		// Remove enemy from the list.
		_chasingEnemies.Remove(enemy);

		// Update the music.
		UpdateMusic();
	}

	/// <summary>
	/// Check the chasing enemies and update to the right audioclip.
	/// </summary>
	private void UpdateMusic()
	{
		if (_chasingEnemies.Count > 0 && MusicSource.clip != _chaseMusic)
		{
			MusicSource.clip = _chaseMusic;
			MusicSource.Play();
		}
		else if (_chasingEnemies.Count == 0 && MusicSource.clip != _backgroundMusic)
		{
			MusicSource.clip = _backgroundMusic;
			MusicSource.Play();
		}
	}
	#endregion
}