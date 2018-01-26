using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftMovement : MonoBehaviour
{
    #region SerializedFields

    /// <summary>
    /// The distance between two floors
    /// </summary>
    [SerializeField]
    private float _distance = 5.0f;

    /// <summary>
    /// The time the lift should take to travel 1 floor
    /// </summary>
    [SerializeField]
    private float _travelTime = 5.0f;

    [SerializeField]
    private AudioClip _arrivalSound = null;

    #endregion

    #region Fields

    /// <summary>
    /// Is the lift moving?
    /// </summary>
    private bool _isMoving = false;

    private AudioSource _audioSource;

    #endregion

    #region Life Cycle

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // If the lift is moving, ignore all input
        if (_isMoving)
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

    #endregion

    #region Methods

    /// <summary>
    /// Move the lift one floor.
    /// </summary>
    /// <param name="up">Move the lift up if true, down if false.</param>
    private IEnumerator Move(bool up = true)
    {
        // Mark the lift as moving
        _isMoving = true;

        // Save the initial position
        float initialPosition = transform.position.y;

        // Calculate the destination
        float destination = (up) ? initialPosition + _distance : initialPosition - _distance;

        // Time in seconds to complete the growth process
        float time = Random.Range(1f, _travelTime);

        // Progress of the moving
        float progress = 0f;

        // Rate at which the lift should move
        float rate = 1f / time;

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

        // Play the arrival sound
        AudioPlayer.Instance.Play(_arrivalSound);
    }

    #endregion
}