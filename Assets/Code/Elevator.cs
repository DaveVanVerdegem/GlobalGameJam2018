﻿using System.Collections;
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
    private AudioClip _elevatorMusic = null;

    [SerializeField]
    private AudioClip _arrivalSound = null;

    [SerializeField]
    private ElevatorShaft _elevatorShaft = null;

    #endregion

    #region Properties
    /// <summary>
    /// The floor the elevator is on.
    /// </summary>
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

        // Check if the player wants the lift to go up or down
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(Move(true, true));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(Move(false, true));
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
    public IEnumerator Move(bool up, bool movePlayer)
    {
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