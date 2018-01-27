using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
	private float _catchRange = 1f;
	#endregion

	#region Fields
	/// <summary>
	/// Attached agent component.
	/// </summary>
	private Agent _agent;

	/// <summary>
	/// Player detected by this enemy.
	/// </summary>
	private bool _playerDetected;
	#endregion

	#region Life Cycle
	// Use this for initialization
	private void Start()
	{
		// Get the needed components.
		_agent = GetComponent<Agent>();
	}

	// Update is called once per frame
	private void Update()
	{
		if (GameManager.Instance.GameOver)
			return;

		DetectPlayer();
		FollowPlayer();
	}
	#endregion

	#region Methods
	/// <summary>
	/// Method to try to detect the player.
	/// </summary>
	private void DetectPlayer()
	{
		// Check if player is in range.
		if (Vector2.Distance(transform.position, Player.Instance.transform.position) > _detectionRange)
		{
			_playerDetected = false;
			return;
		}

		// Check if enemy is facing the player.
		if (_agent.FacingRight != (Player.Instance.transform.position.x > transform.position.x))
		{
			_playerDetected = false;
			return;
		}

		Vector2 lookDirection = Player.Instance.transform.position - transform.position;
		RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, lookDirection, _detectionRange, _detectionMask);

		// Update player detection.
		_playerDetected = (raycastHit.transform == Player.Instance.transform);
	}

	/// <summary>
	/// Follow the player if he's been detected.
	/// </summary>
	private void FollowPlayer()
	{
		// Only continue if the player has been detected.
		if (!_playerDetected)
			return;

		if (Player.Instance.transform.position.x > transform.position.x)

			_agent.Move(1 * Time.deltaTime);
		else
			_agent.Move(-1 * Time.deltaTime);

		// Check if the enemy can catch the player.
		if (Vector2.Distance(transform.position, Player.Instance.transform.position) < _catchRange)
		{
			GameManager.Instance.TriggerGameOver();
		}
	}
	#endregion
}