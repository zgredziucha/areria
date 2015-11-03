using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MothMovement : PlayerMovement {

    public float velocityReducer = 3f;

    private void SetDirectedForce(float movementFactor, ref bool isNoneMovement)
    {
        var noneHorizontalMovement = false;
		if (_direction == VectorDirection.Right) 
		{
            SetHorizontalForce(Mathf.Lerp(_velocity.x, Parameters.MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_direction == VectorDirection.Left)
		{
            SetHorizontalForce(Mathf.Lerp(_velocity.x, -1 * Parameters.MaxSpeed, Time.deltaTime * movementFactor));
		}
        else 
        {
            SetHorizontalForce (Mathf.Lerp(_velocity.x, 0, Time.deltaTime * movementFactor));
            noneHorizontalMovement = true;
        }

		if (_direction == VectorDirection.Up)
		{
            SetVerticalForce(Mathf.Lerp(_velocity.y, Parameters.MaxSpeed, Time.deltaTime * movementFactor));
            gravity.ReduceGravity();
		}
		else if (_direction == VectorDirection.Down)
		{
            SetVerticalForce(Mathf.Lerp(_velocity.y, -1 * Parameters.MaxSpeed * velocityReducer, Time.deltaTime * movementFactor));
		}
		else
		{
			SetVerticalForce (Mathf.Lerp(_velocity.y, 0, Time.deltaTime * movementFactor));
            isNoneMovement = noneHorizontalMovement;
		}

        //Debug.Log("x: " + _velocity.x + " y: " + _velocity.y);
	}
	
	void Awake () {
		State = new MovingState ();
		ResetMovement ();
		
		_boxCollider = GetComponent<BoxCollider2D> ();
		_transform = transform.parent;
		_localScale = transform.localScale;
		
		_halfSize = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		CalculateDistanceBetweenRays ();
		_isFacingForward = transform.localScale.x > 0;

        InitializeEnergyValues();
	}
	void LateUpdate () {
		if (_isMovingEnabled) 
		{
            bool isNoneMovement = false;
			if (State.MovingMode == MoveMode.Flying)
			{
                SetDirectedForce(Parameters.SpeedAccelerationInAir, ref isNoneMovement);
				var deltaMovement = _velocity * Time.deltaTime;
				ValidateFly(ref deltaMovement, isNoneMovement);
				Move (deltaMovement);
			} 
			else if (State.MovingMode == MoveMode.Walking)
			{
                SetDirectedForce(Parameters.SpeedAccelerationOnGround, ref isNoneMovement);

				var deltaMovement = _velocity * Time.deltaTime;
				ValidateWalk(ref deltaMovement);
				Move(deltaMovement);
			}
		}
	}

	private void ValidateWalk (ref Vector2 deltaMovement)
	{
		CalculateOrigins ();
		RaycastHit2D raycastHit;

        if (forceMovementVector != Vector2.zero)
        {
            deltaMovement.y += forceMovementVector.y;
            deltaMovement.x += forceMovementVector.x;
            gravity.ResetGravity(false);
        }
        else
        {
            if (deltaMovement.y <= 0)
            {
                gravity.SetGravity(ref deltaMovement, 0, gravity.walkGravity);
            }
        }

        ValidateHorizontalMovement(ref deltaMovement);

        float energy = 0;
        float velocity = 0;
		if (Mathf.Abs (deltaMovement.x) > .001f)
		{
			SetAniamation("walk", true);
            energy = (float)_energyValue[MoveMode.Walking];
            velocity = Mathf.Abs(deltaMovement.x);
		}
		else 
		{
			SetAniamation("idle", true);
            energy = (float)_energyValue[MoveMode.Standing];
            velocity = 1f;
		}

        if (forceMovementVector != Vector2.zero)
        {
            energy = 0;
            velocity = 0;
            forceMovementVector = Vector2.zero;
        }

        OnMoving(energy, velocity);
		
        //Vector2 textMovement = new Vector2 (0, -0.01f);
        //raycastHit = CalculateCollisions (ref textMovement, false);
        raycastHit = CalculateCollisions(ref deltaMovement, false);
		if (!raycastHit || deltaMovement.y > 0)
		{
			State.MovingMode = MoveMode.Flying;
			SetAniamation("fly", true);
		}
		
		//deltaMovement.y = 0;
	}
	private void ValidateFly (ref Vector2 deltaMovement, bool isNoneMovement )
	{
		CalculateOrigins ();
		RaycastHit2D raycastHit;

        if (forceMovementVector != Vector2.zero)
        {
            deltaMovement.y += forceMovementVector.y;
            deltaMovement.x += forceMovementVector.x;
            gravity.ResetGravity(false);
        }
        else
        {
            gravity.SetGravity(ref deltaMovement, 0, gravity.flyGravity);
        }

        ValidateHorizontalMovement(ref deltaMovement);
        ValidateVerticalMovement(ref deltaMovement);

        float energy = 0;
        float velocity = 0;
        if (forceMovementVector != Vector2.zero)
        {
            energy = 0;
            velocity = 0;
            forceMovementVector = Vector2.zero;
        } 
        else if (deltaMovement.y < 0) 
        {
            energy = (float)_energyValue[MoveMode.Falling];
            velocity = 1f;
        }
        else
        {
            energy = (float)_energyValue[MoveMode.Flying];
            velocity = deltaMovement.magnitude;
        }

        if (isNoneMovement)
        {
            SetAniamation("idle", true);
        }
        else
        {
            SetAniamation("fly", true);
        }

        OnMoving(energy, velocity);
        
	}

    private void  ValidateVerticalMovement(ref Vector2 deltaMovement)
    {
        if (deltaMovement.y > .001f)
        {
            var shiftMovement = deltaMovement;
            var raycastHit = CalculateCollisions(ref deltaMovement, false);

            if (deltaMovement.y == 0 && forceMovementVector.y != 0)
            {
                deltaMovement = shiftMovement / 2;
                ValidateVerticalMovement(ref deltaMovement);
            }
        }
        else if (deltaMovement.y < .001f)
        {
            var shiftMovement = deltaMovement;
            var raycastHit = CalculateCollisions(ref deltaMovement, false);

            if (deltaMovement.y == 0 && forceMovementVector.y != 0)
            {
                deltaMovement = shiftMovement / 2;
                ValidateVerticalMovement(ref deltaMovement);
            }
            else if (raycastHit)
            {
                Debug.Log("walk");
                gravity.ResetGravity();
                ResetForces();
                State.MovingMode = MoveMode.Walking;
                SetAniamation("walk", true);
            }
        }
    }

    private void ValidateHorizontalMovement (ref Vector2 deltaMovement)
    {
        if (Mathf.Abs(deltaMovement.x) > .001f)
        {
            var shiftMovement = deltaMovement;
            var  raycastHit = CalculateCollisions(ref deltaMovement, true);

            if (deltaMovement.x == 0 && forceMovementVector.x != 0)
            {
                deltaMovement = shiftMovement / 2;
                ValidateHorizontalMovement(ref deltaMovement);
            }
        }
      
    }

	private void Move (Vector2 deltaMovement) 
	{
		_transform.Translate (deltaMovement, Space.World);
		
		if (Time.deltaTime > 0) 
		{
			_velocity = deltaMovement / Time.deltaTime;
		}
		
        //var signX = Mathf.Sign (_velocity.x);
        //var signY = Mathf.Sign (_velocity.y);
        //_velocity.x = Mathf.Abs(_velocity.x) > Parameters.MaxVelocity.x ? signX * Parameters.MaxVelocity.x : _velocity.x;
        //_velocity.y = Mathf.Abs(_velocity.y) > Parameters.MaxVelocity.y ? signY * Parameters.MaxVelocity.y : _velocity.y;
		
        //if ( forceMovementVector != Vector2.zero)
        //{
        //    //SetHorizontalForce(forceMovementVector.x);
        //    //SetVerticalForce(forceMovementVector.y);
        //    forceMovementVector = Vector2.zero;
        //    OnMoving (0.0f, 0.0f);
        //}
        //else 
        //{
        //    OnMoving ((float)_energyValue[MoveMode.Walking], deltaMovement.magnitude);
        //}
	}
	
	private void CalculateCollisions (ref Vector2 deltaMovement, bool isHorizontalMovement, ref bool isCollide) {
		Vector2 rayOrigin;
		if (isHorizontalMovement)
		{
			var verticalRayDistance = new Vector2 (0, _verticalDistanceBetweenRays);
			var isMovingRight = deltaMovement.x > 0;
			var verticalRayOrigin = isMovingRight ? bottomRightPoint : bottomLeftPoint;
			var verticalSign = isMovingRight ? 1 : -1;
			
			
			RaycastHit2D raycastHit = CastRays (ref deltaMovement.x, TotalVerticalRays, verticalRayOrigin, verticalRayDistance, isHorizontalMovement, out rayOrigin, sign: verticalSign, angleShift: 0, skipFirst: true);
			if (raycastHit.collider != null)
			{
				deltaMovement.x = 0;
				isCollide = true;
				return;
			}
		}
		else 
		{
			var horizontalRayDistance = new Vector2 (_horizontalDistanceBetweenRays, 0);
			var isMovingUp = deltaMovement.y < 0;
			var horizontalRayOrigin = isMovingUp ?  bottomLeftPoint : topLeftPoint;
			var horizontalSign = isMovingUp ? 1 : -1;
			
			RaycastHit2D raycastHit = CastRays (ref deltaMovement.y, TotalHorizontalRays, horizontalRayOrigin, horizontalRayDistance, isHorizontalMovement, out rayOrigin, sign: horizontalSign, angleShift: 90.0f);
			if (raycastHit.collider != null)
			{
				deltaMovement.y = 0;
				isCollide = true;
				return;
			}
		}
		isCollide = false;
	}
	
}