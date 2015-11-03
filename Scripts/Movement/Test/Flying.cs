using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MovingDirection {
	None = 0,
	Forward = 1,
	Backward = -1
}
public class Flying : MonoBehaviour {
	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;
	private Vector2 _center;
	private Vector2 _halfSize;
	private float _snapShift;
	
	private int TotalHorizontalRays = 4;
	private int TotalVerticalRays = 8;
	private float _verticalDistanceBetweenRays;
	private float _horizontalDistanceBetweenRays;
	
	public LayerMask PlatformMask;
	public float SpeedAccelerationOnGround = 10f;
	public float SpeedAccelerationInAir = 10f;
	public float MaxSpeed = 8;
	private Vector2 _velocity = new Vector2(0, 0);
	
	private VectorDirection _direction = VectorDirection.None;
	private bool _isFacingForward;
	
	public MovingParameters DefaultParameters;
	private MovingParameters _overrideParameters;
	private MovingParameters Parameters { get { return _overrideParameters ?? DefaultParameters; } }
	private const float SkinWidth = .02f;
	public MovingState State { get; private set; }

	//helpers
	private Vector2 bottomRightPoint; 
	private Vector2 bottomLeftPoint; 
	private Vector2 topLeftPoint;
//	private float _rotationTime = 0.5f;
	private float _rotated = 0;

	public SkeletonAnimation skeletonAnimator;
	private string _currentAnimation = "idle";

	private bool _isMovingEnabled = true;
	private bool isRotationEnds = false;
	
	private Vector3 _activeGlobalPlatformPoint,
					_activeLocalPlatformPoint;
	public Vector3 PlatformVelocity { get; private set; }
	private GameObject StandingOn;
	
	public delegate void MovingEvent (MoveMode movingState, float velocity);
	public static event MovingEvent onPlayerMove;

	public static void OnMoving(MoveMode movingState, float velocity)
	{
		if (onPlayerMove != null)
		{
			onPlayerMove(movingState, velocity);
		}
	}
	private void SetAniamation (string name, bool loop)
	{
		if (name == _currentAnimation)
		{
			return;
		}

		skeletonAnimator.state.SetAnimation(0, name, loop);
		_currentAnimation = name;
	}

	void Awake () {

		State = new MovingState ();
		ResetMovement ();

		_boxCollider = GetComponent<BoxCollider2D> ();
		_transform = transform;
		_localScale = transform.localScale;
		
		_halfSize = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		//_snapShift = Mathf.Max (_boxCollider.size.x, _boxCollider.size.y);
		_snapShift = Mathf.Max (_halfSize.x * 2, _halfSize.y * 2);
		CalculateDistanceBetweenRays ();
		_isFacingForward = transform.localScale.x > 0;
	}
	
	public void OnEnable () 
	{
		PlayerController.onPlayerMovingInput += OnPlayerInput;
		PlayerController.onForceMovement += ForceMovement;
		LevelManager.onPlayerDied += DisableMovement;
		LevelManager.onPlayerAlive += EnableMovement;
		LevelManager.onPlayerTeleport += ResolveMovement;
	}
	
	public void OnDisable () 
	{
		PlayerController.onPlayerMovingInput -= OnPlayerInput;
		PlayerController.onForceMovement += ForceMovement;
		LevelManager.onPlayerDied -= DisableMovement;
		LevelManager.onPlayerAlive -= EnableMovement;
		LevelManager.onPlayerTeleport -= ResolveMovement;
	}

	private Vector2 forceMovementVector = Vector2.zero;
	private void ForceMovement (Vector2 movementVector)
	{
		forceMovementVector = movementVector;
	}

	private void ResolveMovement (bool isMovementExpected)
	{
		if (isMovementExpected)
		{
			EnableMovement();
			return;
		}
		DisableMovement ();
	}
	
	private void DisableMovement ()
	{
		ResetMovement ();
		_isMovingEnabled = false;
	}

	private void EnableMovement ()
	{
		if (IsFlying())
		{
			State.MovingMode = MoveMode.Flying;
			SetAniamation("fly", true);
		}
		else 
		{
			State.MovingMode = MoveMode.Walking;
			SetAniamation("idle", true);
		}
		_isMovingEnabled = true;
	}

	private void ResetMovement () 
	{
		_velocity = new Vector2 (0, 0);
		State.SlopeShiftMovement = Vector3.zero;
		State.SlopeAngle = 0;
		StandingOn = null;
		_rotated = 0;
		isRotationEnds = false;
	}
	
	private void OnPlayerInput (VectorDirection direction) {
		if (State.MovingMode != MoveMode.Snapping && 
		    _isMovingEnabled != false &&
		    (
			(direction == VectorDirection.Right && !_isFacingForward) || 
		    (direction == VectorDirection.Left && _isFacingForward)
			))
		{
			Flip();
		}
		_direction = direction;
	}
	
	private void Flip () 
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingForward = transform.localScale.x > 0;
	}

	private void CalculateDistanceBetweenRays () 
	{
		var colliderWidth = _boxCollider.size.x * Mathf.Abs (transform.localScale.x) - (2 * SkinWidth);
		_horizontalDistanceBetweenRays = colliderWidth / (TotalHorizontalRays - 1 );
		
		var colliderHeight = _boxCollider.size.y * Mathf.Abs (transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (TotalVerticalRays - 1);
	}
	
	private void SetFlyingDirectedForce() {
		var movementFactor = SpeedAccelerationInAir;
		if (_direction == VectorDirection.Right) 
		{
			SetHorizontalForce (Mathf.Lerp(_velocity.x, MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_direction == VectorDirection.Left)
		{
			SetHorizontalForce (Mathf.Lerp(_velocity.x, -1 * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_direction == VectorDirection.Up)
		{
			SetVerticalForce (Mathf.Lerp(_velocity.y, MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_direction == VectorDirection.Down)
		{
			SetVerticalForce (Mathf.Lerp(_velocity.y, -1 * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else
		{
			SetHorizontalForce (Mathf.Lerp(_velocity.x, 0, Time.deltaTime * movementFactor));
			SetVerticalForce (Mathf.Lerp(_velocity.y, 0, Time.deltaTime * movementFactor));
		}

	}

	public void SetHorizontalForce (float x) 
	{
		_velocity.x = x;
	}
	
	public void SetVerticalForce (float y) 
	{
		_velocity.y = y;
	}

	private RaycastHit2D CastRays (ref float deltaMovement, int raysCount, Vector2 startPoint, Vector2 pointDistance, bool isHorizontal, out Vector2 rayOrigin, int sign = 1, float angleShift = 0, bool skipFirst = false) 
	{
		var rayDistance = Mathf.Abs (deltaMovement) + SkinWidth;
		var angle = (transform.eulerAngles.z - angleShift) * Mathf.Deg2Rad;
		var rayDirection = new Vector2 (sign * Mathf.Cos(angle), sign * Mathf.Sin(angle));

		RaycastHit2D rayCastHit = Physics2D.Raycast(_center, rayDirection, 0.00001f, PlatformMask);
		rayOrigin = Vector2.zero;

		for (var i = 0; i < raysCount; i++) 
		{
			if (skipFirst && i == 0)
			{
				continue;
			}

			var point = new Vector2 (startPoint.x + (pointDistance.x * i), startPoint.y + (pointDistance.y * i));
			
			Vector2 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * (point - _center); // get point direction relative to pivot, rotate it
			rayOrigin = dir + _center; // calculate rotated point

			Debug.DrawRay (rayOrigin, rayDistance * rayDirection, Color.red);
			rayCastHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, PlatformMask);

			//if don't hit anything, continue loop to next ray
			if  (!rayCastHit) 
			{
				continue;
			}
			return rayCastHit;
		}
		return rayCastHit;
	}

	private void HandleCollisionsRotation (Vector2 normalVector, ref float deltaMovement) {
		Vector2 upVector = Quaternion.Euler(0, 0, transform.eulerAngles.z) * Vector3.up;
		Debug.DrawRay (upVector + _center, 3 * upVector, Color.yellow);

		ResetForces();

		var targetAngle  = Vector2.Angle (normalVector, upVector);
		var angleSign = - Mathf.Sign(Vector3.Cross(normalVector, upVector).z);
		
		State.SlopeAngle = targetAngle * angleSign;

		State.MovingMode = MoveMode.Snapping;
	}

	private void ResetForces ()
	{
		_velocity.x = 0;
		_velocity.y = 0;
	}


	private void RotateWhileSnapping (float angle, float _rotationTime = 0)
	{
		var rotationVector = new Vector3 (0, 0, angle);
		//_rotationTime = Mathf.Clamp(Mathf.Abs(angle)/180.0f, 0.1f, 0.2f);
		_rotated += Mathf.Abs(angle) * (Time.deltaTime / _rotationTime);

		if (Mathf.Abs(angle) > _rotated )
		{
			transform.Rotate( rotationVector * (Time.deltaTime / _rotationTime) );

		} 
		else 
		{
			transform.Rotate( 0, 0, _rotated - Mathf.Abs(angle));
			_rotated = 0;
			isRotationEnds = true;
		}
	}

	private void ShiftWhileSnapping ()
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

	private void Snap (float angle, MoveMode newMode)
	{
		if (State.SlopeShiftMovement == Vector3.zero && isRotationEnds)
		{
			isRotationEnds = false;
			State.MovingMode = newMode;
			return;

		}	

		ShiftWhileSnapping ();
		if (!isRotationEnds) 
		{
			var _rotationTime = Mathf.Clamp(Mathf.Abs(angle)/180.0f, 0.1f, 0.2f);
			RotateWhileSnapping (angle, _rotationTime);
		}
	}

	private void UnSnap (float angle, MoveMode newMode)
	{
		SetAniamation("fly", true);
		var sqrtTolerance = 1.5f;
		var sqrtDistance = 0f;

		if (State.SlopeShiftMovement != Vector3.zero)
		{
			if (State.SlopeShiftMovement != Vector3.zero) 
			{
				_transform.position = Vector3.Lerp(transform.position, State.SlopeShiftMovement, Time.deltaTime * 9);
				
				sqrtDistance = (transform.position - State.SlopeShiftMovement).sqrMagnitude;
				var maxDistance = 0.01f;
				if (sqrtDistance < maxDistance * maxDistance)
				{
					_transform.position = State.SlopeShiftMovement;
					State.SlopeShiftMovement = Vector3.zero;
				}
			}		
		}

		if (sqrtDistance < sqrtTolerance)
		{
			if (!isRotationEnds)
			{
				var _rotationTime =  Mathf.Clamp(Mathf.Abs(angle)/270.0f, 0.4f, 0.7f);
				RotateWhileSnapping (angle, _rotationTime);
			}
		}

		if (isRotationEnds && State.SlopeShiftMovement == Vector3.zero)
		{
			State.MovingMode = newMode;
			transform.eulerAngles = new Vector3(0, 0, 0);
			isRotationEnds = false;
		}
	}

	void LateUpdate () {
		if (_isMovingEnabled) 
		{
			if (State.MovingMode == MoveMode.Flying)
			{
				SetFlyingDirectedForce ();
				//SetGravity (Parameters.FlyGravity * Time.deltaTime);
				Fly (_velocity * Time.deltaTime);
			} 
			else if (State.MovingMode == MoveMode.Snapping) 
			{
				Snap(State.SlopeAngle, MoveMode.Walking);
				
			} 
			else if (State.MovingMode == MoveMode.UnSnapping) 
			{
				UnSnap(State.SlopeAngle, MoveMode.Flying);
			} 
			
			else if (State.MovingMode == MoveMode.Walking)
			{
				SetWalkingDirectedForce();
				Walk(_velocity * Time.deltaTime);
			}
		}

	}

	private bool IsFlying ()
	{
		CalculateOrigins ();
		var angle = transform.eulerAngles.z;
		var rayDistance = 0.8f;
		var rayDirection = Quaternion.Euler(0, 0, angle) * Vector3.down;
		var rayOrigin = _center; 
		
		Debug.DrawRay (rayOrigin, rayDistance * rayDirection, Color.yellow);
		var rayCastHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, PlatformMask);
		
		if  (!rayCastHit) 
		{
			rayCastHit = Physics2D.Raycast(CalculateRotatedPoint(bottomLeftPoint), rayDirection, rayDistance, PlatformMask);
			var rayCastHitRight = Physics2D.Raycast(CalculateRotatedPoint(bottomRightPoint), rayDirection, rayDistance, PlatformMask);

			if (!rayCastHit && !rayCastHitRight)
			{
				return true;
			}
		}

		return false;

	}

	private void HandleGap (ref Vector2 deltaMovement)
	{
		var angle = transform.eulerAngles.z;
		var rayDistance = 0.8f;
		var rayDirection = Quaternion.Euler(0, 0, angle) * Vector3.down;
		var rayOrigin = _center; 
		
		Debug.DrawRay (rayOrigin, rayDistance * rayDirection, Color.red);
		var rayCastHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, PlatformMask);
		
		if  (!rayCastHit) 
		{
			rayCastHit = Physics2D.Raycast(CalculateRotatedPoint(bottomLeftPoint), rayDirection, rayDistance, PlatformMask);
			var rayCastHitRight = Physics2D.Raycast(CalculateRotatedPoint(bottomRightPoint), rayDirection, rayDistance, PlatformMask);
			//var sign = angle >= 180 ? -1 : 1;
			// angle = !rayCastHit ? sign * 90 : sign * -90;
			//angle = !rayCastHit ?  90 : -90;
			if ((!rayCastHit && _isFacingForward) ||
			    (rayCastHit && !_isFacingForward) ||
				(rayCastHit && rayCastHitRight) )
			{
				return;
			}

			angle = !_isFacingForward ?  90 : -90;
			ResetForces();			
			State.SlopeAngle = angle;
			if (Mathf.Abs(State.SlopeAngle) >= 45)
			{
				SetSnapShiftGap (deltaMovement);
			} 
			deltaMovement = Vector2.zero;
			State.MovingMode = MoveMode.Snapping;
		}
		else 
		{
			var normalVector = rayCastHit.normal;
			Vector2 upVector = Quaternion.Euler(0, 0, transform.eulerAngles.z) * Vector3.up;
			var targetAngle  = Vector2.Angle (normalVector, upVector);
			if (targetAngle != 0 && Mathf.Abs(targetAngle) < 45.0f) 
			{
				var angleSign = - Mathf.Sign(Vector3.Cross(normalVector, upVector).z);
				transform.Rotate( 0, 0, targetAngle * angleSign);
				//HandleCollisionsRotation(rayCastHit.normal, ref deltaMovement.y);
				//deltaMovement.y = 0;
			}

		}
	}

	private void SetGravity (float gravity)
	{
		var angle = transform.eulerAngles.z + 90;
		_velocity.y += Mathf.Sin(angle * Mathf.Deg2Rad) * gravity;
		_velocity.x += Mathf.Cos(angle * Mathf.Deg2Rad) * gravity;

        //_velocity.x = Mathf.Min (_velocity.x, Parameters.MaxVelocity.x);
        //_velocity.y = Mathf.Min (_velocity.y, Parameters.MaxVelocity.y);
	}

	private void GetGravityShift (ref Vector2 gravityMovement)
	{
		var angle = Mathf.Round(transform.eulerAngles.z + 90);
		angle = Mathf.Round (angle / 90) * 90;

        //gravityMovement.y = Mathf.Sin(angle * Mathf.Deg2Rad) * Parameters.WalkGravity * Time.deltaTime;
        //gravityMovement.x = Mathf.Cos(angle * Mathf.Deg2Rad) * Parameters.WalkGravity * Time.deltaTime;
	}

	private void HandleGravity (ref Vector2 deltaMovement) 
	{
		var horizontalRayDistance = new Vector2 (_horizontalDistanceBetweenRays, 0);
		var horizontalRayOrigin = bottomLeftPoint;
		var horizontalSign = 1;
		Vector2 rayOrigin;

		var gravityMovement = Vector2.zero;
		GetGravityShift (ref gravityMovement);

		RaycastHit2D raycastHit = CastRays (ref deltaMovement.y, TotalHorizontalRays, horizontalRayOrigin, horizontalRayDistance, false, out rayOrigin, sign: horizontalSign, angleShift: 90.0f);
		if (raycastHit.collider != null)
		{
			if (Mathf.Abs(gravityMovement.y) > 0.0f)
			{
				gravityMovement.y  = 0;
//				gravityMovement.y =  raycastHit.point.y - rayOrigin.y;
//				gravityMovement.y = deltaMovement.y > 0 ? deltaMovement.y - SkinWidth : deltaMovement.y + SkinWidth ;
//				deltaMovement.y *= Mathf.Sin(angle * Mathf.Deg2Rad);
			}

			if (Mathf.Abs(gravityMovement.x) > 0.0f )
			{
				gravityMovement.x = 0;
//				gravityMovement.x = raycastHit.point.x - rayOrigin.x;
//				gravityMovement.x = deltaMovement.x > 0 ? deltaMovement.x - SkinWidth :  deltaMovement.x + SkinWidth;
//				deltaMovement.x *= Mathf.Cos(angle * Mathf.Deg2Rad);
			}

//			StandingOn = raycastHit.collider.gameObject;
//			Debug.Log (StandingOn.name);
		} 
		deltaMovement += gravityMovement;
	} 

	private bool IsUnSnappingPossible()
	{
		Vector2 rayOrigin;
		float deltaMovement = _snapShift;
		var horizontalRayDistance = new Vector2 (_horizontalDistanceBetweenRays, 0);
		var horizontalRayOrigin = topLeftPoint;
		var horizontalSign = -1;
		
		RaycastHit2D raycastHit = CastRays (ref deltaMovement, TotalHorizontalRays, horizontalRayOrigin, horizontalRayDistance, false, out rayOrigin, sign: horizontalSign, angleShift: 90.0f);
		if (raycastHit.collider != null)
		{
			return false;
		}
		return true;

	}



//	private void AddForceMovement ()
//	{
//		if ( forceMovementVector != Vector2.zero)
//		{
//			SetHorizontalForce(forceMovementVector.x);
//			SetVerticalForce(forceMovementVector.y);
//			forceMovementVector = Vector2.zero;
//		}
//	}

//	private void HandlePlatforms()
//	{
//		if (StandingOn != null)
//		{
//			var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
//			var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
//
//			if (moveDistance != Vector3.zero)
//			{
//				var moveVector = new Vector2 (moveDistance.x, moveDistance.y);
////				CalculateCollisions (ref  moveVector, true);
//				transform.Translate( moveVector, Space.World);
//			}
//			PlatformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;
//		}
//		else 
//		{
//			PlatformVelocity = Vector3.zero;
//		}
//
//		StandingOn = null;
//	}

	private void UnSnappingParams ()
	{
		ResetForces();
		CalculateUnSnapShift();
		State.SlopeAngle = -1 * transform.eulerAngles.z;
		State.MovingMode = MoveMode.UnSnapping;
	}
	
	private void SetWalkingDirectedForce() {
		var movementFactor = SpeedAccelerationOnGround;
		if (_direction == VectorDirection.Right) 
		{
			SetHorizontalForce (Mathf.Lerp(_velocity.x, MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_direction == VectorDirection.Left)
		{
			SetHorizontalForce (Mathf.Lerp(_velocity.x, -1 * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_direction == VectorDirection.Up && IsUnSnappingPossible())
		{
			UnSnappingParams ();
		}
		else
		{
			SetHorizontalForce (Mathf.Lerp(_velocity.x, 0, Time.deltaTime * movementFactor));
			SetVerticalForce (Mathf.Lerp(_velocity.y, 0, Time.deltaTime * movementFactor));
		}
		
	}

	private void CalculateUnSnapShift()
	{
		float deltaMovement = _snapShift;
		float angle = Mathf.Round(transform.eulerAngles.z);
		angle = Mathf.Round (angle / 90) * 90;

		var sin = Mathf.Round (Mathf.Sin (angle));
		var cos = Mathf.Round (Mathf.Cos (angle));

		if (angle == 180) 
		{
			sin = 0;
			cos = -1;
		}
		else if (angle == 270)
		{
			sin = -1;
			cos = 0;
		}

		float x = deltaMovement * -sin;
		float y = deltaMovement * cos;

		State.SlopeShiftMovement = new Vector2 (transform.position.x + x, transform.position.y + y);
	}

	private void CalculateCollisions (ref Vector2 deltaMovement, bool isHorizontalMovement) {
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
				if (raycastHit.transform.gameObject.layer == LevelManager.OBSTACLE_LAYER)
				{
					Debug.Log("OBSTACLE");
					deltaMovement.x = 0;
					return;
				}
				HandleCollisionsRotation(raycastHit.normal, ref deltaMovement.x);
				if (Mathf.Abs(State.SlopeAngle) >= 45)
				{
					SetSnapShift (deltaMovement);
				}
				deltaMovement.x = 0;
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
				if (raycastHit.transform.gameObject.layer == LevelManager.OBSTACLE_LAYER)
				{
					deltaMovement.y = 0;
					return;
				}
				HandleCollisionsRotation(raycastHit.normal, ref deltaMovement.y);
				deltaMovement.y = 0;
			}
		}
	}

	private void SetSnapShiftGap (Vector2 shiftMovement)
	{
		//var shift = _halfSize.x - _halfSize.y;
		var shiftY = _halfSize.x + (_halfSize.x );
		var shiftX = _halfSize.y;
		var shiftVector = new Vector3(0, 0, 0); 
		var angle = Mathf.Round(transform.eulerAngles.z);
		var tolerance = 45;

		if ( (angle >= 90 - tolerance && angle < 90 + tolerance) ||
		    (angle >= 270 - tolerance && angle < 270 + tolerance)) 
		{
			var sin = Mathf.Sign(Mathf.Sin(angle));
			if ( shiftMovement.y > 0)
			{
				shiftVector.x = shiftY * sin;
				shiftVector.y = shiftX;
				
			}
			else if ( shiftMovement.y < 0)
			{
				shiftVector.x = shiftY * sin;
				shiftVector.y = -shiftX;
			}
		}
		else if ((angle >= 360 - tolerance && angle < 360)
		         || (angle >= 0 && angle < 0 + tolerance)
		         || (angle >= 180 - tolerance && angle < 180 + tolerance))
		{
			var cos =  Mathf.Sign(Mathf.Cos(angle));
			if ( shiftMovement.x > 0)
			{
				shiftVector.x = shiftX * cos;
				shiftVector.y = -shiftY * cos;
			}
			else if ( shiftMovement.x < 0)
			{
				shiftVector.x = -shiftX * cos;
				shiftVector.y = -shiftY * cos;
			}
		}
		
		State.SlopeShiftMovement = _transform.position + shiftVector;
		//Debug.Log(shiftVector);
	}


	private void SetSnapShift (Vector2 shiftMovement)
	{
		var shift = _halfSize.x - _halfSize.y;
		//var shift = _snapShift;
		var shiftVector = new Vector3(0, 0, 0); 
		var angle = Mathf.Round(transform.eulerAngles.z);
		var tolerance = 45;
		if ( (angle >= 90 - tolerance && angle < 90 + tolerance) ||
		    (angle >= 270 - tolerance && angle < 270 + tolerance)) 
		{
			var sin = -Mathf.Sign(Mathf.Sin(angle));
			if ( shiftMovement.y > 0)
			{
				shiftVector.x = shift * sin;
				shiftVector.y = shift;
				
			}
			else if ( shiftMovement.y < 0)
			{
				shiftVector.x = shift * sin;
				shiftVector.y = -shift;
			}
		}
		else if ((angle >= 360 - tolerance && angle < 360)
		         || (angle >= 0 && angle < 0 + tolerance)
		         || (angle >= 180 - tolerance && angle < 180 + tolerance))
		{
			var cos =  Mathf.Sign(Mathf.Cos(angle));
			if ( shiftMovement.x > 0)
			{
				shiftVector.x = shift * cos;
				shiftVector.y = shift * cos;
			}
			else if ( shiftMovement.x < 0)
			{
				shiftVector.x = -shift * cos;
				shiftVector.y = shift * cos;
			}
		}

		State.SlopeShiftMovement = _transform.position + shiftVector;
		//Debug.Log(shiftVector);
	}

	private void Fly (Vector2 deltaMovement) 
	{
		//HandlePlatforms();
		CalculateOrigins ();
		if (Mathf.Abs (deltaMovement.x) > .001f) 
		{
			CalculateCollisions (ref deltaMovement, true);
		}
		if (Mathf.Abs (deltaMovement.y) > .001f) 
		{
			CalculateCollisions (ref deltaMovement, false);
		}

		_transform.Translate (deltaMovement, Space.World);

		if (Time.deltaTime > 0) 
		{
			_velocity = deltaMovement / Time.deltaTime;
		}

		var signX = Mathf.Sign (_velocity.x);
		var signY = Mathf.Sign (_velocity.y);
        //_velocity.x = Mathf.Abs(_velocity.x) > Parameters.MaxVelocity.x ? signX * Parameters.MaxVelocity.x : _velocity.x;
        //_velocity.y = Mathf.Abs(_velocity.y) > Parameters.MaxVelocity.y ? signY * Parameters.MaxVelocity.y : _velocity.y;

		if ( forceMovementVector != Vector2.zero)
		{
			SetHorizontalForce(forceMovementVector.x);
			SetVerticalForce(forceMovementVector.y);
			forceMovementVector = Vector2.zero;
			OnMoving (State.MovingMode, 0.0f);
		}
		else 
		{
			OnMoving (State.MovingMode, deltaMovement.magnitude);
		}

//		if (StandingOn != null) {
//			_activeGlobalPlatformPoint = transform.position;
//			_activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);
//		}
	}

	private void Walk (Vector2 deltaMovement) 
	{
		//HandlePlatforms();
		CalculateOrigins ();
		var shiftMovement = new Vector2 (0, 0);

		if (Mathf.Abs (deltaMovement.x) > .001f) 
		{
			CalculateCollisions (ref deltaMovement, true);
			HandleGap(ref deltaMovement);
			
			var angle = transform.eulerAngles.z;
			shiftMovement.y += Mathf.Sin(angle * Mathf.Deg2Rad) * deltaMovement.x;
			shiftMovement.x = Mathf.Cos(angle * Mathf.Deg2Rad) * deltaMovement.x;
			
			//			var angle = transform.eulerAngles.z;
			//			if (angle > 90 && angle < 270)
			//			{
			//				deltaMovement.x = -deltaMovement.x;
			//			}
		}
		HandleGravity (ref shiftMovement);
		
		if (shiftMovement != Vector2.zero)
		{
			SetAniamation("walk", true);
		}
		else 
		{
			SetAniamation("idle", true);
		}
		
		_transform.Translate (shiftMovement, Space.World);
		
		if (Time.deltaTime > 0) 
		{
			_velocity = shiftMovement / Time.deltaTime;
		}
		
		var signX = Mathf.Sign (_velocity.x);
		var signY = Mathf.Sign (_velocity.y);
        //_velocity.x = Mathf.Abs(_velocity.x) > Parameters.MaxVelocity.x ? signX * Parameters.MaxVelocity.x : _velocity.x;
        //_velocity.y = Mathf.Abs(_velocity.y) > Parameters.MaxVelocity.y ? signY * Parameters.MaxVelocity.y : _velocity.y;
		
		
		if ( forceMovementVector != Vector2.zero)
		{
			var gravityMovement = Vector2.zero;
			GetGravityShift (ref gravityMovement);
			
			if ( IsUnSnappingPossible() &&
			    (
//				(gravityMovement.y > 0 && forceMovementVector.y < 0 ) ||
//				 (gravityMovement.y < 0 && forceMovementVector.y > 0 ) ||
				 (gravityMovement.x > 0 && forceMovementVector.x < 0 ) ||
			 	 (gravityMovement.x < 0 && forceMovementVector.x > 0 ) ||
			 Mathf.Abs(forceMovementVector.y) > .001f)
			    )
			{
				UnSnappingParams ();
			}
			SetHorizontalForce(forceMovementVector.x);
			SetVerticalForce(forceMovementVector.y);
			forceMovementVector = Vector2.zero;
			OnMoving (State.MovingMode, 0.0f);
		} 
		else 
		{
			OnMoving (State.MovingMode, shiftMovement.magnitude);
		}
		
		//		if (StandingOn != null) {
		//			_activeGlobalPlatformPoint = transform.position;
		//			_activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);
		//		}
		
	}

	private void CalculateOrigins ()
	{
		_center = _transform.position + new Vector3 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		bottomRightPoint = new Vector2 (_center.x + _halfSize.x - SkinWidth, _center.y - _halfSize.y + SkinWidth); 
		bottomLeftPoint = new Vector2 (_center.x - _halfSize.x + SkinWidth, _center.y - _halfSize.y + SkinWidth);
		topLeftPoint = new Vector2 (_center.x - _halfSize.x + SkinWidth, _center.y + _halfSize.y - SkinWidth);
//		Debug.DrawRay( CalculateRotatedPoint (bottomLeftPoint), _center, Color.green); 
//		Debug.DrawRay( CalculateRotatedPoint (topLeftPoint), _center, Color.cyan); 
//		Debug.DrawRay( CalculateRotatedPoint (bottomRightPoint), _center, Color.magenta); 
	}

	private Vector2 CalculateRotatedPoint (Vector2 point)
	{
		Vector2 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * (point - _center); // get point direction relative to pivot, rotate it
		return dir + _center; // calculate rotated point
	}
	
	
}




//	private void Snap (float angle, MoveMode newMode)
//	{
//		var endPosition = State.SlopeShiftMovement; 
//		var rotationVector = new Vector3 (0, 0, angle);
//		_rotationTime = Mathf.Clamp(Mathf.Abs(angle)/180.0f, 0.1f, 0.3f);
//
//		_rotated += Mathf.Abs(angle) * (Time.deltaTime / _rotationTime);
//		if (Mathf.Abs(angle) > _rotated )
//		{
//			transform.Rotate( rotationVector * (Time.deltaTime / _rotationTime) );
//
//			if (State.SlopeShiftMovement != Vector3.zero) 
//			{
//				_transform.position = Vector3.Lerp(_transform.position, endPosition, Time.deltaTime * 5);
//			}
//		
//		} 
//		else 
//		{
//
////			var journeyLength = Vector3.Distance(_transform.position, endPosition);
////			if (journeyLength > 0.01f) 
////			{
////				_transform.position = Vector3.Lerp(_transform.position, endPosition, Time.deltaTime * 2);
////			}
////			else 
////			{
//			if (State.SlopeShiftMovement != Vector3.zero) 
//			{
//				_transform.position = endPosition;
//			}
//				transform.Rotate( 0, 0, _rotated - Mathf.Abs(angle));
//				_rotated = 0;
//				State.SlopeShiftMovement = Vector3.zero;
//				State.MovingMode = newMode;
////			}
//
//		}
//	}
//	