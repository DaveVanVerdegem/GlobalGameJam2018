using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class Player : MonoBehaviour
{
    #region Static Properties
    /// <summary>
    /// Reference to this player object.
    /// </summary>
    public static Player Instance;
    #endregion

    #region Properties

    /// <summary>
    /// The floor the player is currently on.
    /// </summary>
    public int Floor = 0;
    #endregion

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
    /// Hotspot that the player is currently connected to.
    /// </summary>
    private Hotspot _connectedHotspot;
    /// <summary>
    /// Is the player frozen aka prohibited to move?
    /// </summary>
    private bool _freeze = false;
    #endregion

    #region Life Cycle
    // Use this for initialization
    private void Awake()
    {
        // Set the instance.
        if (Instance == null)
            Instance = this;

        // Get the needed components.
        _agent = GetComponent<Agent>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get all the hotspots in range of this player.
        _hotspotsInRange = Hotspot.ReturnHotspotsInRange(this);

        // Handle the inputs.
        Inputs();

        if (_connectedHotspot != null)
        {
            // Disconnect if the hotspot is drained.
            if (_connectedHotspot.Drained)
                _connectedHotspot = null;
            else
            {
                // Get data from the connected hot spot.
                // Debug.Log(string.Format("Getting {0} of data.", _connectedHotspot.ReturnData(this)));
            }
        }
    }
    #endregion

    #region Methods
    private void Inputs()
    {
        // Movement controls.
        // Disable inputs when the player is frozen
        if (_freeze)
            return;

		// Movement controls.
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            _agent.Move(Input.GetAxis("Horizontal") * Time.deltaTime);
        }

        // Wifi controls.
        if (Input.GetButtonUp("Connect"))
        {
			// Connect to new hotspot.
			if (_connectedHotspot == null)
			{
				_connectedHotspot = ReturnBestAvailableHotspot();

				if (_connectedHotspot != null)
					Debug.Log(string.Format("Connected to hot spot."));
			}
			// Disconnect from hotspot.
			else
			{
				_connectedHotspot = null;

				Debug.Log(string.Format("Disconnected from hotspot."));
			}
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

    #region Returns
    /// <summary>
    /// Get the strongest hotspot in range of this player.
    /// </summary>
    /// <returns>Returns a hotspot.</returns>
    private Hotspot ReturnBestAvailableHotspot()
    {
        // No hotspots in range.
        if (_hotspotsInRange.Count == 0)
            return null;

        // Get strongest hot spot in range.
        Hotspot strongestHotspot = null;

        for (int i = 0; i < _hotspotsInRange.Count; i++)
        {
            // Skip drained hotspots.
            if (_hotspotsInRange[i].Drained)
                continue;

            if (strongestHotspot == null)
            {
                strongestHotspot = _hotspotsInRange[i];
            }
            else
            {
                if (_hotspotsInRange[i].ReturnSignalStrength(this) > strongestHotspot.ReturnSignalStrength(this))
                    strongestHotspot = _hotspotsInRange[i];
            }
        }

        return strongestHotspot;
    }
    #endregion
}