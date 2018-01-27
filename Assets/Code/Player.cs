using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class Player : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Attached agent component.
    /// </summary>
    private Agent _agent;

    /// <summary>
    /// Lists of the hotspots in range of this player.
    /// </summary>
    private List<Hotspot> _hotspotsInRange = new List<Hotspot>();

    /// <summary>
    /// Is the player frozen aka prohibited to move?
    /// </summary>
    private bool _freeze = false;

    /// <summary>
    /// The sprite renderer of the player
    /// </summary>
    private SpriteRenderer _spriteRenderer = null;

    #endregion

    #region Life Cycle

    // Use this for initialization
    private void Awake()
    {
        // Get the needed components.
        _agent = GetComponent<Agent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Handle the inputs.
        Inputs();

        // Get all the hotspots in range of this player.
        _hotspotsInRange = Hotspot.ReturnHotspotsInRange(this);

        if (_hotspotsInRange.Count > 0)
            // Get data from first hot spot.
            Debug.Log(string.Format("Got {0} of data from hotspot.", _hotspotsInRange[0].ReturnData(this)));
    }

    #endregion

    #region Methods

    private void Inputs()
    {
        // Disable inputs when the player is frozen
        if (_freeze)
            return;

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            _agent.Move(Input.GetAxis("Horizontal") * Time.deltaTime);

            // Look in the direction you are walking
            _spriteRenderer.flipX = (Input.GetAxis("Horizontal") <= 0);
        }
    }

    /// <summary>
    /// Disable the players movements or not
    /// </summary>
    /// <param name="freeze"></param>
    public void Freeze(bool freeze = true)
    {
        _freeze = freeze;
    }

    #endregion
}