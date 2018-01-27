using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Contains a reference to the player renderer ONLY IF the player is in the interactable area.
    /// </summary>
    private Renderer _playerRenderer = null;

    #endregion

    #region Life Cycle

    // Update is called once per frame
    private void Update()
    {
        // Ignore input if player is not present in the interactable area
        if (_playerRenderer == null)
            return;

        // Toggle the renderer if the activate key is pressed
        if (Input.GetButtonDown("Activate"))
            ToggleHide();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Update the player reference
        if (other.CompareTag("Player"))
            _playerRenderer = other.GetComponentInChildren<Renderer>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Ignore everything but the player
        if (!other.CompareTag("Player"))
            return;

        // Make sure the player is not hidden anymore
        _playerRenderer.enabled = true;

        // Clear the reference
        _playerRenderer = null;
    }

    #endregion

    #region Methods

    private void ToggleHide()
    {
        _playerRenderer.enabled = !_playerRenderer.enabled;
    }
    #endregion
}