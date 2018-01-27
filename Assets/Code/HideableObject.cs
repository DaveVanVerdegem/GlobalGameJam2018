using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The renderer of this object.
    /// </summary>
    private Renderer _renderer = null;

    /// <summary>
    /// Contains a reference to the player ONLY IF the player is in the interactable area.
    /// </summary>
    private Player _player = null;
    #endregion

    #region Life Cycle
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Ignore input if player is not present in the interactable area
        if (_player == null)
            return;

        // Toggle the renderer if the activate key is pressed
        if (Input.GetButtonDown("Activate"))
            ToggleHide();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Update the player reference
        if (other.CompareTag("Player"))
            _player = other.GetComponent<Player>();
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear the player reference
        if (other.CompareTag("Player"))
            _player = null;
    }

    #endregion

    #region Methods

    private void ToggleHide()
    {
        _renderer.enabled = !_renderer.enabled;
    }
    #endregion
}