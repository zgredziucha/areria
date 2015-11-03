using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MantisMovement : PlayerMovement {

	private float _jumpIn = 1;

	private Vector2 jumpMovement;

	public bool CanJump 
	{
		get
		{
			return _jumpIn <= 0 && !_inTheAir && State.MovingMode == MoveMode.Walking;
		}
	}
    public bool CanReduceGravity
    {
        get
        {
            return _jumpIn <= 0 && _inTheAir;
        }
    }

	private bool isRotationEnds = false;
	private float _rotated = 0;

	private Vector2 tempJumpShift;
	private bool _inTheAir = false;

    private bool isJumpRotationEnds = false;
    private bool _isRotationPossible = false;

	public override bool IsGrounded () {
		return State.MovingMode == MoveMode.Walking && !_inTheAir;
	}
	
    public float JumpYMagnitude;
    public float JumpXMagnitude;
    public float JumpFrequency;
    public float horizontalMovInAirReducer = 0.2f;

    private float maxHorizontalReduce;
    private float curHorizontalReduce = 0;
    private float HorizontalReduceSpeed = 5;

	void Awake () {
		State = new MovingState ();
        
		ResetMovement ();
		State.MovingMode = MoveMode.Walking;

		_boxCollider = GetComponent<BoxCollider2D> ();
		_transform = transform.parent;
		_localScale = transform.localScale;
		
		_halfSize = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		CalculateDistanceBetweenRays ();
		_isFacingForward = transform.localScale.x > 0;

        maxHorizontalReduce = Parameters.MaxSpeed -  (Parameters.MaxSpeed * horizontalMovInAirReducer);
        InitializeEnergyValues();

	}

	void LateUpdate () {
		_jumpIn -= Time.deltaTime;

		if (_isMovingEnabled) 
		{
			if (State.MovingMode == MoveMode.Snapping) 
			{
				Snap(State.SlopeAngle, MoveMode.Walking);
			} 
			else if (State.MovingMode == MoveMode.Walking)
			{
                SetDirectedForce(Parameters.SpeedAccelerationOnGround);
				var deltaMovement = _velocity * Time.deltaTime;

				ValidateWalk(ref deltaMovement);

				if (State.MovingMode != MoveMode.Snapping) 
				{
					Move(deltaMovement);
				}
			} 
			else if (State.MovingMode == MoveMode.UnSnapping) 
			{
                UnSnap(State.SlopeAngle, MoveMode.Walking);
			} 
		}
	}

    /**
     * Jump &  Unsnapping
     */
    private void UnSnappingParams()
    {
        State.DestinationAngle = 0;
        if (State.DestinationAngle != Utils.Round90(transform.eulerAngles.z))
        {
            ResetForces();
            CalculateUnSnapShift();
            State.SlopeAngle = -1 * transform.eulerAngles.z;

            State.MovingMode = MoveMode.UnSnapping;
        }
        _inTheAir = true;
     
    }

    private void CalculateUnSnapShift()
    {
        float deltaMovement = Mathf.Max(_halfSize.x * 4, _halfSize.y * 4);

        var angle = Utils.Round90(transform.eulerAngles.z - 90);
        var y = -Utils.Sin(angle) * deltaMovement;
        var x = -Utils.Cos(angle) * deltaMovement;

        //State.SlopeShiftMovement = new Vector2(transform.position.x + x, transform.position.y + y);
        State.SlopeShiftMovement = new Vector2(x, y);

    }

    private void UnSnap(float angle, MoveMode newMode)
    {
        var tolerance = 1.5f;
        var distance = 0f;
        var maxDistance = 0.21f;

        var shiftMovement = new Vector2(State.SlopeShiftMovement.x, State.SlopeShiftMovement.y);
        if (shiftMovement != Vector2.zero)
        {
            tempJumpShift = Vector2.Lerp(tempJumpShift, shiftMovement, Time.deltaTime * Parameters.SpeedAccelerationInAir * 2);
            distance = shiftMovement.magnitude;
            if (distance < maxDistance)
            {
                shiftMovement = Vector2.zero;
                tempJumpShift = Vector2.zero;
            }
            else
            {
                //Debug.Log(tempJumpShift.y);
                //Debug.Log(distance);
                shiftMovement -= tempJumpShift;
                _transform.Translate(tempJumpShift, Space.World);
            }
            
            State.SlopeShiftMovement = new Vector3(shiftMovement.x, shiftMovement.y, 0);
        }

        if (distance < tolerance)
        {
            if (!isRotationEnds)
            {
                var _rotationTime = Mathf.Clamp(Mathf.Abs(angle) / 270.0f, 0.1f, 0.3f);
                RotateWhileSnapping(angle, true, _rotationTime);
            }
        }

        if (isRotationEnds && State.SlopeShiftMovement == Vector3.zero)
        {
            State.MovingMode = newMode;
            transform.eulerAngles = new Vector3(0, 0, 0);
            isRotationEnds = false;
        }
    }

    private void Jump()
    {
        var angle = Utils.Round90(transform.eulerAngles.z - 90);
        jumpMovement.y = -Utils.Sin(angle) * JumpYMagnitude;
        jumpMovement.x = -Utils.Cos(angle) * JumpYMagnitude;

        if (_direction == VectorDirection.Right || _direction == VectorDirection.Left)
        {
            angle = Utils.Round90(transform.eulerAngles.z);
            var sign = _isFacingForward ? 1 : -1;
            jumpMovement.y += sign * Utils.Sin(angle) * JumpXMagnitude;
            jumpMovement.x += sign * Utils.Cos(angle) * JumpXMagnitude;
        }

        _jumpIn = JumpFrequency;
        //_horizontalAirTimer = horizontalMovInAirFraq;
        UnSnappingParams();
    }

    private bool IsRotationPossible()
    {
        Vector2 rayOrigin;
        float deltaMovement = Mathf.Abs(jumpMovement.y) > 0 ? jumpMovement.y : jumpMovement.x;
        RaycastHit2D raycastHit = CastRays(ref deltaMovement, TotalHorizontalRays, topLeftPoint, new Vector2(_horizontalDistanceBetweenRays, 0), false, out rayOrigin, sign: -1, angleShift: 90.0f);
        if (raycastHit.collider != null)
        {
            return false;
        }
        return true;
    }

    private void HandleJump(ref Vector2 shiftMovement)
    {
        if (jumpMovement != Vector2.zero)
        {
            tempJumpShift = Vector2.Lerp(tempJumpShift, jumpMovement, Time.deltaTime * Parameters.SpeedAccelerationInAir);

            if (jumpMovement.magnitude < 0.2f)
            {
                jumpMovement = Vector2.zero;
                tempJumpShift = Vector2.zero;
                _inTheAir = true;
            }
            else
            {
                shiftMovement += tempJumpShift;
                jumpMovement -= tempJumpShift;
            }
        }
    }
   
    /**
     * Snapping
     */
    private void Snap(float angle, MoveMode newMode)
    {
        if (State.SlopeShiftMovement == Vector3.zero && isRotationEnds)
        {
            isRotationEnds = false;
            State.MovingMode = newMode;
            return;
        }

        ShiftWhileSnapping();
        if (!isRotationEnds)
        {
            var _rotationTime = Mathf.Clamp(Mathf.Abs(angle) / 180.0f, 0.1f, 0.2f);
            RotateWhileSnapping(angle, true, _rotationTime);
        }
    }

    private void RotateWhileSnapping(float angle, bool isSnapping, float _rotationTime = 0)
    {
        var rotationVector = new Vector3(0, 0, angle);
        _rotated += Mathf.Abs(angle) * (Time.deltaTime / _rotationTime);

        if (Mathf.Abs(angle) > _rotated)
        {
            transform.Rotate(rotationVector * (Time.deltaTime / _rotationTime));

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, State.DestinationAngle);
            _rotated = 0;

            if (isSnapping)
            {
                isRotationEnds = true;
            }
            else
            {
                isJumpRotationEnds = true;
            }
        }
    }

    private void ShiftWhileSnapping()
    {
        var endPosition = State.SlopeShiftMovement;
        if (State.SlopeShiftMovement != Vector3.zero)
        {
            _transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * 9);

            var distance = (transform.position - endPosition).sqrMagnitude;
            var maxDistance = 0.01f;
            if (distance < maxDistance * maxDistance)
            {
                _transform.position = endPosition;
                State.SlopeShiftMovement = Vector3.zero;
            }
        }
    }

    private void SetSnapShift()
    {
        var shift = _halfSize.x - _halfSize.y;
        var shiftVector = new Vector3(0, 0, 0);
        var angle = Utils.Round90(transform.eulerAngles.z);

        shiftVector.x = shift * Utils.Sin(angle);
        shiftVector.y = shift * Utils.Cos(angle);

        State.SlopeShiftMovement = _transform.position + shiftVector;
    }
    
    /**
     * Handle Gap
     */

    private void HandleGap(ref Vector2 deltaMovement)
    {
        var angle = Utils.Round90(transform.eulerAngles.z);
        var rayDistance = 0.4f;
        var rayDirection = Quaternion.Euler(0, 0, angle) * Vector3.down;
        var rayOrigin = _center;

        Debug.DrawRay(rayOrigin, rayDistance * rayDirection, Color.yellow);
        var rayCastHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, Parameters.PlatformMask);
        if (!rayCastHit)
        {
        
            rayCastHit = Physics2D.Raycast(CalculateRotatedPoint(bottomLeftPoint), rayDirection, rayDistance, Parameters.PlatformMask);
            var rayCastHitRight = Physics2D.Raycast(CalculateRotatedPoint(bottomRightPoint), rayDirection, rayDistance, Parameters.PlatformMask);
            Debug.DrawRay(CalculateRotatedPoint(bottomLeftPoint), rayDistance * rayDirection, Color.yellow);
            Debug.DrawRay(CalculateRotatedPoint(bottomRightPoint), rayDistance * rayDirection, Color.yellow);

            if (!rayCastHit && !rayCastHitRight)
            {
                UnSnappingParams();
                return;
            }

            if ((rayCastHit && rayCastHit.transform.gameObject.layer == LevelManager.PLATFORM_LAYER) ||
                (rayCastHitRight && rayCastHitRight.transform.gameObject.layer == LevelManager.PLATFORM_LAYER))
            {

               if ((!rayCastHit && _isFacingForward) ||
                    (rayCastHit && !_isFacingForward) ||
                    (rayCastHit && rayCastHitRight))
                {
                    return;
                }

                angle = !_isFacingForward ? 90 : -90;

                ResetForces();
                State.SlopeAngle = angle;
                State.DestinationAngle = Utils.Round90(transform.eulerAngles.z) + State.SlopeAngle;

                if (Mathf.Abs(State.SlopeAngle) >= 45)
                {
                    SetSnapShiftGap(deltaMovement);
                }

                deltaMovement = Vector2.zero;
                State.MovingMode = MoveMode.Snapping;
            }
           
        }
        else
        {
            var normalVector = rayCastHit.normal;
            Vector2 upVector = Quaternion.Euler(0, 0, transform.eulerAngles.z) * Vector3.up;
            var targetAngle = Vector2.Angle(normalVector, upVector);
            if (targetAngle != 0 && Mathf.Abs(targetAngle) < 45.0f)
            {
                var angleSign = -Mathf.Sign(Vector3.Cross(normalVector, upVector).z);
                transform.Rotate(0, 0, targetAngle * angleSign);
            }

        }
    }

    private void SetSnapShiftGap(Vector2 shiftMovement)
    {
        var shiftY = _halfSize.x + (_halfSize.x);
        var shiftX = _halfSize.y;

        var shiftVector = new Vector3(0, 0, 0);
        var angle = Utils.Round90(transform.eulerAngles.z);
        var tolerance = 45;

        if (angle == 90 || angle == 270)
        {
            var sin = Utils.Sin(angle);
            if (shiftMovement.y > 0)
            {
                shiftVector.x = shiftY * sin;
                shiftVector.y = shiftX;

            }
            else if (shiftMovement.y < 0)
            {
                shiftVector.x = shiftY * sin;
                shiftVector.y = -shiftX;
            }
        }
        else if (angle == 360 || angle == 0 || angle == 180)
        {
            var cos = Utils.Cos(angle);
            if (shiftMovement.x > 0)
            {
                shiftVector.x = shiftX;
                shiftVector.y = -shiftY * cos;
            }
            else if (shiftMovement.x < 0)
            {
                shiftVector.x = -shiftX;
                shiftVector.y = -shiftY * cos;
            }
        }

        State.SlopeShiftMovement = _transform.position + shiftVector;
    }

    /**
     * Overrides
     */
	public override void ResetMovement () 
	{
        gravity.ResetGravity(false);
		_velocity = new Vector2 (0, 0);
		State.SlopeShiftMovement = Vector3.zero;
		State.SlopeAngle = 0;
		_rotated = 0;
		isRotationEnds = false;
		_jumpIn = 1;
		jumpMovement = Vector2.zero;
		_inTheAir = false;
		//gravityMovement = Vector2.zero;
       
		tempJumpShift = Vector2.zero;
		_isRotationPossible = false;
		isJumpRotationEnds = false;
	}

	protected override void EnableMovement ()
	{
        gravity.ResetGravity(false);
		State.MovingMode = MoveMode.Walking;
		SetAniamation("idle", true);

		transform.rotation = Quaternion.Euler(0, 0, 0);

		Vector2 deltaMovement = new Vector2 (0, -0.01f);
		RaycastHit2D raycastHit = CalculateCollisions (ref deltaMovement, false);

		_inTheAir = raycastHit != null; 
		_isMovingEnabled = true;

	}

    protected override void OnPlayerInput(VectorDirection direction)
    {
        if (State.MovingMode != MoveMode.Snapping &&
            _isMovingEnabled != false)
        {

            if (direction == VectorDirection.Up)
            {
                if (CanJump)
                {
                    Jump();
                    SetAniamation("idle", true);

                }
                else if (CanReduceGravity)
                {
                    gravity.ReduceGravity();
                    SetAniamation("fly", true);
                }
                return;
            }

            if ((direction == VectorDirection.Right && !_isFacingForward) ||
                (direction == VectorDirection.Left && _isFacingForward))
            {
                Flip();
            }
           
        }
        _direction = direction;
    }

    /**
     * Walk
     */
	private void Move (Vector2 deltaMovement) 
	{
		_transform.Translate (deltaMovement, Space.World);
	
		if (Time.deltaTime > 0) 
		{
			_velocity = deltaMovement / Time.deltaTime;
		}

        //var signX = Mathf.Sign(_velocity.x);
        //var signY = Mathf.Sign(_velocity.y);
        //_velocity.x = Mathf.Abs(_velocity.x) > Parameters.MaxVelocity.x ? signX * Parameters.MaxVelocity.x : _velocity.x;
        //_velocity.y = Mathf.Abs(_velocity.y) > Parameters.MaxVelocity.y ? signY * Parameters.MaxVelocity.y : _velocity.y;

        if (forceMovementVector != Vector2.zero)
        {
            //SetHorizontalForce(forceMovementVector.x);
            //SetVerticalForce(forceMovementVector.y);
            forceMovementVector = Vector2.zero;
            OnMoving(0.0f , 0.0f);
        }
        else if (_inTheAir)
        {
            OnMoving((float)_energyValue[MoveMode.Falling], deltaMovement.magnitude);
        }
        else
        {
            float moveEnergy = deltaMovement.magnitude > 0 ? (float)_energyValue[MoveMode.Walking] : (float)_energyValue[MoveMode.Standing];
            float velocity = deltaMovement.magnitude > 0 ? deltaMovement.magnitude : 1.0f;
            OnMoving(moveEnergy, velocity);
        }
	}

	private void ValidateWalk (ref Vector2 deltaMovement)
	{
		CalculateOrigins ();
		
        var shiftMovement = new Vector2 (0, 0);
        var angle = Utils.Round90(transform.eulerAngles.z);
        
        float horizontalMagnitude;// = (angle == 270 || angle == 90) ? shiftMovement.y : shiftMovement.x;
        float verticalMagnitude;

        HandleJump(ref shiftMovement);
        //if (State.MovingMode == MoveMode.Snapping)
        //{
        //    deltaMovement = Vector2.zero;
        //    return;
        //}

        

		if (Mathf.Abs (deltaMovement.x) > .001f) 
		{
			shiftMovement.y += Utils.Sin(angle) * deltaMovement.x; 
			shiftMovement.x += Utils.Cos(angle) * deltaMovement.x;
        }

        if (forceMovementVector != Vector2.zero)
        {
            shiftMovement.y += forceMovementVector.y;
            shiftMovement.x += forceMovementVector.x;
            gravity.ResetGravity(false);
        }
        else
        {
            gravity.SetGravity(ref shiftMovement, transform.eulerAngles.z, _inTheAir ? gravity.flyGravity : gravity.walkGravity);
        }
        
        if (angle == 270 || angle == 90)
        {
            horizontalMagnitude = shiftMovement.y;
            verticalMagnitude = shiftMovement.x;
        }
        else
        {
            horizontalMagnitude = shiftMovement.x;
            verticalMagnitude = shiftMovement.y;
        }
        //if (!_inTheAir) 
        //{
        //    Debug.Log(_inTheAir);
        //}
        
        if (!_inTheAir && jumpMovement == Vector2.zero)
        {
            HandleGap(ref shiftMovement);
        }



        var quit = ValidateHorizontalMovement(ref deltaMovement, ref shiftMovement, horizontalMagnitude, angle);
        if (quit)
        {
            return;
        }

         quit = ValidateVerticalMovement(ref deltaMovement, ref shiftMovement, verticalMagnitude, angle);
        if (quit) 
        {
            return;
        }
		deltaMovement = shiftMovement;

        if (_inTheAir)
        {
            return;
        }
        else if (deltaMovement != Vector2.zero)
        {
            SetAniamation("walk", true);
        }
        else
        {
            SetAniamation("idle", true);
        }
	}

    private bool ValidateHorizontalMovement(ref Vector2 deltaMovement, ref Vector2 shiftMovement, float horizontalMagnitude, float angle)
    {
        Vector2 rayOrigin;
        if (Mathf.Abs(horizontalMagnitude) > 0)
        {
            var isMovingRight = (horizontalMagnitude > 0 && (angle == 0 || angle == 90)) || (horizontalMagnitude < 0 && (angle == 180 || angle == 270));
            var verticalRayOrigin = isMovingRight ? bottomRightPoint : bottomLeftPoint;
            var verticalSign = isMovingRight ? 1 : -1;
            var raycastHit = CastRays(ref horizontalMagnitude, TotalVerticalRays, verticalRayOrigin, new Vector2(0, _verticalDistanceBetweenRays), true, out rayOrigin, sign: verticalSign, angleShift: 0, skipFirst: true);

            if (raycastHit.collider != null)
            {
                if (raycastHit.transform.gameObject.layer == LevelManager.PLATFORM_LAYER)
                {
                    _inTheAir = false;
                    gravity.ResetGravity();

                    jumpMovement = Vector2.zero;
                    tempJumpShift = Vector2.zero;
                    HandleCollisionsRotation(raycastHit.normal);
                    if (Mathf.Abs(State.SlopeAngle) >= 45)
                    {
                        SetSnapShift();
                    }
                    ResetForces();
                    deltaMovement = Vector2.zero;
                    State.MovingMode = MoveMode.Snapping;

                    return true;
                }
                else
                {
                    if (angle == 270 || angle == 90)
                    {
                        if (forceMovementVector.y != 0 && Mathf.Abs(horizontalMagnitude) > 0.05f)
                        {
                            shiftMovement.y /= 2;
                            horizontalMagnitude /= 2;
                            ValidateHorizontalMovement(ref deltaMovement, ref shiftMovement, horizontalMagnitude, angle);
                        }
                        else
                        {
                            shiftMovement.y = 0;
                        }

                    }
                    else
                    {
                        if (forceMovementVector.x != 0 && Mathf.Abs(horizontalMagnitude) > 0.05f)
                        {
                            shiftMovement.x /= 2;
                            horizontalMagnitude /= 2;
                            ValidateHorizontalMovement(ref deltaMovement, ref shiftMovement, horizontalMagnitude, angle);
                        }
                        else
                        {
                            shiftMovement.x = 0;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool ValidateVerticalMovement(ref Vector2 deltaMovement, ref Vector2 shiftMovement, float verticalMagnitude, float angle)
    {
        Vector2 rayOrigin;
        if (Mathf.Abs(verticalMagnitude) > 0)
        {
            var isMovingUp = (verticalMagnitude > 0 && (angle == 0 || angle == 270)) || (verticalMagnitude < 0 && (angle == 90 || angle == 180));
            var horizontalRayOrigin = isMovingUp ? topLeftPoint : bottomLeftPoint;
            var horizontalSign = isMovingUp ? -1 : 1;

            var raycastHit = CastRays(ref verticalMagnitude, TotalHorizontalRays, horizontalRayOrigin, new Vector2(_horizontalDistanceBetweenRays, 0), false, out rayOrigin, sign: horizontalSign, angleShift: 90.0f);

            if (raycastHit.collider != null)
            {
                _inTheAir = false;
                gravity.ResetGravity();

                if (raycastHit.transform.gameObject.layer == LevelManager.PLATFORM_LAYER)
                {
                    //_inTheAir = false;
                    //gravity.ResetGravity();
                    if (isMovingUp)
                    {
                        HandleCollisionsRotation(raycastHit.normal);
                        ResetForces();
                        deltaMovement = Vector2.zero;
                        State.MovingMode = MoveMode.Snapping;
                        jumpMovement = Vector2.zero;
                        tempJumpShift = Vector2.zero;
                        return true;
                    }
                    //if (angle == 270 || angle == 90)
                    //{
                    //    shiftMovement.x = 0;
                    //}
                    //else
                    //{
                    //    shiftMovement.y = 0;
                    //}
                }
                else
                {

                    //_inTheAir = false;
                    //gravity.ResetGravity();

                    if (isMovingUp)
                    {
                        jumpMovement = Vector2.zero;
                        tempJumpShift = Vector2.zero;
                    }

                    //if (angle == 270 || angle == 90)
                    //{
                    //    shiftMovement.x = 0;
                    //}
                    //else
                    //{
                    //    shiftMovement.y = 0;
                    //}
                }


                if (angle == 270 || angle == 90)
                {
                    if (forceMovementVector.x != 0 && Mathf.Abs(verticalMagnitude) > 0.05f)
                    {
                        shiftMovement.x /= 2;
                        verticalMagnitude /= 2;
                        ValidateVerticalMovement(ref deltaMovement, ref shiftMovement, verticalMagnitude, angle);
                    }
                    else
                    {
                        shiftMovement.x = 0;
                    }
                   
                }
                else
                {
                    if (forceMovementVector.y != 0 && Mathf.Abs(verticalMagnitude) > 0.05f)
                    {
                        shiftMovement.y /= 2;
                        verticalMagnitude /= 2;
                        ValidateVerticalMovement(ref deltaMovement, ref shiftMovement, verticalMagnitude, angle);
                    }
                    else
                    {
                        shiftMovement.y = 0;
                    }
                }


            }
        }
        return false;
    }

    private void SetDirectedForce(float movementFactor)
    {
        var maxSpeed = Parameters.MaxSpeed;

        if (_inTheAir && _jumpIn < 0) 
        {
            ReduceSpeed(ref maxSpeed);
        }
        else if (!_inTheAir)
        {
            curHorizontalReduce = 0;
        }

        if (_direction == VectorDirection.Right)
        {
            SetHorizontalForce(Mathf.Lerp(_velocity.x, maxSpeed, Time.deltaTime * movementFactor));
        }
        else if (_direction == VectorDirection.Left)
        {
            SetHorizontalForce(Mathf.Lerp(_velocity.x, -1 * maxSpeed, Time.deltaTime * movementFactor));
        }
        else
        {
            SetHorizontalForce(Mathf.Lerp(_velocity.x, 0, Time.deltaTime * movementFactor));
            SetVerticalForce(Mathf.Lerp(_velocity.y, 0, Time.deltaTime * movementFactor));
        }
    }

	private void HandleCollisionsRotation (Vector2 normalVector) {
		Vector2 upVector = Quaternion.Euler(0, 0, transform.eulerAngles.z) * Vector3.up;
		Debug.DrawRay (upVector + _center, 3 * upVector, Color.yellow);

		var targetAngle  = Vector2.Angle (normalVector, upVector);
		var angleSign = - Mathf.Sign(Vector3.Cross(normalVector, upVector).z);
		
		State.SlopeAngle = targetAngle * angleSign;
		State.DestinationAngle = transform.eulerAngles.z + State.SlopeAngle;
	}

    private void ReduceSpeed(ref float maxSpeed)
    {
        curHorizontalReduce = Mathf.Lerp(curHorizontalReduce, maxHorizontalReduce, Time.deltaTime * HorizontalReduceSpeed);
        
        maxSpeed -= curHorizontalReduce;

    }
}
