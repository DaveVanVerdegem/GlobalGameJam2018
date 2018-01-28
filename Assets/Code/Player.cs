using System.Collections;
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
    private LayerMask _dashObstaclesLayerMask;

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

    /// <summary>
    /// Override dash cooldown for debugging purposes.
    /// </summary>
    [SerializeField]
    private bool _noDashCooldown = false;

    #endregion

    #region Static Properties
    /// <summary>
    /// Reference to this player object.
    /// </summary>
    private static Player _instance;

    public static Player Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<Player>()); }
        set { _instance = value; }
    }
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

    /// <summary>
    /// Dash is unavailable during cooldown.
    /// </summary>
    private bool _dashAvailable = true;

    /// <summary>
    /// A reference to keep track of the last object that ordered to show the interactable icon.
    /// This is to make sure that no object that has not sent the last order to show, can hide it.
    /// This would happen e.g. when 2 hideable objects are close.
    /// </summary>
    private object _lastInteractableSender = null;

    /// <summary>
    /// Dashtrail object
    /// </summary>
    private DashTrail _dashTrail = null;

    #endregion

    #region Life Cycle
    // Use this for initialization
    private void Awake()
    {
        // Get the needed components.
        _agent = GetComponent<Agent>();
        _dashTrail = GetComponentInChildren<DashTrail>();
        _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        // Set floor.
        Floor = Level.Instance.ReturnFloorLevel(transform.position);
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

        // Make sure player doesnt end up completely against a wall
        float bleed = 0.5f;

        // Visualize dashing check
        Vector2 lookDirection = (_agent.FacingRight) ? Vector2.right : Vector2.left;
        Debug.DrawRay(transform.position, lookDirection * (_dashDistance + bleed), Color.cyan);

        // Press X button on xbox or Q on pc
        if (_dashAvailable && Input.GetButtonDown("Fire3"))
        {
            // Do a raycast to check if dashing is allowed
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, lookDirection, _dashDistance + bleed, _dashObstaclesLayerMask);

            // If the raycast hit something, don't allow dashing
            if (raycastHit.transform != null)
            {
                AudioPlayer.EffectsSource.PlayOneShot(_dashTrail.DeniedSound);
                Debug.LogWarning(string.Format("Can't dash, raycast hit {0}", raycastHit.transform.gameObject.name));
                return;
            }

            // Dash!
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
            }
            // Disconnect from hotspot.
            else
            {
                _connectedHotspot = null;
            }
        }

        // Hideable controls.
        if (Input.GetButtonDown("Activate") && NearbyHideableObject != null)
        {
            Hide(!Hidden);
            NearbyHideableObject.ToggleState(!Hidden);
        }
    }

    private IEnumerator Dash()
    {
        // Set the dash flag
        Dashing = true;

        // Disable dashing until cooldown is done.
        _dashAvailable = false;

        // Enable the dashtrail
        _dashTrail.Activate(_agent);

        // Play the dashing sound
        AudioPlayer.EffectsSource.PlayOneShot(_dashTrail.DashSound);

        // Slow motion
        Time.timeScale = 0.3f;

        float progress = 0f;

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + ((_agent.FacingRight) ? Vector2.right : Vector2.left) * _dashDistance;

        while (progress < 1f)
        {
            // Increment progress
            progress += Time.deltaTime * _dashSpeed * (1 - progress + 0.1f);

            // Decrement cooldown image (just for visuals)
            _dashImage.fillAmount = 1 - progress;

            // Move
            _agent.transform.position = (Vector2.Lerp(startPosition, targetPosition, progress));
            yield return new WaitForEndOfFrame();
        }

        // Stop slow motion
        Time.timeScale = 1f;

        // Reset the dash flag
        Dashing = false;

        // Deactivate the dash trail
        _dashTrail.Deactivate();

        // Let the ability cool down
        yield return StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown(float coolDownTime = 10f)
    {
        // Check for developer override
        if (_noDashCooldown)
            coolDownTime = 1f;

        // Cool down
        float originalCooldownTime = coolDownTime;
        while (coolDownTime > 0.0f)
        {
            // Decrement time
            coolDownTime -= Time.deltaTime;

            // Calculate progress
            float timePassed = originalCooldownTime - coolDownTime;
            float progress = timePassed / originalCooldownTime;

            // Update visual
            _dashImage.fillAmount = progress;

            // Wait for end of frame
            yield return new WaitForEndOfFrame();
        }

        // Re-enable dashing
        _dashAvailable = true;
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
    public void ShowInteractableIcon(object sender, bool show)
    {
        // If this is a hide request, check if it is valid
        if (!show && sender != _lastInteractableSender)
            return;

        // Remember the last sender who requested
        _lastInteractableSender = sender;

        // Toggle the icon
        _interactableIcon.enabled = show;
    }

    public void SetAnimation(string animationName)
    {
        _agent.SetAnimation(animationName);
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