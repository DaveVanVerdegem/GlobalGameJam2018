﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Level : MonoBehaviour
{
	#region Inspector Fields
	/// <summary>
	/// Height of one floor level.
	/// </summary>
	[Tooltip("Height of one floor level.")]
	public float FloorHeight = 2f;
    #endregion

    #region Static Properties

    private static Level _instance;
    /// <summary>
    /// Reference to the ingame level object.
    /// </summary>
    public static Level Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<Level>()); }
        set { _instance = value; }
    }
    #endregion

    #region Life Cycle
    // Use this for initialization
    private void Awake()
	{
	}

	// Update is called once per frame
	private void Update()
	{
	}
	#endregion

	#region Returns
	/// <summary>
	/// Returns back the floor level of the given position.
	/// </summary>
	/// <param name="position">Position to get floor level for.</param>
	/// <returns>Returns the floor level as an int.</returns>
	public int ReturnFloorLevel(Vector2 position)
	{
		return Mathf.FloorToInt(position.y / FloorHeight);
	}
	#endregion
}