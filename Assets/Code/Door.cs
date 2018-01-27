using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    #region Serialized fields

    /// <summary>
    /// The collider for the door, impassable.
    /// </summary>
    [SerializeField]
    private Collider2D _doorCollider = null;

    #endregion
    #region Fields

    /// <summary>
    /// The collider for the interaction. If the player is in this collider, he can open or close the door
    /// </summary>
    private Collider2D _interactionCollider = null;

    /// <summary>
    /// Is this door open?
    /// </summary>
    private bool _isOpen = false;

    /// <summary>
    /// Contains a reference to the player ONLY IF the player is present in the interaction area.
    /// </summary>
    private Player _player = null;

    /// <summary>
    /// The sprite renderer for the door.
    /// </summary>
    private SpriteRenderer _spriteRenderer = null;

    #endregion

    #region Life Cycle

    private void Awake()
    {
        // Retrieve the components
        _interactionCollider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

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

        // Toggle the collider and sprite renderer
        _doorCollider.enabled = !_isOpen;
        _spriteRenderer.enabled = !_isOpen;
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
            _player = null;
    }

    #endregion
}