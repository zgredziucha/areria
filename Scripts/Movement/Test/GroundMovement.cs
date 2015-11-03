using UnityEngine;
using System.Collections;

public class GroundMovement : MonoBehaviour 
{
	private const float SkinWidth = .02f;
	private const int TotalHorizontalRays = 8;
	private const int TotalVerticalRays = 4;

	private static readonly float SlopeLimitTangent = Mathf.Tan (75f* Mathf.Deg2Rad);

	public LayerMask PlatformMask;
	public MovementParameters DefaultParameters;

	public MovementState State { get; private set; }

	// This is a method and Vector is stucture type, so the property (method) is returning copy of this structure
	// This is not a variable !
	// Vector is value type, stucture
	// public Vector2 Velocity { get; private set; }
	// Convert from automatic property, to ptoperty with the backing field:
	public Vector2 Velocity { get { return _velocity; } }
	public bool CanJump 
	{ 
		get { 
			if (Parameters.JumpRestriction == MovementParameters.JumpBehavior.CanJumpAnywhere)
			{
				return _jumpIn < 0;
			}
			if (Parameters.JumpRestriction == MovementParameters.JumpBehavior.CanJumpOnGround)
			{
				return State.IsGrounded;
			}
			return false; 
		} 
	}
	public bool HandleCollisions { get; set; }

	// if _overrideParameters is null return default
	private MovementParameters Parameters { get { return _overrideParameters ?? DefaultParameters; } }
	public GameObject StandingOn { get; private set; }
	public Vector3 PlatformVelocity { get; private set; } 

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

	public void Awake () 
	{
		HandleCollisions = true;
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
	
	public void  Jump () 
	{
		AddForce (new Vector2 (0, Parameters.JumpMagnitude));
		_jumpIn = Parameters.JumpFrequency;
	}

	public void LateUpdate()
	{
		_jumpIn -= Time.deltaTime;
		_velocity.y += Parameters.Gravity * Time.deltaTime;
		Move (Velocity * Time.deltaTime);
	}

	private void Move (Vector2 deltaMovement) 
	{
		var wasGrounded = State.IsCollidingBelow;
		State.Reset ();

		if (HandleCollisions) 
		{
			HandlePlatforms();
			//caltulate raycast position every frame
			CalculateRaysOrigin();
			//if it's moving down and stand on the graound
			if (deltaMovement.y < 0 && wasGrounded)
			{
				HandleVerticalSlope (ref deltaMovement);
			}

			//if it moving horizontally
			if (Mathf.Abs (deltaMovement.x) > .001f)
			{
				MoveHorizontally(ref deltaMovement);
			}

			//it will always be some vertical movement because of gravity
			MoveVertically(ref deltaMovement);
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

		if (State.IsMovingUpSlope) 
		{
			_velocity.y = 0;
		}

		//platformCode
		if (StandingOn != null) 
		{
			_activeGlobalPlatformPoint = transform.position;
			//relative position - local position - on the platform when we standing
			// inverseTransformPoint - Transforms position from world space to local space - The opposite of Transform.TransformPoint.
			_activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);
		}
	}

	private void HandlePlatforms()
	{
		if (StandingOn != null) 
		{
			//new gloabal platform poit inverse from local point
			//_activeLocalPlatformPoint - transform from last frame!
			var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
			//move distance - based on where we were and were we are now
			var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

			//if there were some movement (offset between current transform of platform and transform from last frame
			if (moveDistance != Vector3.zero) 
			{
				//move the player
				transform.Translate(moveDistance, Space.World);
			}

			PlatformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;

		}
		else
		{
			PlatformVelocity = Vector3.zero;
		}

		StandingOn = null;
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
			//TODO: DLACZEGO rayOrigin.y - a nie rayOrigin.y + !!!
			var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
			Debug.DrawRay (rayVector, rayDistance * rayDirection, Color.red);
			//raycast
			var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
			//if don't hit anything, continue loop to next ray
			if  (!rayCastHit) 
			{
				continue;
			}

			//if there is a horizontal slope
			if (i == 0 && HandleHorizontalSlope (ref deltaMovement, Vector2.Angle (rayCastHit.normal, Vector2.up), isGoingRight)) 
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

	private void MoveVertically (ref Vector2 deltaMovement)
	{
		var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs (deltaMovement.y) + SkinWidth;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;

		//position that we arer trying to go, ont where we are 
		rayOrigin.x += deltaMovement.x;

		var standingOnDistance = float.MaxValue;
		for (var i = 0; i < TotalVerticalRays; i++) 
		{
			var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
			Debug.DrawRay (rayVector, rayDirection * rayDistance, Color.red);

			var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
			if (!raycastHit)
			{
				continue;
			}

			//if we falling
			if (!isGoingUp) 
			{
				//distance between player and ground
				var verticalDistanceToHit = _transform.position.y - raycastHit.point.y;
				//if current distance between player and ground is less than last raycast betweenr player and ground
				if (verticalDistanceToHit < standingOnDistance) 
				{
					//store the new, shortest distance between player and ground 
					standingOnDistance = verticalDistanceToHit;
					//closest ground object we will collide
					StandingOn = raycastHit.collider.gameObject;
				}
			}

			//farthes distance between player and obsticle
			deltaMovement.y = raycastHit.point.y - rayVector.y;
			//shortest ray distance
			rayDistance = Mathf.Abs(deltaMovement.y);

			if (isGoingUp)
			{
				deltaMovement.y -= SkinWidth;
				State.IsCollidingAbove = true;
			}
			else 
			{
				deltaMovement.y += SkinWidth;
				State.IsCollidingBelow = true;
			}
			
			if (!isGoingUp && deltaMovement.y > .0001f ) 
			{
				State.IsMovingUpSlope = true;
			}
			
			if (rayDistance < SkinWidth+ .0001f)
			{
				break;
			}
		}

	}

	private void HandleVerticalSlope (ref Vector2 deltaMovement)
	{
		//center of the box collider
		var center = (_raycastTopLeft.x + _raycastBottomRight.x) / 2;
		var direction = -Vector2.up;
		var slopeDistance = SlopeLimitTangent * (_raycastBottomRight.x - center);
		//vector - center as x, bottom as y
		var slopeRayVector = new Vector2 (center, _raycastBottomLeft.y);

		Debug.DrawRay (slopeRayVector, direction * slopeDistance, Color.yellow);

		var raycastHit = Physics2D.Raycast (slopeRayVector, direction, slopeDistance, PlatformMask);
		//if doesn't hit anything
		if (!raycastHit) 
		{
			return;
		}

		//true if raycast normal has the same direction than movement vector
		//Sign - return -1 if value is negative, 1 if value is possitive, 0 if vale is equal 0
		var isMovingDownSlope = Mathf.Sign (raycastHit.normal.x) == Mathf.Sign (deltaMovement.x);
		//it is not vertical slope - you go up the the slope, we don't moving down the slope
		if (!isMovingDownSlope) 
		{
			return;
		}

		//angle between normal and up vector. if the angle is nearly 0, the normal is perpendicular, so there is no slope
		var angle = Vector2.Angle (raycastHit.normal, Vector2.up);
		if (Mathf.Abs (angle) < .0001f)
		{
			return;
		}

		State.IsMovingDownSlope = true;
		State.SlopeAngle = angle;
		//we can't get any further down
		deltaMovement.y = raycastHit.point.y - slopeRayVector.y;

	}

	private bool HandleHorizontalSlope (ref Vector2 deltaMovement, float angle, bool isGoingRight)
	{
		//90 angle is not a slope is the wall
		if (Mathf.RoundToInt (angle) == 90)
		{
			return false;
		}

		//the slope is bigger than limit
		if (angle > Parameters.SlopeLimit) 
		{
			deltaMovement.x = 0;
			//we overriten deltaMovement so return true
			return true;
		}
		//there were some HandleVerticalSlope
		if (deltaMovement.y > .07f)
		{
			return true;
		}

		deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
		//movement based on the angle taht we moving up
		deltaMovement.y = Mathf.Abs ( Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
		State.IsMovingUpSlope = true;
		State.IsCollidingBelow = true;
		return true;
	}
	
	public void OnTriggerEnter2D (Collider2D other) 
	{

	}

	public void OnTriggerExit2D (Collider2D other) 
	{
		
	}
}
