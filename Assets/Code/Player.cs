using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class Player : MonoBehaviour
{
    #region Inspector Fields
    /// <summary>
    /// The material for the image to download.
    /// </summary>
    [Tooltip("The material for the image to download.")]
    [SerializeField]
    private Material _downloadMaterial = null;

    /// <summary>
    /// The icon that displays the signal strength.
    /// </summary>
    [Tooltip("The icon that displays the signal strength.")]
    [SerializeField]
    private SignalStrengthIcon _signalStrengthIcon = null;

    #endregion

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

    /// <summary>
    /// Hideable object in range.
    /// </summary>
    [HideInInspector]
    public HideableObject NearbyHideableObject;

    /// <summary>
    /// Player is hidden from the enemy.
    /// </summary>
    [HideInInspector]
    public bool Hidden = false;

    #endregion

    #region Fields
    /// <summary>
    /// Attached agent component.
    /// </summary>
    private Agent _agent;

    /// <summary>
    /// Attached player renderer.
    /// </summary>
    private Renderer _playerRenderer = null;

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

    /// <summary>
    /// Data that the player has already downloaded.
    /// </summary>
    private float _downloadedData;

    /// <summary>
    /// The amount of total data to download.
    /// </summary>
    private float _dataToDownload = 100f;
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
        _playerRenderer = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get all the hotspots in range of this player.
        _hotspotsInRange = Hotspot.ReturnHotspotsInRange(this);

        // Handle the inputs.
        Inputs();

        if (_connectedHotspot == null)
        {
            // Show the no connection sprite
            UpdateSignalStrengthIcon(0f);
            return;
        }

        // Disconnect if the hotspot is drained.
        float signalStrength = _connectedHotspot.ReturnSignalStrength(this);
        if (_connectedHotspot.Drained || signalStrength < 0.001f)
        {
            // Disconnect the hotspot
            _connectedHotspot = null;

            // Show the no connection sprite
            UpdateSignalStrengthIcon(0f);
            return;
        }

        // Get data from the connected hot spot.
        _downloadedData += _connectedHotspot.ReturnData(this);

        // Visually update the progress
        UpdateProgress();

        // Update the sprite
        UpdateSignalStrengthIcon(signalStrength);
    }

    private void OnDisable()
    {
        Instance = null;
    }

    private void OnApplicationQuit()
    {
        // Reset the progress material
        UpdateProgress(true);
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
        _agent.Move(Input.GetAxis("Horizontal") * Time.deltaTime);

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

        // Hideable controls.
        if (Input.GetButtonDown("Activate") && NearbyHideableObject != null)
            Hide(!Hidden);
    }
    /// <summary>
    /// Disable the players input or not.
    /// </summary>
    /// <param name="freeze">Freeze the inputs of the player.</param>
    public void Freeze(bool freeze = true)
    {
        _freeze = freeze;
    }

    /// <summary>
    /// Visually update the progress of downloading the graphic.
    /// </summary>
    /// <param name="reset"></param>
    private void UpdateProgress(bool reset = false)
    {
        float progress = (reset) ? 0f : _downloadedData / _dataToDownload;
        _downloadMaterial.SetFloat("_Progress", progress);
    }

    public void Hide(bool hide)
    {
        // Update the state.
        Hidden = hide;

        // Make the player invisible.
        _playerRenderer.enabled = !hide;
    }

    public void UpdateSignalStrengthIcon(float signalStrength)
    {
        _signalStrengthIcon.UpdateSprite(signalStrength);
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