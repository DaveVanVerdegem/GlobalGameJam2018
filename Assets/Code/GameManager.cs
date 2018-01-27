using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	#region Inspector Fields
	[Header("GUI")]
	/// <summary>
	/// Game over panel.
	/// </summary>
	[Tooltip("Game over panel.")]
	[SerializeField]
	private GameObject _gameOverPanel;
	#endregion

	#region Properties
	/// <summary>
	/// Instance reference of the game manager.
	/// </summary>
	public static GameManager Instance;

	/// <summary>
	/// The game is over.
	/// </summary>
	public bool GameOver;
	#endregion

	#region Life Cycle
	// Use this for initialization
	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	// Update is called once per frame
	void Update()
	{
	}
	#endregion

	#region GUI Methods
	/// <summary>
	/// Triggers the game over state of the game.
	/// </summary>
	public void TriggerGameOver()
	{
		// Enable game over state.
		GameOver = true;

		// Pause the game.
		Time.timeScale = 0;

		Debug.Log("Game over triggered.");

		// Show the game over panel.
		_gameOverPanel.SetActive(true);
	}

	public void RestartLevel()
	{
		// Unpause the game.
		Time.timeScale = 1;

		Debug.Log("Restarted level.");

		// Reload the active scene.
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	#endregion
}