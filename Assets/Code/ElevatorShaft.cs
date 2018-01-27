using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorShaft : MonoBehaviour
{
    #region Serialized Fields
    /// <summary>
    /// The elevator for this shaft.
    /// </summary>
    [SerializeField]
    private Elevator _elevator = null;

    /// <summary>
    /// The text that displays the information for calling the elevator.
    /// </summary>
    [SerializeField]
    private Text _elevatorCallingInfo = null;

    [SerializeField]
    private List<Collider2D> _floorElevatorColliders = null;

    #endregion

    #region Fields

    /// <summary>
    /// This only contains a reference when the player is INSIDE one of the interaction areas.
    /// </summary>
    private Player _player;
    #endregion

    #region Life Cycle
    // Use this for initialization
    private void Start()
    {
        // Make sure the doors where the elevator currently are are open at start
        UpdateDoorColliders();
    }

    // Update is called once per frame
    private void Update()
    {
        // Ignore input if the player is not in the interaction zone
        if (_player == null)
            return;

        // Call the elevator if the activate button is pressed
        if (Input.GetButtonDown("Activate"))
            StartCoroutine(CallElevator());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore everything but the player
        if (!other.CompareTag("Player"))
            return;

        // Set the player reference
        _player = other.GetComponent<Player>();

        // Display the information if the elevator is not at the current floor
        if (_elevator.Floor != _player.Floor)
            _elevatorCallingInfo.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Ignore everything but the player
        if (!other.CompareTag("Player"))
            return;

        // Reset the player reference
        _player = null;

        // Hide the information
        _elevatorCallingInfo.enabled = false;
    }

    #endregion

    #region Methods

    private IEnumerator CallElevator()
    {
        // The difference between the floor the elevator is called from and the elevators current floor
        int floorDifference = _elevator.Floor - _player.Floor;
        for (int i = 0; i < Mathf.Abs(floorDifference); ++i)
        {
            // If the floor difference is negative, the elevator should travel upwards
            if (floorDifference < 1)
            {
                yield return StartCoroutine(_elevator.Move(true, false));
            }
            else // If the floor difference is positive, the elevator should travel downwards
            {
                yield return StartCoroutine(_elevator.Move(false, false));
            }
        }

        // Hide the information
        _elevatorCallingInfo.enabled = false;
    }

    /// <summary>
    /// Update the door colliders. The elevator should only be accessible for the player on the floor where the elevator is currently on.
    /// </summary>
    public void UpdateDoorColliders()
    {
        for (int i = 0; i < _floorElevatorColliders.Count; ++i)
            _floorElevatorColliders[i].enabled = (i != _elevator.Floor);
    }
    #endregion
}