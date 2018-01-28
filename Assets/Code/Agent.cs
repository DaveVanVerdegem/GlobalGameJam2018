using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Agent : MonoBehaviour
{
    #region Inspector Fields
    /// <summary>
    /// Speed of this agent.
    /// </summary>
    [Tooltip("Speed of this agent.")]
    [SerializeField]
    private float _speed = 5f;
    #endregion

    #region Properties
    /// <summary>
    /// The agent is facing to the right.
    /// </summary>
    [HideInInspector]
    public bool FacingRight = true;

    /// <summary>
    /// Is this agent the player's agent?
    /// </summary>
    [HideInInspector]
    public bool IsPlayerAgent = false;
    #endregion

    #region Fields
    /// <summary>
    /// The skeleton animation of the agent.
    /// </summary>
    private SkeletonAnimation _skeletonAnimation;
    #endregion

    #region Life Cycle
    // Use this for initialization
    private void Awake()
    {
        // Get the needed components.
        _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
    }

    #endregion

    #region Movement
    /// <summary>
    /// Moves the agent horizontally with the given force.
    /// </summary>
    /// <param name="direction">Force to move agent. The sign will determine the direction.</param>
    public void Move(float direction)
    {
        transform.Translate(Vector2.right * direction * _speed);

        // For the player, only switch sides after a certain threshold is reached
        // otherwise, releasing the joystick is enough to swap around
        // if arrowkeys are pressed instead, ignore the threshold
        bool arrowKeysPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        float tolerance = (IsPlayerAgent && !arrowKeysPressed) ? 0.005f : 0f;
        if (Mathf.Abs(direction) > tolerance)
        {
            // Have the agent face the direction it's moving in.
            FacingRight = (direction > tolerance);
            UpdateViewingDirection();
        }

        // Set the right animation.
        SetAnimation((Mathf.Abs(direction) > 0) ? "walk" : "idle");
    }

    public void UpdateViewingDirection()
    {
        _skeletonAnimation.transform.localScale = (FacingRight) ? Vector3.one : new Vector3(-1f, 1, 1);
    }

    public void SetAnimation(string animationName)
    {
        // Set the right animation.
        _skeletonAnimation.AnimationName = animationName;
    }
    #endregion
}