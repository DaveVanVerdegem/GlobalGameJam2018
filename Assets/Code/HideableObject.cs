using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    #region Inspector Fields
    /// <summary>
    /// Visuals for the open state of the object.
    /// </summary>
    [Tooltip("Visuals for the open state of the object.")]
    [SerializeField]
    private GameObject _openVisuals;

    /// <summary>
    /// Visuals for the closed state of the object.
    /// </summary>
    [Tooltip("Visuals for the closed state of the object.")]
    [SerializeField]
    private GameObject _closedVisuals;
    #endregion

    #region Fields

    #endregion

    #region Life Cycle
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Update the player reference
        if (other.CompareTag("Player"))
        {
            Player.Instance.NearbyHideableObject = this;
            Player.Instance.ShowInteractableIcon(this, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Ignore everything but the player
        if (!other.CompareTag("Player"))
            return;

        // Make sure the player is not hidden anymore
        if (Player.Instance.Hidden)
            Player.Instance.Hide(false);

        // Set the object to open
        ToggleState(true);

        // Clear the reference
        // Do a check if this was still the nearby hideable object before clearing the reference.
        // This is in case another object was close and had taken the place already
        if (Player.Instance.NearbyHideableObject == this)
            Player.Instance.NearbyHideableObject = null;

        // Hide interactable icon
        Player.Instance.ShowInteractableIcon(this, false);
    }

    #endregion

    #region Methods
    /// <summary>
    /// Toggle the closed state of this objects visuals.
    /// </summary>
    /// <param name="open">Set this object to open.</param>
    public void ToggleState(bool open)
    {
        if (_openVisuals == null || _closedVisuals == null)
            return;

        _openVisuals.SetActive(open);
        _closedVisuals.SetActive(!open);
    }
    #endregion
}