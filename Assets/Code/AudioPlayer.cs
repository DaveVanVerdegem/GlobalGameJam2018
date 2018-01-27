using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    #region Singleton

    private static AudioPlayer _instance;

    public static AudioPlayer Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<AudioPlayer>()); }
        set { _instance = value; }
    }

    #endregion
    #region Properties

    /// <summary>
    /// The audio source from which the music will be played.
    /// </summary>
    public static AudioSource MusicSource;

    /// <summary>
    /// The audio source from which the effects will be played.
    /// </summary>
    public static AudioSource EffectsSource;
    #endregion

    #region Life Cycle

    private void Awake()
    {
        MusicSource = GetComponents<AudioSource>()[0];
        EffectsSource= GetComponents<AudioSource>()[1];
    }

    #endregion


}