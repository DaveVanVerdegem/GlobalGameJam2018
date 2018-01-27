using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Agent : MonoBehaviour
{
	#region Inspector Fields
	/// <summary>
	/// Speed of this agent.
	/// </summary>
	[Tooltip("Speed of this agent.")]
	[SerializeField]
	private float _speed = 5f;
	#endregion

	#region Properties
	/// <summary>
	/// The agent is facing to the right.
	/// </summary>
	[HideInInspector]
	public bool FacingRight = true;
	#endregion

	#region Fields
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
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	// Update is called once per frame
	private void Update()
	{
	}
	#endregion

	#region Movement
	/// <summary>
	/// Moves the agent horizontally with the given force.
	/// </summary>
	/// <param name="direction">Force to move agent. The sign will determine the direction.</param>
	public void Move(float direction)
	{
		transform.Translate(Vector2.right * direction * _speed);

		// Have the agent face the direction it's moving in.
		FacingRight = (direction > 0);
		_spriteRenderer.flipX = !FacingRight;
	}
	#endregion
}