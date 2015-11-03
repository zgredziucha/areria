using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {



	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;
	private Vector2 _center;
	private Vector2 _halfSize;

	private int TotalHorizontalRays = 4;
	private int TotalVerticalRays = 8;
	private float _verticalDistanceBetweenRays;
	private float _horizontalDistanceBetweenRays;

	public LayerMask PlatformMask;
	public float SpeedAccelerationOnGround = 10f;
	public float MaxSpeed = 8;
	private Vector2 _velocity = new Vector2(0, 0);

	private MovingDirection _direction = 0;
	private VectorDirection _forwardVector = VectorDirection.Right;
	private bool _isFacingForward;

	public MovementParameters DefaultParameters;
	private MovementParameters _overrideParameters;
	private MovementParameters Parameters { get { return _overrideParameters ?? DefaultParameters; } }
//	private float _rotationAngle = 0f;
	private const float SkinWidth = .02f;

	void Awake () {
		_boxCollider = GetComponent<BoxCollider2D> ();
		_transform = transform;
		_localScale = transform.localScale;

		_halfSize = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;

		CalculateDistanceBetweenRays ();
		_isFacingForward = transform.localScale.x > 0;
		//_rotationAngle = transform.eulerAngles.z;
	}

	public void OnEnable () 
	{
//		PlayerController.onPlayerMove += OnPlayerInput;
		LevelManager.onPlayerDied += SetDieMovement;
	}
	
	public void OnDisable () 
	{
//		PlayerController.onPlayerMove -= OnPlayerInput;
		LevelManager.onPlayerDied += SetDieMovement;
	}

	private void SetDieMovement ()
	{
		//TODO:Implement me!
	}
	
	private void OnPlayerInput (MovingDirection direction) {
		if ((direction == MovingDirection.Forward && !_isFacingForward) || 
		    (direction == MovingDirection.Backward && _isFacingForward))
		{
			Flip();
		}
		_direction = direction;
	}

	private void Flip () 
	{
		//if change local scale in x dimension to opposite negative number it will flip sprite vertically
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		// if local scale in x dimension is greater than 0 i facing right, else it facing left
		_isFacingForward = transform.localScale.x > 0;
	}


	private void CalculateDistanceBetweenRays () 
	{
		var colliderWidth = _boxCollider.size.x * Mathf.Abs (transform.localScale.x) - (2 * SkinWidth);
		_horizontalDistanceBetweenRays = colliderWidth / (TotalHorizontalRays - 1 );
		
		var colliderHeight = _boxCollider.size.y * Mathf.Abs (transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (TotalVerticalRays - 1);
	}

	private void SetDirectedForce() {
		//var movementFactor = State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
		var movementFactor = SpeedAccelerationOnGround;
		var direction = (int)_direction;
		if (_forwardVector == VectorDirection.Right) 
		{
			SetHorizontalForce (Mathf.Lerp(_velocity.x, direction * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_forwardVector == VectorDirection.Left)
		{
			SetHorizontalForce (Mathf.Lerp(_velocity.x, -direction * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_forwardVector == VectorDirection.Down)
		{
			SetVerticalForce (Mathf.Lerp(_velocity.y, direction * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_forwardVector == VectorDirection.Up)
		{
			SetVerticalForce (Mathf.Lerp(_velocity.y, -direction * MaxSpeed, Time.deltaTime * movementFactor));
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



	private Vector2 CastRays (float deltaMovement, int raysCount, Vector2 startPoint, Vector2 pointDistance, bool isHorizontal, int sign = 1, float angleShift = 0) 
	{
		var rayDistance = Mathf.Abs (deltaMovement) + SkinWidth;
		var angle = (transform.eulerAngles.z - angleShift) * Mathf.Deg2Rad;
		var rayDirection = new Vector2 (sign * Mathf.Cos(angle), sign * Mathf.Sin(angle));
		//var rotationAngle = 0;

		for (var i = 0; i < raysCount; i++) 
		{
			var point = new Vector2 (startPoint.x + (pointDistance.x * i), startPoint.y + (pointDistance.y * i));

			Vector2 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * (point - _center); // get point direction relative to pivot, rotate it
			var rayOrigin = dir + _center; // calculate rotated point


			Debug.DrawRay (rayOrigin, rayDistance * rayDirection, Color.red);
			var rayCastHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, PlatformMask);

			//if don't hit anything, continue loop to next ray
			if  (!rayCastHit) 
			{
				continue;
			}



			if ( isHorizontal)
			{
				if (i == 0) 
				{
					var rotationAngle = Vector2.Angle (rayCastHit.normal, Vector2.up);
					HandleWall (rotationAngle, _isFacingForward);
					break;
				}
				deltaMovement = rayCastHit.point.x - rayOrigin.x;
			}
			else 
			{
				//deltaMovement = rayCastHit.point.y - rayOrigin.y;
			}

			rayDistance = Mathf.Abs(deltaMovement);


			if (_isFacingForward) 
			{
				deltaMovement -= SkinWidth;
			}
			else 
			{
				deltaMovement += SkinWidth;
			}
//
//			//if ray is so small that nearly overlap with skinwith (overlap with object)
			if (rayDistance < SkinWidth + .00001f)
			{
				break;
			}

		}

		var movementVector = new Vector2 (0, 0);
		movementVector.x = deltaMovement;
		//movementVector.y = Mathf.Tan (transform.eulerAngles.z * Mathf.Deg2Rad) * deltaMovement;
		return movementVector;
	}

	private void HandleWall (float angle, bool isGoingForward)
	{
		var sign = isGoingForward ? 1 : -1;
		transform.Rotate (0, 0, sign * angle);
//		if (transform.eulerAngles.z < 90 )
//		{
//			_forwardVector = VectorDirection.Right;
//		}
//		else if (transform.eulerAngles.z < 180 )
//		{
//			_forwardVector = VectorDirection.Down;
//		}
//		else if (transform.eulerAngles.z < 270 )
//		{
//			_forwardVector = VectorDirection.Left;
//		}
//		else if (transform.eulerAngles.z < 360 )
//		{
//			_forwardVector = VectorDirection.Up;
//		}
	}

	// Use this for initialization
	void LateUpdate () {
		SetDirectedForce ();
		//_velocity.x = 9;
		Move (_velocity * Time.deltaTime);




//		if (Input.GetKeyUp (KeyCode.D))
//		{
//			transform.Rotate (0, 0, 30);
//		} 
	}

	private void CalculateMovement (ref Vector2 deltaMovement, bool isHorizontalMovement) {
		_center = _transform.position + new Vector3 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		var bottomRightPoint = new Vector2 (_center.x + _halfSize.x - SkinWidth, _center.y - _halfSize.y + SkinWidth); 
		var bottomLeftPoint = new Vector2 (_center.x - _halfSize.x + SkinWidth, _center.y - _halfSize.y + SkinWidth);
	
		if (isHorizontalMovement)
		{
			var verticalRayDistance = new Vector2 (0, _verticalDistanceBetweenRays);
			var verticalRayOrigin = _isFacingForward ? bottomRightPoint : bottomLeftPoint;
			var verticalSign = _isFacingForward ? 1 : -1;
			deltaMovement = CastRays (deltaMovement.x, TotalVerticalRays, verticalRayOrigin, verticalRayDistance, isHorizontalMovement, verticalSign, 0);
		}
		else 
		{
//			var horizontalRayDistance = new Vector2 (_horizontalDistanceBetweenRays, 0);
//			deltaMovement.y = CastRays (deltaMovement.y, TotalHorizontalRays, bottomLeftPoint, horizontalRayDistance, isHorizontalMovement, 1, 90.0f);
		}
	}

	private void Move (Vector2 deltaMovement) 
	{
//		var wasGrounded = State.IsCollidingBelow;
//		State.Reset ();
		if (Mathf.Abs (deltaMovement.x) > .001f) 
		{
			CalculateMovement (ref deltaMovement, true);
		}
//		if (Mathf.Abs (deltaMovement.y) > .001f) 
//		{
//			CalculateMovement (ref deltaMovement, false);
//		}
		//moving the player by delta movement in the world space
		_transform.Translate (deltaMovement, Space.World);
		
		//refresh velocity value if something change
		if (Time.deltaTime > 0) 
		{
			_velocity = deltaMovement / Time.deltaTime;
		}
		_velocity.x = Mathf.Min (_velocity.x, Parameters.MaxVelocity.x);
		_velocity.y = Mathf.Min (_velocity.y, Parameters.MaxVelocity.y);
		
	}


}


//private Vector3 SetVectorFromAngle (Vector3 startVector, Vector3 center) 
//{
//	var distance = Vector3.Distance (center, startVector);
//	var angle = transform.eulerAngles.z;
//	var rotation = Quaternion.Euler (0, 0, angle);
//	//		var vector = Vector3.right * distance;
//	//		return startVector + (rotation * vector);
//	var vector = new Vector3(1,1,0) * distance;
//	return startVector + (rotation * vector);
//	
//}


//
//private void UpdateRay () {
//	var _boxCollider = GetComponent<BoxCollider2D> ();
//	var _transform = transform;
//	var _localScale = transform.localScale;
//	
//	var SkinWidth = .02f;
//	var size = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
//	var center = new Vector2 (_boxCollider.center.x * _localScale.x, _boxCollider.center.y * _localScale.y);
//	
//	var _pointTopLeft = _transform.position + new Vector3 (center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
//	var _pointBottomRight = (_transform.position + new Vector3 (center.x + size.x - SkinWidth, center.y - size.y + SkinWidth));
//	var _pointBottomLeft = (_transform.position + new Vector3 (center.x - size.x + SkinWidth, center.y - size.y + SkinWidth));
//	
//	center = _transform.position + new Vector3 (center.x, center.y, 0);
//	var _raycastTopLeft = SetVectorFromAngle (_pointTopLeft, center);
//	var _raycastBottomLeft = SetVectorFromAngle (_pointTopLeft, center);
//	
//	
//	
//	
//	
//	var _raycastBottomRight = SetVectorFromAngle (_pointBottomRight, center);
//	
//	
//	
//	
//	
//	
//	
//	var TotalHorizontalRays = 2;
//	var colliderHeight = _boxCollider.size.y * Mathf.Abs (transform.localScale.y) - (2 * SkinWidth);
//	var _verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
//	
//	var deltaMovement = new Vector2 (3.0f, 0);
//	Debug.Log (transform.eulerAngles.z);
//	var angle = transform.eulerAngles.z * Mathf.Deg2Rad;
//	var isGoingRight = true;
//	var rayDistance = Mathf.Abs (deltaMovement.x) + SkinWidth;
//	var rayDirection = new Vector2 (Mathf.Cos(angle), Mathf.Sin(angle));//isGoingRight ? Vector2.right : -Vector2.right;
//	var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;
//	
//	for (var i = 0; i < TotalHorizontalRays; i++) 
//	{
//		var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
//		Debug.DrawRay (rayVector, rayDistance * rayDirection, Color.red);
//		var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance);
//		
//		if  (!rayCastHit) 
//		{
//			continue;
//		}
//		
//		
//		
//		
//	}
//	
//	if (Input.GetKeyUp (KeyCode.D))
//	{
//		
//		var sign = isGoingRight ? 1 : -1;
//		transform.Rotate (0, 0, sign * 30);
//		
//		
//	} 
//}
//
//
//private void UpdateRay2 () {
//	var _boxCollider = GetComponent<BoxCollider2D> ();
//	var _transform = transform;
//	var _localScale = transform.localScale;
//	
//	var size = _boxCollider.size / 2;
//	var center = new Vector3 (_boxCollider.center.x, _boxCollider.center.y, 0);
//	
//	var point = (_transform.position + new Vector3 (center.x + size.x , center.y - size.y));
//	
//	
//	var dir = point - center; // get point direction relative to pivot
//	dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * dir; // rotate it
//	var _point = dir + center; // calculate rotated point
//	
//	
//	
//	var deltaMovement = new Vector2 (3.0f, 0);
//	var angle = transform.eulerAngles.z * Mathf.Deg2Rad;
//	
//	var rayDistance = Mathf.Abs (deltaMovement.x);
//	var rayDirection = new Vector2 (Mathf.Cos(angle), Mathf.Sin(angle));//isGoingRight ? Vector2.right : -Vector2.right;
//	var rayOrigin = _point; 
//	var rayVector = new Vector2(rayOrigin.x, rayOrigin.y);
//	Debug.DrawRay (rayVector, rayDistance * rayDirection, Color.red);
//	
//	
//	
//	
//	
//	if (Input.GetKeyUp (KeyCode.D))
//	{
//		transform.Rotate (0, 0, 30);
//		
//		
//	} 
//}
