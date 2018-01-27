using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	#region Inspector Fields
	/// <summary>
	/// Main menu panel.
	/// </summary>
	[Tooltip("Main menu panel.")]
	[SerializeField]
	private GameObject _mainMenuPanel;

	/// <summary>
	/// Help panel.
	/// </summary>
	[Tooltip("Help panel.")]
	[SerializeField]
	private GameObject _helpPanel;

	/// <summary>
	/// Credits panel.
	/// </summary>
	[Tooltip("Credits panel.")]
	[SerializeField]
	private GameObject _creditsPanel;
	#endregion

	#region Life Cycle
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
	#endregion

	#region Methods
	/// <summary>
	/// Loads the given scene.
	/// </summary>
	/// <param name="scene">Scene to load.</param>
	public void LoadScene(string scene)
	{
		SceneManager.LoadScene(scene);
	}

	/// <summary>
	/// Exit the game.
	/// </summary>
	public void ExitGame()
	{
		Application.Quit();
	}

	/// <summary>
	/// Shows the main menu panel.
	/// </summary>
	public void ShowMainMenu()
	{
		// Hide all the other panels.
		_creditsPanel.SetActive(false);
		_helpPanel.SetActive(false);

		// Show main menu.
		_mainMenuPanel.SetActive(true);
	}

	/// <summary>
	/// Shows the help panel.
	/// </summary>
	public void ShowHelp()
	{
		// Hide all the other panels.
		_creditsPanel.SetActive(false);
		_mainMenuPanel.SetActive(false);

		// Show help.
		_helpPanel.SetActive(true);
	}

	/// <summary>
	/// Shows the credits panel.
	/// </summary>
	public void ShowCredits()
	{
		// Hide all the other panels.
		_mainMenuPanel.SetActive(false);
		_helpPanel.SetActive(false);

		// Show credits.
		_creditsPanel.SetActive(true);
	}
	#endregion
}