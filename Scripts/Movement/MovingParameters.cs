using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class MovingParameters
{
    //public Vector2 MaxVelocity = new Vector2 (float.MaxValue, float.MaxValue);

    //public float FlyGravity = -25f;
    //public float WalkGravity = -1f;

    public LayerMask PlatformMask;
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 10f;
    public float MaxSpeed = 8;
}
