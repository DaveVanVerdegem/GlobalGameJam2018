using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
public class Player : MonoBehaviour
{
	#region Fields
	/// <summary>
	/// Attached agent component.
	/// </summary>
	private Agent _agent;

	/// <summary>
	/// Lists of the hotspots in range of this player.
	/// </summary>
	private List<Hotspot> _hotspotsInRange = new List<Hotspot>();
	#endregion

	#region Life Cycle
	// Use this for initialization
	private void Awake()
	{
		// Get the needed components.
		_agent = GetComponent<Agent>();
	}

	// Update is called once per frame
	private void Update()
	{
		// Handle the inputs.
		Inputs();

		// Get all the hotspots in range of this player.
		_hotspotsInRange = Hotspot.ReturnHotspotsInRange(this);

		if (_hotspotsInRange.Count > 0)
			// Get data from first hot spot.
			Debug.Log(string.Format("Got {0} of data from hotspot.", _hotspotsInRange[0].ReturnData(this)));
	}
	#endregion

	#region Methods
	private void Inputs()
	{
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
		{
			_agent.Move(Input.GetAxis("Horizontal") * Time.deltaTime);
		}
	}
	#endregion
}