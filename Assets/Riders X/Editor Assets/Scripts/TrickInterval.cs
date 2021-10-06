using System;
using UnityEngine;

namespace RidersX.Objects
{
    [System.Serializable]
    public struct TrickInterval
    {
        [Range(0.0f, 1.0f)]
        public float ToJumpStrength;

        [Space]
        public Transform Target;
        public float ArcHeight;
        [Tooltip("Ignores an axis of the player/ramp\nFor example:\nX: Horizontal position is not taken into account when calculating the trajectory\nresulting in the player retaining their relative horizontal position to the ramp.\n\nIgnoring every axis will make the trajectory relative to the character.")]
        public Axis IgnoreAxis;
    }

    [Flags]
    public enum Axis
    {
        None = 0,
        X = 1,
        Y = 1 << 1,
        Z = 1 << 2
    }
}