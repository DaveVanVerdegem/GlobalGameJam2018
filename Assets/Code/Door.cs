using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    #region Serialized fields

    /// <summary>
    /// The gameobject when the door is closed.
    /// </summary>
    [Header("Transition")]
    [SerializeField]
    private GameObject _doorClosed = null;

    /// <summary>
    /// The gameobject when the door is open.
    /// </summary>
    [SerializeField]
    private GameObject _doorOpen = null;

    #endregion

    #region Fields

    /// <summary>
    /// Is this door open?
    /// </summary>
    private bool _isOpen = false;

    /// <summary>
    /// Contains a reference to the player ONLY IF the player is present in the interaction area.
    /// </summary>
    private Player _player = null;

    #endregion

    #region Life Cycle

    private void Update()
    {
        // If there is no player reference, no interaction check is needed
        if (_player == null)
            return;

        // Check if the player presses the interaction key
        if (Input.GetButtonDown("Activate"))
            Interact();
    }

    #endregion

    #region Methods

    public void Interact()
    {
        // Toggle the open
        _isOpen = !_isOpen;

        // If the player opened the door, remove the interactable icon
        _player.ShowInteractableIcon(this, false);

        // Toggle the collider and sprite renderer
        _doorClosed.SetActive(!_isOpen);
        _doorOpen.SetActive(_isOpen);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Set the player reference
        if (other.CompareTag("Player"))
        {
            _player = other.GetComponent<Player>();
            _player.ShowInteractableIcon(this, false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Clear the player reference
        if (other.CompareTag("Player"))
        {
            // Hide the interactable icon
            _player.ShowInteractableIcon(this, false);

            // Clear the player reference
            _player = null;
        }
    }

    #endregion
}