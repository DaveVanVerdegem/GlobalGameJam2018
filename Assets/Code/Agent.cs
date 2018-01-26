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
	private float _speed = 10f;
	#endregion

	#region Fields
	///// <summary>
	///// Rigid bosy component of this agent.
	///// </summary>
	//private Rigidbody2D _rigidBody;
	#endregion

	#region Life Cycle
	// Use this for initialization
	private void Awake()
	{
		// Get components.
		//_rigidBody = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	private void Update()
	{
		//Debug.Log(string.Format("This agent is on floor {0}", Level.Instance.ReturnFloorLevel(transform.position)));
	}
	#endregion

	#region Movement
	/// <summary>
	/// Moves the agent horizontally with the given force.
	/// </summary>
	/// <param name="direction">Force to move agent. The sign will determine the direction.</param>
	public void Move(float direction)
	{
		//_rigidBody.AddForce(Vector2.right * direction * _speed);
		transform.Translate(Vector2.right * direction * _speed);
	}
	#endregion
}