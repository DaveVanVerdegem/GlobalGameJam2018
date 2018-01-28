using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Phone : MonoBehaviour
{
	#region Inspector Fields
	/// <summary>
	/// The icon that displays the signal strength.
	/// </summary>
	[Tooltip("The icon that displays the signal strength.")]
	public SignalStrengthIcon SignalStrengthIcon = null;

	/// <summary>
	/// The dash image on the UI that shows the player the cooldown time.
	/// </summary>
	[Tooltip("The dash image on the UI that shows the player the cooldown time.")]
	public Image DashImage = null;

	/// <summary>
	/// The percentage of how much of the total to download has been downloaded.
	/// </summary>
	[Tooltip("The percentage of how much of the total to download has been downloaded.")]
	public Text DownloadPercentageText = null;
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
			Destroy(this);
	}

	// Update is called once per frame
	void Update()
	{
	}
	#endregion
}