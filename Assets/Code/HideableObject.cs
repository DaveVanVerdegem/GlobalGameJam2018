using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
	#region Fields

	#endregion

	#region Life Cycle
	// Update is called once per frame
	private void Update()
	{
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		// Update the player reference
		if (other.CompareTag("Player"))
			Player.Instance.NearbyHideableObject = this;
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		// Ignore everything but the player
		if (!other.CompareTag("Player"))
			return;

		// Make sure the player is not hidden anymore
		Player.Instance.Hide(false);

		// Clear the reference
		Player.Instance.NearbyHideableObject = null;
	}

	#endregion

	#region Methods
	#endregion
}