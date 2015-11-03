using System;
using UnityEngine;
using System.Collections;


public enum GravityState
{
    Walking,
    Flying,
}

[Serializable]
public class Gravity {

    private Vector2 gravityMovement = Vector2.zero;

    public float walkGravity;
    public float flyGravity;
    public float gravityMultiplier;
    public float gravityReducer;

    public float minGravityValue = 0.09f;
    public float maxGravityValue = 0.15f;

    private GravityState State = GravityState.Walking;
    private bool _isFalling = false;

    public void SetGravity(ref Vector2 deltaMovement, float selfRotation, float gravityValue)
    {
       
        if (_isFalling)
        {
            SetLinearGravity(ref deltaMovement);
        }
        else
        {
            SetConstantGravity(ref deltaMovement, selfRotation, gravityValue);
        }
        //if (Mathf.Abs(gravityMovement.y) > 0.7f ) 
        //{
        //    Debug.Log(gravityMovement.y);
        //}
        
    }

    private void SetLinearGravity(ref Vector2 deltaMovement)
    {
        gravityMovement.y += flyGravity * Time.deltaTime * gravityMultiplier;
        deltaMovement += gravityMovement; 
    }

    public void ResetGravity(bool tryTakeDamage = true)
    {
        var gravity = Mathf.Abs(gravityMovement.y);
        
        if (tryTakeDamage && gravity > minGravityValue) 
        {
           // Debug.Log(gravity);

            var energy = ((gravity - minGravityValue) / (maxGravityValue - minGravityValue)) * 100.0f;
            Debug.Log("ENERGY " + energy);
            //LevelManager.Instance.TakeDamage(energy);
           // LevelManager.Instance.OnPlayerHit(gravity, maxGravityValue);
        }
        gravityMovement = Vector2.zero;
        _isFalling = false;
       
    }

    public void ReduceGravity()
    {
        gravityMovement.y -= gravityMovement.y * gravityReducer;
    }

    public float GetLinearGravity()
    {
        return gravityMovement.y;
    }


    private void SetConstantGravity(ref Vector2 deltaMovement, float selfRotation, float gravityValue)
    {
        var isInAir = gravityValue == flyGravity;

        if (isInAir)
        {
            gravityMovement.y = gravityValue * Time.deltaTime;
            gravityMovement.x = 0;
            _isFalling = true;
        }
        else
        {
            var angle = Utils.Round90(selfRotation + 90);
            gravityMovement.y = Utils.Sin(angle) * gravityValue * Time.deltaTime;
            gravityMovement.x = Utils.Cos(angle) * gravityValue * Time.deltaTime;
            _isFalling = false;
        }
        deltaMovement += gravityMovement;
    } 
}
