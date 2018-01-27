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
	void Start()
	{
		// Get the needed components.
		_agent = GetComponent<Agent>();
	}

	// Update is called once per frame
	void Update()
	{
		DetectPlayer();
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
			return;

		// Check if enemy is facing the player.
		if (_agent.FacingRight != (Player.Instance.transform.position.x > transform.position.x))
			return;

		Vector2 lookDirection = Player.Instance.transform.position - transform.position;
		RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, lookDirection, _detectionRange, _detectionMask);

		Debug.Log(string.Format("Seeing {0}.", raycastHit.transform), raycastHit.transform);

		// Update player detection.
		_playerDetected = (raycastHit.transform == Player.Instance.transform);

		if (_playerDetected)
			Debug.Log("seeing you!");
	}
	#endregion
}