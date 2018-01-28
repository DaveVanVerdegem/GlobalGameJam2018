using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DashTrail : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Amount of "Ghosts" in the trail.
    /// </summary>
    [SerializeField]
    private int _initialPoolSize = 5;

    /// <summary>
    /// The time a ghost should be displayed
    /// </summary>
    [SerializeField]
    private float _displayTime = 0.2f;

    /// <summary>
    /// The delay between 2 ghosts
    /// </summary>
    [SerializeField]
    private float _spawnDelay = 0.1f;

    /// <summary>
    /// The offset for the sprite according to the gameobject
    /// </summary>
    [SerializeField]
    private Vector3 _spriteOffset = new Vector3(0, -0.6f, 0);

    /// <summary>
    /// The prefab that will form the trail.
    /// </summary>
    [SerializeField]
    private GameObject _ghostPrefab = null;

    #endregion

    #region Fields

    private List<GameObject> _trailObjectPool = new List<GameObject>();

    /// <summary>
    /// Is the trail active.
    /// </summary>
    private bool _active = false;

    /// <summary>
    /// Timer to spawn
    /// </summary>
    private float _timer = 0f;

    /// <summary>
    /// A reference to the player's agent
    /// </summary>
    private Agent _playerAgent = null;
    #endregion

    #region Properties
    /// <summary>
    /// Sound to play when dashing.
    /// </summary>
    public AudioClip DashSound = null;

    /// <summary>
    /// Sound to play when dashing is denied.
    /// </summary>
    public AudioClip DeniedSound = null;
    #endregion

    #region Life Cycle
    private void Awake()
    {
        // Fill the pool
        for (int i = 0; i < _initialPoolSize; ++i)
            _trailObjectPool.Add(Instantiate(_ghostPrefab, transform.position + _spriteOffset, transform.rotation));

        // Start with all objects inactive
        _trailObjectPool.ForEach(x => x.SetActive(false));
    }

    private void Update()
    {
        if (!_active)
            return;

        // If the timer has reached the delay
        if (_timer >= _spawnDelay)
        {
            // Spawn a new ghost
            StartCoroutine(SpawnGhost());

            // Reset the timer
            _timer = 0f;
        }

        // Increment the timer
        _timer += Time.deltaTime;
    }

    #endregion

    #region Methods

    public void Activate(Agent playerAgent)
    {
        // Update the player reference
        _playerAgent = playerAgent;

        // Activate the trail
        _active = true;
    }

    public void Deactivate()
    {
        _active = false;
    }
    #endregion

    #region Coroutines

    /// <summary>
    /// Takes an object from the pool and places it at the players current position.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnGhost()
    {
        // Find an inactive ghost
        GameObject ghost = _trailObjectPool.FirstOrDefault(x => x.activeSelf == false);

        // If none can be found, add a new one to the pool
        if (ghost == default(GameObject))
        {
            ghost = Instantiate(_ghostPrefab, transform.position + _spriteOffset, transform.rotation);
            _trailObjectPool.Add(ghost);
        }

        // Activate the ghost
        ghost.SetActive(true);

        // Make the ghost transform from the player transform and look in the right direction
        ghost.transform.position = transform.position + _spriteOffset;
        ghost.transform.rotation = transform.rotation;
        ghost.GetComponent<SpriteRenderer>().flipX = _playerAgent.FacingRight;

        // Disable the ghost after the displaytime has passed
        yield return new WaitForSeconds(_displayTime);
        ghost.SetActive(false);
    }

    #endregion
}