using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalStrengthIcon : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// List of the signal strength sprites.
    /// </summary>
    [SerializeField]
    private List<Sprite> _sprites = null;
    #endregion

    #region Fields
    /// <summary>
    /// The image on the UI.
    /// </summary>
    private Image _signalStrengthImage = null;
    #endregion

    #region Life Cycle

    private void Awake()
    {
        _signalStrengthImage = GetComponent<Image>();
    }
    #endregion

    #region Methods

    public void UpdateSprite(float signalStrength)
    {
        // TEMP TEMP because implementation for the curve is currently perfectly in 3 slices
        if (Math.Abs(signalStrength) < 0.01f)
        {
            _signalStrengthImage.sprite = _sprites[0];
        }
        else if (signalStrength < .33f)
        {
            _signalStrengthImage.sprite = _sprites[1];
        }
        else if (signalStrength < 0.66f)
        {
            _signalStrengthImage.sprite = _sprites[2];
        }
        else
        {
            _signalStrengthImage.sprite = _sprites[3];
        }
    }
    #endregion
}