using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Inspector Fields
    [Header("GUI")]
    /// <summary>
    /// Pause menu.
    /// </summary>
    [Tooltip("Pause menu.")]
    [SerializeField]
    private GameObject _pauseMenu;

    /// <summary>
    /// Game over panel.
    /// </summary>
    [Tooltip("Game over panel.")]
    [SerializeField]
    private GameObject _gameOverPanel;

    /// <summary>
    /// Win panel.
    /// </summary>
    [Tooltip("Win panel.")]
    [SerializeField]
    private GameObject _winPanel;

    [Space]
    /// <summary>
    /// Audioclip to play with win screen.
    /// </summary>
    [Tooltip("Audioclip to play with win screen.")]
    [SerializeField]
    private AudioClip _winCry;
    #endregion

    #region Properties
    /// <summary>
    /// Instance reference of the game manager.
    /// </summary>
    public static GameManager Instance;

    /// <summary>
    /// The game is over.
    /// </summary>
    [HideInInspector]
    public bool GameOver;
    #endregion

    #region Life Cycle
    // Use this for initialization
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Make sure that the game isn't paused.
        Time.timeScale = 1;
    }

    private void Start()
    {
        // Play the background music.
        AudioPlayer.Instance.PlayMusic();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Pause(!_pauseMenu.activeSelf);
        }
    }
    #endregion

    #region GUI Methods
    /// <summary>
    /// Triggers the game over state of the game.
    /// </summary>
    public void TriggerGameOver()
    {
        // Don't trigger the game over multiple times.
        if (GameOver)
            return;

        // Enable game over state.
        GameOver = true;

        // Pause the game.
        Time.timeScale = 0;

        Debug.Log("Game over triggered.");

        // Show the game over panel.
        _gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Triggers the win state of the game.
    /// </summary>
    public void TriggerWin()
    {
        // Don't trigger the win multiple times.
        if (GameOver)
            return;

        // Enable game over state.
        GameOver = true;

        // Pause the game.
        Time.timeScale = 0;

        Debug.Log("Win triggered.");

        // Show the win panel.
        _winPanel.SetActive(true);

        // Play win sound.
        AudioPlayer.MusicSource.Stop();
        AudioPlayer.EffectsSource.PlayOneShot(_winCry);
    }

    /// <summary>
    /// Restarts the game level.
    /// </summary>
    public void RestartLevel()
    {
        // Unpause the game.
        Time.timeScale = 1;

        Debug.Log("Restarted level.");

        // Reload the active scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Pause or resume the game.
    /// </summary>
    /// <param name="paused">Set true to pause or false to resume the game.</param>
    public void Pause(bool paused)
    {
        // Pause time.
        Time.timeScale = (paused) ? 0 : 1;

        // Set display of pause menu.
        _pauseMenu.SetActive(paused);
    }

    /// <summary>
    /// Loads the given scene.
    /// </summary>
    /// <param name="scene">Scene to load.</param>
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    #endregion
}