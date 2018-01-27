using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    #region SerializedFields

    [Header("Audio")]
    [SerializeField]
    private ElevatorShaft _elevatorShaft = null;

    /// <summary>
    /// The time the lift should take to travel 1 floor
    /// </summary>
    [SerializeField]
    private float _travelTime = 2.6f;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _elevatorMusic = null;

    [SerializeField]
    private AudioClip _arrivalSound = null;

    #endregion

    #region Properties
    [Header("Limits")]
    /// <summary>
    /// The top floor for this elevator.
    /// </summary>
    [Tooltip("The top floor for this elevator.")]
    public int TopFloor = 1;

    /// <summary>
    /// The bottom floor for this elevator.
    /// </summary>
    [Tooltip("The bottom floor for this elevator.")]
    public int BottomFloor = 0;

    /// <summary>
    /// The floor the elevator is on.
    /// </summary>
    [Tooltip("Set this as the floor the elevator is starting on.")]
    public int Floor = 0;
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

        // Retrieve the vertical axis
        float verticalAxis = Input.GetAxis("Vertical");
        float tolerance = 0.1f;
        if (Math.Abs(Mathf.Abs(verticalAxis)) < tolerance)
            return;

        // Check if the player wants the lift to go up or down
        StartCoroutine((verticalAxis > 0) ? Move(true, true) : Move(false, true));
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
    public IEnumerator Move(bool up, bool movePlayer)
    {
        // Avoid exceeding limits
        if (up && Floor == TopFloor)
            yield break;

        if (!up && Floor == BottomFloor)
            yield break;

        // Mark the lift as moving
        _isMoving = true;

        // Put the player on the elevator and disable his movement
        if (movePlayer)
        {
            _player.Freeze();
            _player.transform.SetParent(transform);
            _player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            // Play the elevator music
            AudioPlayer.MusicSource.PlayOneShot(_elevatorMusic);
        }

        // Save the initial position
        float initialPosition = transform.position.y;

        // Calculate the destination
        float destination = (up) ? initialPosition + Level.Instance.FloorHeight : initialPosition - Level.Instance.FloorHeight;

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
            yield return new WaitForEndOfFrame();
        }

        // Moving is done
        // mark the lift as stationary
        _isMoving = false;

        // Increment the elevator floor
        Floor = (up) ? Floor + 1 : Floor - 1;

        // Play the arrival sound
        AudioPlayer.EffectsSource.PlayOneShot(_arrivalSound);

        // Update the doors
        _elevatorShaft.UpdateDoorColliders();

        if (movePlayer)
        {
            // Stop playing the elevator music
            AudioPlayer.MusicSource.Stop();

            // Release the player from the elevator
            _player.Freeze(false);
            _player.transform.SetParent(null);

            // Increment the player floor
            _player.Floor = (up) ? _player.Floor + 1 : _player.Floor - 1;
        }
    }

    #endregion
}