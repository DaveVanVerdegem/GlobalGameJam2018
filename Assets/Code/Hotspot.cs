using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
	/// Penalty per floor different from the hotspots floor.
	/// </summary>
	[Tooltip("Penalty per floor different from the hotspots floor.")]
	[SerializeField]
	private float _floorPenalty = .4f;

	[Space]
	/// <summary>
	/// Strength of the signal depending on the distance from the hotspot.
	/// </summary>
	[Tooltip("Strength of the signal depending on the distance from the hotspot.")]
	[SerializeField]
	private AnimationCurve _signalStrength = null;

	/// <summary>
	/// Bandwidth for this hotspot. Defines how fast this hotspot sends data.
	/// </summary>
	[Tooltip("Bandwidth for this hotspot. Defines how fast this hotspot sends data.")]
	[SerializeField]
	private float _bandwidth = .1f;

	/// <summary>
	/// Data still remaining on this hotspot.
	/// </summary>
	[Tooltip("Data still remaining on this hotspot.")]
	[SerializeField]
	private float _availableData = 1f;

	/// <summary>
	/// The time in seconds that it takes to complete one pulse.
	/// </summary>
	[SerializeField]
	private float _signalPulseSpeed = 3.0f;

	[Header("Colors")]
	/// <summary>
	/// The color when the bandwidth is at the lowest.
	/// </summary>
	[SerializeField]
	private Color _minimumBandwidthColor = Color.red;

	/// <summary>
	/// The color when the bandwidth is at the highest.
	/// </summary>
	[SerializeField]
	private Color _maximumBandwidthColor = Color.green;
	#endregion

	#region Static Properties
	/// <summary>
	/// List of all the hotspots in the scene.
	/// </summary>
	public static List<Hotspot> Hotspots = new List<Hotspot>();
	#endregion

	#region Properties
	/// <summary>
	/// True when all the available data on this hotspot has been used.
	/// </summary>
	[HideInInspector]
	public bool Drained = false;
	#endregion

	/// <summary>
	/// The ratio for scale of the sprite renderer object when the sprite has reached the boundaries.
	/// </summary>
	[SerializeField]
	private float _maxScaleRatio = 17.3f / 5f;
	#region Fields
	/// <summary>
	/// Floor that this hotspot is on.
	/// </summary>
	private int _floorLevel;
	/// <summary>
	/// The sprite renderers of this object.
	/// </summary>
	private List<SpriteRenderer> _spriteRenderers = null;

	#endregion

	#region Life Cycle

	private void Awake()
	{
		// Add the hot spot to the scene list.
		if (!Hotspots.Contains(this))
			Hotspots.Add(this);

		_spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
	}
	// Use this for initialization
	private void Start()
	{
		// Set the floor for this hotspot.
		_floorLevel = Level.Instance.ReturnFloorLevel(transform.position);

		// Visualize the pulses
		for (int i = 0; i < _spriteRenderers.Count; ++i)
			StartCoroutine(PulseSignal(_spriteRenderers[i], 0.3f * i));

		// Determine the color according to the bandwidth and update the spriterenderers
		if (_bandwidth < 0 || _bandwidth > 1f)
			Debug.LogWarning(string.Format("Bandwidth should be normalised! ({0})", _bandwidth));

		Color bandwithColor = Color.Lerp(_minimumBandwidthColor, _maximumBandwidthColor, _bandwidth);
		_spriteRenderers.ForEach(x => x.color = bandwithColor);
	}

	// Update is called once per frame
	private void Update()
	{
	}

	private void OnDisable()
	{
		// Clear the static list.
		Hotspots.Clear();
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
		// Retrieve the signal strength
		float signalStrength = ReturnSignalStrength(player);

		// Calculate the transmitted data.
		float bandwidthScale = 0.1f;
		float transmittedData = signalStrength * (_bandwidth * bandwidthScale);

		// Make sure that the transmitted data doesn't exceed the available data.
		if (transmittedData > _availableData)
			transmittedData = _availableData;

		// Remove the data from the available data.
		_availableData -= transmittedData;

		// Update the state of this hotspot.
		Drained = (_availableData <= 0);

		return transmittedData;
	}

	/// <summary>
	/// Returns the signal strength of this hotspot to the given player.
	/// </summary>
	/// <param name="player">Player to get signal strength for.</param>
	/// <returns>Returns the signal strength as a float.</returns>
	public float ReturnSignalStrength(Player player)
	{
		// Get signal strength.
		float distance = Vector2.Distance(transform.position, player.transform.position);

		// Return no signal if the hotspot is out of range.
		if (distance > Range)
			return 0;

		float signalStrength = _signalStrength.Evaluate((Range - distance) / Range);

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
		List<Hotspot> hotspotsInRange = new List<Hotspot>(Hotspots);

		// Remove all the hotspots that are too far from the player.
		hotspotsInRange.RemoveAll(hotspot => Vector2.Distance(player.transform.position, hotspot.transform.position) > hotspot.Range);

		return hotspotsInRange;
	}
	#endregion

	#region Coroutines

	private IEnumerator PulseSignal(SpriteRenderer targetRenderer, float progressOffset)
	{
		float progress = progressOffset;
		float rate = 1f / _signalPulseSpeed;
		float targetScale = Range * _maxScaleRatio;

		// Infinite loop to keep pulsing
		for (; ; )
		{
			while (progress < 1.0f)
			{
				progress += Time.deltaTime * rate;

				// Update the scale of the spriterenderer
				float scale = Mathf.Lerp(0, targetScale, progress);
				targetRenderer.transform.localScale = new Vector3(scale, scale, scale);

				// Fade the opacity
				float opacity = Mathf.Lerp(1f, 0f, progress);
				targetRenderer.color = new Color(targetRenderer.color.r, targetRenderer.color.g, targetRenderer.color.b, opacity);

				yield return new WaitForEndOfFrame();
			}

			// Once finished, reset the progress
			progress = 0f;
		}
	}
	#endregion
}