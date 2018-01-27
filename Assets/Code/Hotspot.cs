using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotspot : MonoBehaviour
{
	#region Inspector Fields
	/// <summary>
	/// Range of this wifi hotspot.
	/// </summary>
	[Tooltip("Range of this wifi hotspot.")]
	public float Range = 5f;

	/// <summary>
	/// Strength of the signal depending on the distance from the hotspot.
	/// </summary>
	[Tooltip("Strength of the signal depending on the distance from the hotspot.")]
	[SerializeField]
	private AnimationCurve _signalStrength;

	/// <summary>
	/// Penalty per floor different from the hotspots floor.
	/// </summary>
	[Tooltip("Penalty per floor different from the hotspots floor.")]
	[SerializeField]
	private float _floorPenalty = .4f;
	#endregion

	#region Properties
	/// <summary>
	/// List of all the hotspots in the scene.
	/// </summary>
	public static List<Hotspot> Hotspots = new List<Hotspot>();
	#endregion

	#region Fields
	/// <summary>
	/// Floor that this hotspot is on.
	/// </summary>
	private int _floorLevel;
	#endregion

	#region Life Cycle
	// Use this for initialization
	private void Start()
	{
		// Add the hot spot to the scene list.
		if (!Hotspots.Contains(this))
			Hotspots.Add(this);

		// Set the floor for this hotspot.
		_floorLevel = Level.Instance.ReturnFloorLevel(transform.position);
	}

	// Update is called once per frame
	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, Range);
	}
	#endregion

	#region Returns
	/// <summary>
	/// Returns the data that this hotspot can send to the given player.
	/// </summary>
	/// <param name="player">Player to send data to.</param>
	/// <returns>Returns the data as a float.</returns>
	public float ReturnData(Player player)
	{
		// Get signal strength.
		float relativeDistance = Vector2.Distance(transform.position, player.transform.position);
		float signalStrength = _signalStrength.Evaluate((Range - relativeDistance) / Range);

		int floorDifference = Mathf.Abs(Level.Instance.ReturnFloorLevel(player.transform.position) - _floorLevel);

		// Apply floor penalty to signal strength.
		signalStrength -= floorDifference * _floorPenalty;

		// Clamp signal strength.
		signalStrength = Mathf.Clamp01(signalStrength);

		return signalStrength;
	}
	#endregion

	#region Static Methods
	/// <summary>
	/// Returns all the hotspots in range of the given player.
	/// </summary>
	/// <param name="player">Player to get all the hotspots for.</param>
	/// <returns>Returns al list of hotspots.</returns>
	public static List<Hotspot> ReturnHotspotsInRange(Player player)
	{
		return Hotspots;
	}
	#endregion
}