using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimations : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// The time between two sprites.
    /// </summary>
    [SerializeField]
    private float _interval = 0.1f;

    [SerializeField]
    private List<Sprite> _sprites = null;
    #endregion

    #region Fields

    /// <summary>
    /// The sprite renderer.
    /// </summary>
    private SpriteRenderer _spriteRenderer = null;
    #endregion

    #region Life Cycle
    // Use this for initialization
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Start the infinite animation
        StartCoroutine(AnimateSprite());
    }

    #endregion

    #region Coroutines

    private IEnumerator AnimateSprite()
    {
        int currentIndex = 0;

        // Infinite loop
        for (;;)
        {
            // Don't update if the sprite renderer is not enabled
            if (!_spriteRenderer.enabled)
                yield return new WaitForEndOfFrame();

            // Increment the index
            currentIndex = (currentIndex + 1) % _sprites.Count;

            // Display the sprite
            _spriteRenderer.sprite = _sprites[currentIndex];

            // Wait for the interval
            yield return new WaitForSeconds(_interval);
        }
    }
    #endregion
}