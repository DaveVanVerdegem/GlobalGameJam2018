using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Enums
    /// <summary>
    /// States that this enemy can be in.
    /// </summary>
    private enum State
    {
        Idling,
        Resting,
        Chasing
    }
    #endregion

    #region Inspector Fields
    /// <summary>
    /// Range at which enemy can detect player.
    /// </summary>
    [Tooltip("Range at which enemy can detect player.")]
    [SerializeField]
    private float _detectionRange = 5f;

    /// <summary>
    /// Layer mask that enemy can detect player on.
    /// </summary>
    [Tooltip("Layer mask that enemy can detect player on.")]
    [SerializeField]
    private LayerMask _detectionMask;

    /// <summary>
    /// Range at which enemy catches the player.
    /// </summary>
    [Tooltip("Range at which enemy catches the player.")]
    [SerializeField]
    private float _range = 1f;

    /// <summary>
    /// Time that the enemy waits before starting a new idle movement.
    /// </summary>
    [Tooltip("Time that the enemy waits before starting a new idle movement.")]
    [SerializeField]
    private float _waitingTime = 3f;
    #endregion

    #region Fields
    /// <summary>
    /// Attached agent component.
    /// </summary>
    private Agent _agent;

    /// <summary>
    /// Current state of this enemy.
    /// </summary>
    private State _state = State.Idling;

    /// <summary>
    /// Target for idle movement for this enemy.
    /// </summary>
    private Vector2 _idleTarget = Vector2.zero;

    /// <summary>
    /// Time the enemy has waited in its idle.
    /// </summary>
    private float _idleTimer;

    /// <summary>
    /// The spriterenderer for the angry icons.
    /// </summary>
    private SpriteRenderer _angrySpriteRenderer = null;
    #endregion

    #region Life Cycle
    // Use this for initialization
    private void Start()
    {
        // Get the needed components.
        _agent = GetComponent<Agent>();
        _angrySpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Set the timer high enough so the enemies immediately get a target assigned in the beginning
        _idleTimer = _waitingTime + 1f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.Instance.GameOver)
            return;

        DetectPlayer();

        // Check if the angry sprite should be displayed
        _angrySpriteRenderer.enabled = (_state == State.Chasing);

        // Check state specific behavior
        switch (_state)
        {
            default:
            case State.Idling:
                Idle();

                break;

            case State.Chasing:
                FollowPlayer();

                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (_state != State.Idling)
            return;

        // Draw path to idle target.
        Gizmos.color = Color.magenta;

        Gizmos.DrawCube(_idleTarget, Vector3.one * .1f);
        Gizmos.DrawLine(transform.position, _idleTarget);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Method to try to detect the player.
    /// </summary>
    private void DetectPlayer()
    {
        // Check if the player is whistling
        if (Player.Instance.Whistling)
        {
            // If the player whistles, face the player
            _agent.FacingRight = (Player.Instance.transform.position.x > transform.position.x);
            _agent.UpdateViewingDirection();
        }

        // Check if player is hidden.
        if (Player.Instance.Hidden)
        {
            AudioPlayer.Instance.RemoveChasingEnemy(this);
            _state = State.Idling;
            return;
        }

        // Check if player is in range.
        if (Vector2.Distance(transform.position, Player.Instance.transform.position) > _detectionRange)
        {
            AudioPlayer.Instance.RemoveChasingEnemy(this);
            _state = State.Idling;
            return;
        }

        // Check if enemy is facing the player
        if (_agent.FacingRight != (Player.Instance.transform.position.x > transform.position.x))
        {
            AudioPlayer.Instance.RemoveChasingEnemy(this);
            _state = State.Idling;
            return;
        }

        Vector2 lookDirection = Player.Instance.transform.position - transform.position;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, lookDirection, _detectionRange, _detectionMask);

        // Update player detection.
        if (raycastHit.transform.CompareTag("Enemy"))
        {
            // If another enemy is in range, mark this target for refreshing next frame
            _idleTarget = Vector2.zero;
        }
        if (raycastHit.transform.CompareTag("Player"))
        {
            AudioPlayer.Instance.AddChasingEnemy(this);
            _state = State.Chasing;
        }
        else
        {
            AudioPlayer.Instance.RemoveChasingEnemy(this);
            _state = State.Idling;
        }
    }

    /// <summary>
    /// Follow the player if he's been detected.
    /// </summary>
    private void FollowPlayer()
    {
        // Ignore the player if he is dashing
        if (Player.Instance.Dashing)
            return;

        // Move towards the player
        if (Player.Instance.transform.position.x > transform.position.x)
        {
            _agent.Move(1 * Time.deltaTime);
        }
        else
        {
            _agent.Move(-1 * Time.deltaTime);
        }

        // Check if the enemy can catch the player.
        if (Vector2.Distance(transform.position, Player.Instance.transform.position) < _range)
            GameManager.Instance.TriggerGameOver();

        // Set the next target idle position to be the current location of the Player
        // this is to avoid a lured enemy going back
        _idleTarget = new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y);
    }

    /// <summary>
    /// Idle movement of enemy.
    /// </summary>
    private void Idle()
    {
        // Get new idle target.
        if (_idleTarget == Vector2.zero || Vector2.Distance(transform.position, _idleTarget) < _range)
        {
            if (_idleTimer >= _waitingTime)
            {
                // Reset the timer.
                _idleTimer = 0;
            }
            else
            {
                // Wait.
                _idleTimer += Time.deltaTime;
                _agent.Move(0);
                return;
            }

            // Check how much to the left the enemy can move.
            RaycastHit2D raycastInfo = Physics2D.Raycast(transform.position, Vector2.left, 100, _detectionMask);
            Vector2 leftLimit = raycastInfo.transform.position + Vector3.right;

            // Check how much to the right the enemy can move.
            raycastInfo = Physics2D.Raycast(transform.position, Vector2.right, 100, _detectionMask);
            Vector2 rightLimit = raycastInfo.transform.position + Vector3.left;
            float randomFloat = Random.Range(0f, 1f);

            // Get new idle target.
            _idleTarget = Vector2.Lerp(leftLimit, rightLimit, randomFloat);
        }

        // Move to idle target.
        bool moveRight = _idleTarget.x > transform.position.x;
        if (moveRight)
        {
            _agent.Move(.25f * Time.deltaTime);
        }
        else
        {
            _agent.Move(-.25f * Time.deltaTime);
        }
    }
    #endregion
}