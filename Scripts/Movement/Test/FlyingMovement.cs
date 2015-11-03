using UnityEngine;
using System.Collections;

public class FlyingMovement : MonoBehaviour 
{
	private const float SkinWidth = .02f;
	private const int TotalHorizontalRays = 8;
	private const int TotalVerticalRays = 4;
		
	public LayerMask PlatformMask;
	public MovementParameters DefaultParameters;
	
	public MovementState State { get; private set; }

	public Vector2 Velocity { get { return _velocity; } }

	// if _overrideParameters is null return default
	private MovementParameters Parameters { get { return _overrideParameters ?? DefaultParameters; } }
	
	private Vector2 _velocity;
	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;
	private MovementParameters _overrideParameters;
	private float _jumpIn;
	
	private float _verticalDistanceBetweenRays,
	_horizontalDistanceBetweenRays;
	
	private Vector3
		_activeGlobalPlatformPoint,
		_activeLocalPlatformPoint;
	
	private Vector3 _raycastTopLeft,
	_raycastBottomLeft,
	_raycastBottomRight;

	private bool _isFacingForward;
	//private PlayerController _controller;
	private float _normalizedHorizontalSpeed;
	
	public float MaxSpeed = 8;
	public float SpeedAccelerationOnGround = 10f;
	public float SpeedAccelerationInAir = 5f;
	private MovingDirection _direction = 0;
	private VectorDirection _forwardVector;
	public enum VectorDirection
	{
		Up,
		Down,
		Right,
		Left
	}

	public void Awake () 
	{
		State = new MovementState ();
		
		//better to store transforms and references if we will call them often
		_transform = transform;
		_localScale = transform.localScale;
		_boxCollider = GetComponent<BoxCollider2D> ();
		
		// size of a collider multiplied by his scale, minus skinWidth in the corners
		var colliderWidth = _boxCollider.size.x * Mathf.Abs (transform.localScale.x) - (2 * SkinWidth);
		//calculated box colllider width divided by needed pieces(for 3 rays - 2 pieces)
		_horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1 );
		
		var colliderHeight = _boxCollider.size.y * Mathf.Abs (transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);

		_isFacingForward = transform.localScale.x > 0;
		_forwardVector = VectorDirection.Right;
	}

	public void OnEnable () 
	{
		//PlayerController.onPlayerMove += onPlayerInput;
	}

	public void OnDisable () 
	{
		//PlayerController.onPlayerMove -= onPlayerInput;
	}

	private void onPlayerInput (MovingDirection direction) {
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

	public void AddForce (Vector2 force)
	{
		_velocity += force;
	}
	
	public void SetForce (Vector2 force)
	{
		_velocity = force;
	}
	
	public void SetHorizontalForce (float x) 
	{
		_velocity.x = x;
	}
	
	public void SetVerticalForce (float y) 
	{
		_velocity.y = y;
	}

	private void SetDirectedForce() {
		var movementFactor = State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
		var direction = (int)_direction;
		if (_forwardVector == VectorDirection.Right) 
		{
			SetHorizontalForce (Mathf.Lerp(Velocity.x, direction * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_forwardVector == VectorDirection.Left)
		{
			SetHorizontalForce (Mathf.Lerp(Velocity.x, -direction * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_forwardVector == VectorDirection.Down)
		{
			SetVerticalForce (Mathf.Lerp(Velocity.y, direction * MaxSpeed, Time.deltaTime * movementFactor));
		}
		else if (_forwardVector == VectorDirection.Up)
		{
			SetVerticalForce (Mathf.Lerp(Velocity.y, -direction * MaxSpeed, Time.deltaTime * movementFactor));
		}
	}
	
	public void LateUpdate()
	{
		SetDirectedForce ();
		Move (Velocity * Time.deltaTime);
	}
	
	private void Move (Vector2 deltaMovement) 
	{
		var wasGrounded = State.IsCollidingBelow;
		State.Reset ();

		//caltulate raycast position every frame
		CalculateRaysOrigin();

		//if it moving horizontally
		if (Mathf.Abs (deltaMovement.x) > .001f)
		{
			MoveHorizontally(ref deltaMovement);
		}

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

	private void CalculateRaysOrigin () 
	{
		//distance from center to edge of box collider
		var size = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		//center of the box collider
		var center = new Vector2 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		
		_raycastTopLeft = _transform.position + new Vector3 (center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
		_raycastBottomRight = _transform.position + new Vector3 (center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
		_raycastBottomLeft = _transform.position + new Vector3 (center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);
	}



	private void MoveHorizontally (ref Vector2 deltaMovement)
	{
		var isGoingRight = deltaMovement.x > 0;
		//lenght of movement vector
		var rayDistance = Mathf.Abs (deltaMovement.x) + SkinWidth;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		//starting point of raycasting
		var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;
		
		for (var i = 0; i < TotalHorizontalRays; i++) 
		{
			//to construct the ray vector take start point of raycasting in x dimension and y dimensions with calculated offset for each ray.
			var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
			Debug.DrawRay (rayVector, rayDistance * rayDirection, Color.red);
			//raycast
			var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
			//if don't hit anything, continue loop to next ray
			if  (!rayCastHit) 
			{
				continue;
			}

			//HandleWall (ref deltaMovement, Vector2.Angle (rayCastHit.normal, Vector2.up), isGoingRight);
			//if there is a horizontal slope
			if (i == 0 && HandleWall (ref deltaMovement, Vector2.Angle (rayCastHit.normal, Vector2.up), isGoingRight)) 
			{
				//break, movement will be done for us

				break;
			}
			//calculate distance between raycast hit point and ray vector - it will give ditance between player and obstacle (it will not overlap witj it) 
			deltaMovement.x = rayCastHit.point.x - rayVector.x;
			//change ray distance - the next loop will casting smaller ray (distance beetween player and obstacle, so next time only obstacles placed closer will be detected
			rayDistance = Mathf.Abs(deltaMovement.x);
			
			//Handle SkinWidth - adding to the movement 
			if (isGoingRight) 
			{
				deltaMovement.x -= SkinWidth;
				State.IsCollidingRight = true;
			}
			else 
			{
				deltaMovement.x += SkinWidth;
				State.IsCollidingLeft = true;
			}
			
			//if ray is so small that nearly overlap with skinwith (overlap with object)
			if (rayDistance < SkinWidth + .00001f)
			{
				break;
			}
		}
	}
	


	private bool HandleWall (ref Vector2 deltaMovement, float angle, bool isGoingRight)
	{
		var sign = isGoingRight ? 1 : -1;
		transform.Rotate (0, 0, sign * angle);
		if (transform.eulerAngles.z < 90 )
		{
			_forwardVector = VectorDirection.Right;
		}
		else if (transform.eulerAngles.z < 180 )
		{
			_forwardVector = VectorDirection.Down;
		}
		else if (transform.eulerAngles.z < 270 )
		{
			_forwardVector = VectorDirection.Left;
		}
		else if (transform.eulerAngles.z < 360 )
		{
			_forwardVector = VectorDirection.Up;
		}
		return true;
	}


}
//
//private void MoveForward (ref Vector2 deltaMovement) {
//	var rayDistance = rayRight.Distance(deltaMovement, SkinWidth);
//	var rayDirection = rayRight.Direction(deltaMovement);
//	var rayOrigin = rayRight.Origin(deltaMovement);
//	
//	for (var i = 0; i < TotalHorizontalRays; i++) 
//	{
//		//to construct the ray vector take start point of raycasting in x dimension and y dimensions with calculated offset for each ray.
//		var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
//		Debug.DrawRay (rayVector, rayDistance * rayDirection, Color.red);
//		//raycast
//		var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
//		//if don't hit anything, continue loop to next ray
//		if  (!rayCastHit) 
//		{
//			continue;
//		}
//		
//		//HandleWall (ref deltaMovement, Vector2.Angle (rayCastHit.normal, Vector2.up), isGoingRight);
//		//if there is a horizontal slope
//		//			if (i == (TotalHorizontalRays - 1) && HandleWall (ref deltaMovement, Vector2.Angle (rayCastHit.normal, Vector2.up), isGoingRight)) 
//		//			{
//		//				//break, movement will be done for us
//		//				
//		//				break;
//		//			}
//		//calculate distance between raycast hit point and ray vector - it will give ditance between player and obstacle (it will not overlap witj it) 
//		deltaMovement.x = rayCastHit.point.x - rayVector.x;
//		//change ray distance - the next loop will casting smaller ray (distance beetween player and obstacle, so next time only obstacles placed closer will be detected
//		rayDistance = Mathf.Abs(deltaMovement.x);
//		
//		//Handle SkinWidth - adding to the movement 
//		if (isGoingRight) 
//		{
//			deltaMovement.x -= SkinWidth;
//			State.IsCollidingRight = true;
//		}
//		else 
//		{
//			deltaMovement.x += SkinWidth;
//			State.IsCollidingLeft = true;
//		}
//		
//		//if ray is so small that nearly overlap with skinwith (overlap with object)
//		if (rayDistance < SkinWidth + .00001f)
//		{
//			break;
//		}
//	}
//}


//CustomRay rayRight;
//public void Start () {
//	rayRight = new CustomRay ();
//	rayRight.Distance  = delegate(Vector2 deltaMovement, float SkinWidth)
//	{
//		return Mathf.Abs (deltaMovement.x) + SkinWidth; 
//	};
//	rayRight.Direction  = delegate(Vector2 deltaMovement)
//	{
//		return deltaMovement.x > 0 ? Vector2.right : -Vector2.right; 
//	};
//	rayRight.Origin  = delegate(Vector2 deltaMovement)
//	{
//		return deltaMovement.x > 0 ? _raycastBottomRight : _raycastBottomLeft; 
//	};
//	
//	
//}
//
//public class CustomRay {
//	public delegate float Distance (Vector2 deltaMovement, float SkinWidth);
//	public delegate Vector2 Direction (Vector2 deltaMovement);
//	public delegate Vector2 Origin (Vector2 deltaMovement);
//}

//private void MoveVertically (ref Vector2 deltaMovement)
//{
//	var isGoingUp = deltaMovement.y > 0;
//	var rayDistance = Mathf.Abs (deltaMovement.y) + SkinWidth;
//	var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
//	var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;
//	
//	//position that we arer trying to go, ont where we are 
//	rayOrigin.x += deltaMovement.x;
//	
//	
//	for (var i = 0; i < TotalVerticalRays; i++) 
//	{
//		var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
//		Debug.DrawRay (rayVector, rayDirection * rayDistance, Color.red);
//		
//		var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
//		if (!raycastHit)
//		{
//			continue;
//		}
//		
//		Debug.Log(Vector2.Angle (raycastHit.normal, Vector2.up));
//		Debug.Log(Vector2.Angle (raycastHit.normal, Vector2.right));
//		
//		if (i == 0 && HandleWall (ref deltaMovement, Vector2.Angle (raycastHit.normal, Vector2.right), isGoingUp)) 
//		{
//			//break, movement will be done for us
//			
//			break;
//		}
//		
//		
//		//farthes distance between player and obsticle
//		deltaMovement.y = raycastHit.point.y - rayVector.y;
//		//shortest ray distance
//		rayDistance = Mathf.Abs(deltaMovement.y);
//		
//		if (isGoingUp)
//		{
//			deltaMovement.y -= SkinWidth;
//			State.IsCollidingAbove = true;
//		}
//		else 
//		{
//			deltaMovement.y += SkinWidth;
//			State.IsCollidingBelow = true;
//		}
//		
//		if (rayDistance < SkinWidth+ .0001f)
//		{
//			break;
//		}
//	}
//	
//}

