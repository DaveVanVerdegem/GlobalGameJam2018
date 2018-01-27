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
	#endregion

	#region Fields
	/// <summary>
	/// Player detected by this enemy.
	/// </summary>
	private Player _detectedPlayer;
	#endregion

	#region Life Cycle
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		DetectPlayer();
	}
	#endregion

	#region Methods
	private void DetectPlayer()
	{
		// Check if player is in range.
		if (Vector2.Distance(transform.position, Player.Instance.transform.position) > _detectionRange)
			return;

		// Check if enemy is facing the player.

		Vector2 lookDirection = Player.Instance.transform.position - transform.position;
		bool playerVisible = Physics2D.Raycast(transform.position, lookDirection, _detectionRange);

		if (playerVisible)
			Debug.Log("seeing you!");
	}
	#endregion
}