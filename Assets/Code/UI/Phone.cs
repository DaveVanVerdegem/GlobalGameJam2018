using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
	#region Inspector Fields
	/// <summary>
	/// The icon that displays the signal strength.
	/// </summary>
	[Tooltip("The icon that displays the signal strength.")]
	public SignalStrengthIcon SignalStrengthIcon = null;
	#endregion

	#region Static Properties
	/// <summary>
	/// Reference to this object.
	/// </summary>
	public static Phone Instance;
	#endregion

	#region Life Cycle
	// Use this for initialization
	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	// Update is called once per frame
	void Update()
	{
	}
	#endregion
}