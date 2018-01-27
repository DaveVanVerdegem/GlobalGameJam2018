using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    #region SerializedFields

    /// <summary>
    /// The distance between two floors
    /// </summary>
    [SerializeField]
    private float _distance = 1f;

    /// <summary>
    /// The time the lift should take to travel 1 floor
    /// </summary>
    [SerializeField]
    private float _travelTime = 2.6f;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _elevatorMusic= null;

    [SerializeField]
    private AudioClip _arrivalSound = null;

    #endregion

    #region Fields

    /// <summary>
    /// Is the lift moving?
    /// </summary>
    private bool _isMoving = false;

    /// <summary>
    /// Contains a reference to the player ONLY IF the player is present.
    /// </summary>
    private Player _player = null;


    #endregion

    #region Life Cycle


    private void Update()
    {
        // If the player is not in the elevator or the elevator is already moving, ignore all input
        if (_player == null || _isMoving)
            return;

        // Check if the player wants the lift to go up or down
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(Move());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(Move(false));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Set the player reference
        if (other.CompareTag("Player"))
            _player = other.GetComponent<Player>();

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Clear the player reference
        if (other.CompareTag("Player"))
        {
            _player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;   
            _player = null;
        }

    }

    #endregion

    #region Methods

    /// <summary>
    /// Move the lift one floor.
    /// </summary>
    /// <param name="up">Move the lift up if true, down if false.</param>
    private IEnumerator Move(bool up = true)
    {
        // Put the player on the elevator and disable his movement
        _player.Freeze();
        _player.transform.SetParent(transform);
        _player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        // Mark the lift as moving
        _isMoving = true;

        // Play the elevator music
        AudioPlayer.MusicSource.PlayOneShot(_elevatorMusic);

        // Save the initial position
        float initialPosition = transform.position.y;

        // Calculate the destination
        float destination = (up) ? initialPosition + _distance : initialPosition - _distance;

        // Progress of the moving
        float progress = 0f;

        // Rate at which the lift should move
        float rate = 1f / _travelTime;

        // Gradually move the lift
        while (progress < 1f)
        {
            progress += Time.deltaTime * rate;
            float newPosition = Mathf.Lerp(initialPosition, destination, progress);
            transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
            //_player.transform.position = new Vector3(_player.transform.position.x, newPosition, _player.transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        // Moving is done
        // mark the lift as stationary
        _isMoving = false;

        // Play the arrival sound
        AudioPlayer.EffectsSource.PlayOneShot(_arrivalSound);

        // Stop playing the elevator music
        AudioPlayer.MusicSource.Stop();

        // Release the player from the elevator
        _player.Freeze(false);
        _player.transform.SetParent(null);
    }

    #endregion
}