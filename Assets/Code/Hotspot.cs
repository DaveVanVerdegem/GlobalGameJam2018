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
    private AnimationCurve _signalStrength = null;

    /// <summary>
    /// Bandwidth for this hotspot. Defines how fast this hotspot sends data.
    /// </summary>
    [Tooltip("Bandwidth for this hotspot. Defines how fast this hotspot sends data.")]
    [SerializeField]
    private float _bandwidth = .1f;

    /// <summary>
    /// Penalty per floor different from the hotspots floor.
    /// </summary>
    [Tooltip("Penalty per floor different from the hotspots floor.")]
    [SerializeField]
    private float _floorPenalty = .4f;
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

    #region Serialized Fields
    /// <summary>
    /// Data still remaining on this hotspot.
    /// </summary>
    [SerializeField]
    private float _availableData = 1f;
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
        float signalStrength = ReturnSignalStrength(player);

        // Calculate the transmitted data.
        float transmittedData = signalStrength * _bandwidth;

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
}