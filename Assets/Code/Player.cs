﻿using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

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
    /// The icon that is displayed when the player can interact with the environment.
    /// </summary>
    [Tooltip("The icon that is displayed when the player can interact with the environment.")]
    [SerializeField]
    private SpriteRenderer _interactableIcon = null;

    /// <summary>
    /// The amount of total data to download.
    /// </summary>
    [Tooltip("The amount of total data to download.")]
    [SerializeField]
    private float _dataToDownload = 100f;

    /// <summary>
    /// Because of rounding errors a hotspot will never give its total amount of data. Use this margin to prevent data not filling up.
    /// </summary>
    [Tooltip("Because of rounding errors a hotspot will never give its total amount of data. Use this margin to prevent data not filling up.")]
    [SerializeField]
    private float _roundingMargin = 1f;

    /// <summary>
    /// The percentage of how much of the total to download has been downloaded.
    /// </summary>
    [SerializeField]
    private Text _downloadPercentage = null;

    /// <summary>
    /// Layer mask that detects objects in dash range that make dash unavailable
    /// </summary>
    [Header("Dashing")]
    [Tooltip("Layer mask that detects objects in dash range that make dash unavailable.")]
    [SerializeField]
    private LayerMask _wallLayerMask;

    /// <summary>
    /// Dash distance.
    /// </summary>
    [SerializeField]
    private float _dashDistance = 3f;

    /// <summary>
    /// Dash speed.
    /// </summary>
    [SerializeField]
    private float _dashSpeed = 10f;

    /// <summary>
    /// The dash image on the UI that shows the player the cooldown time.
    /// </summary>
    [SerializeField]
    private Image _dashImage = null;

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
    [HideInInspector]
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

    /// <summary>
    /// Is the player currently dashing
    /// </summary>
    [HideInInspector]
    public bool Dashing = false;
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
    /// The skeletonanimation component.
    /// </summary>
    private SkeletonAnimation _skeletonAnimation = null;

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
        _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get all the hotspots in range of this player.
        _hotspotsInRange = Hotspot.ReturnHotspotsInRange(this);

        // Handle the inputs.
        Inputs();

        // Visually update the progress
        UpdateProgress();

        // Get the signal strength.
        float signalStrength = (_connectedHotspot == null) ? 0 : _connectedHotspot.ReturnSignalStrength(this);

        // Update the sprite
        UpdateSignalStrengthIcon(signalStrength);

        // Check if the game can be won.
        if (Mathf.Abs(_downloadedData - _dataToDownload) <= _roundingMargin)
        {
            // Set the loaded image to full.
            _downloadedData = _dataToDownload;
            UpdateProgress();

            // Win game.
            GameManager.Instance.TriggerWin();
        }

        if (_connectedHotspot == null)
        {
            // Show the no connection sprite
            UpdateSignalStrengthIcon(0f);
            return;
        }

        // Disconnect if the hotspot is drained.
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

        // Visualize dashing check
        Vector2 lookDirection = (_agent.FacingRight) ? Vector2.right : Vector2.left;
        Debug.DrawRay(transform.position, lookDirection * _dashDistance, Color.cyan);

        if (Input.GetKeyDown(KeyCode.L))
        {
            // Do a raycast to check if dashing is allowed
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, lookDirection, _dashDistance, _wallLayerMask);

            // If the raycast hit something, don't allow dashing
            if (raycastHit.transform != null)
            {
                Debug.LogWarning(string.Format("Can't dash, raycast hit {0}", raycastHit.transform.gameObject.name));
                return;
            }

            StartCoroutine(Dash());
            return;
        }

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

    private IEnumerator Dash()
    {
        // Set the dash flag
        Dashing = true;

        // Slow motion
        Time.timeScale = 0.3f;

        float progress = 0f;

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + ((_agent.FacingRight) ? Vector2.right : Vector2.left) * _dashDistance;

        while (progress < 1f)
        {
            // Increment progress
            progress += Time.deltaTime * _dashSpeed * (1 - progress + 0.1f);

            // Move
            _agent.transform.position = (Vector2.Lerp(startPosition, targetPosition, progress));
            yield return new WaitForEndOfFrame();
        }

        // Stop slow motion
        Time.timeScale = 1f;

        // Reset the dash flag
        Dashing = false;
        //_agent.Move(/*Input.GetAxis("Horizontal") * */Time.deltaTime * dashForce);
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
    /// <param name="reset">Reset the update progress.</param>
    private void UpdateProgress(bool reset = false)
    {
        float progress = (reset) ? 0f : (_downloadedData / _dataToDownload);
        _downloadMaterial.SetFloat("_Progress", progress);
        _downloadPercentage.text = string.Format("Downloading: {0}%", (int)(progress * 100));
    }

    /// <summary>
    /// Hide the player ingame.
    /// </summary>
    /// <param name="hide">Hide or show the player.</param>
    public void Hide(bool hide)
    {
        // Update the state.
        Hidden = hide;

        // Change the player skin accordingly.
        string targetSkin = (hide) ? "Player_Hidden" : "Player";
        _skeletonAnimation.skeleton.SetSkin(targetSkin);
    }

    /// <summary>
    /// Update the signal strength visuals.
    /// </summary>
    /// <param name="signalStrength">Signal strength to set icon to.</param>
    public void UpdateSignalStrengthIcon(float signalStrength)
    {
        Phone.Instance.SignalStrengthIcon.UpdateSprite(signalStrength);
    }

    /// <summary>
    /// Show the interactable icon.
    /// </summary>
    /// <param name="show">Show/hide the icon.</param>
    public void ShowInteractableIcon(bool show = true)
    {
        _interactableIcon.enabled = show;
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