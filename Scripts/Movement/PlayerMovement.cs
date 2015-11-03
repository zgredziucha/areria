using UnityEngine;
using System.Collections;

//public enum MovingDirection {
//	None = 0,
//	Forward = 1,
//	Backward = -1
//}

public class PlayerMovement : MonoBehaviour {

	protected Transform _transform;
	protected Vector3 _localScale;
	protected BoxCollider2D _boxCollider;
	protected Vector2 _center;
	protected Vector2 _halfSize;
	
	protected int TotalHorizontalRays = 4;
	protected int TotalVerticalRays = 8;
	protected float _verticalDistanceBetweenRays;
	protected float _horizontalDistanceBetweenRays;
	
	protected Vector2 _velocity = new Vector2(0, 0);
	protected VectorDirection _direction = VectorDirection.None;
	protected bool _isFacingForward;
    protected bool _isMovingEnabled = true;
    protected Vector2 forceMovementVector = Vector2.zero;
	
	public MovingParameters DefaultParameters;
	protected MovingParameters Parameters { get { return DefaultParameters; } }
	protected const float SkinWidth = .02f;
	public MovingState State { get; protected set; }

	protected Vector2 bottomRightPoint; 
	protected Vector2 bottomLeftPoint; 
	protected Vector2 topLeftPoint;

    public Gravity gravity;

	public SkeletonAnimation skeletonAnimator;
    protected string _currentAnimation = "idle";
   
    protected Hashtable _energyValue;
    public EnergyValues energyValues;

	public delegate void MovingEvent (float energyMultiplier, float velocity);
	public static event MovingEvent onPlayerMove;

    public static void OnMoving(float energyMultiplier, float velocity)
	{
		if (onPlayerMove != null)
		{
            onPlayerMove(energyMultiplier, velocity);
		}
	}

	protected void SetAniamation (string name, bool loop)
	{
		if (name == _currentAnimation)
		{
			return;
		}
		
		skeletonAnimator.state.SetAnimation(0, name, loop);
		_currentAnimation = name;
	}

    protected void InitializeEnergyValues()
    {
        _energyValue = new Hashtable();
        for (var i = 0; i < energyValues.names.Count; i++)
        {
            _energyValue.Add(energyValues.names[i], energyValues.values[i]);
        }
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
	
	protected void ForceMovement (Vector2 movementVector)
	{
		forceMovementVector = movementVector;
	}
	
	protected void ResolveMovement (bool isMovementExpected)
	{
		if (isMovementExpected)
		{
			EnableMovement();
			return;
		}
		DisableMovement ();
	}
	
	protected void DisableMovement ()
	{
		ResetMovement ();
		_isMovingEnabled = false;
	}
	
	protected virtual void EnableMovement ()
	{
        gravity.ResetGravity(false);
		Vector2 deltaMovement = new Vector2 (0, -0.01f);
		RaycastHit2D raycastHit = CalculateCollisions (ref deltaMovement, false);
		if (raycastHit != null)
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
	
	public virtual void ResetMovement () 
	{
        gravity.ResetGravity(false);
		_velocity = new Vector2 (0, 0);
	}
	
	protected virtual void OnPlayerInput (VectorDirection direction) {
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

	public virtual bool IsGrounded () {
		return State.MovingMode == MoveMode.Walking;
	}
	
	protected void Flip () 
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingForward = transform.localScale.x > 0;
	}

    public void SetFacingDirection(float localScale)
    {
        var sign = Mathf.Sign(localScale);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * sign, transform.localScale.y, transform.localScale.z);
		_isFacingForward = sign > 0;
    }
	
	public void SetHorizontalForce (float x) 
	{
		_velocity.x = x;
	}
	
	public void SetVerticalForce (float y) 
	{
		_velocity.y = y;
	}
	
	protected void ResetForces ()
	{
		_velocity.x = 0;
		_velocity.y = 0;
	}

	protected RaycastHit2D CastRays (ref float deltaMovement, int raysCount, Vector2 startPoint, Vector2 pointDistance, bool isHorizontal, out Vector2 rayOrigin, int sign = 1, float angleShift = 0, bool skipFirst = false) 
	{
		var rayDistance = Mathf.Abs (deltaMovement) + SkinWidth;
		var angle = (Utils.Round90(transform.eulerAngles.z) - angleShift) * Mathf.Deg2Rad;
		var rayDirection = new Vector2 (sign * Mathf.Cos(angle), sign * Mathf.Sin(angle));
		
		RaycastHit2D rayCastHit = Physics2D.Raycast(_center, rayDirection, 0.00001f, Parameters.PlatformMask);
		rayOrigin = Vector2.zero;
		
		for (var i = 0; i < raysCount; i++) 
		{
			if (skipFirst && i == 0)
			{
				continue;
			}
			
			var point = new Vector2 (startPoint.x + (pointDistance.x * i), startPoint.y + (pointDistance.y * i));
			rayOrigin = CalculateRotatedPoint (point);
			
			Debug.DrawRay (rayOrigin, rayDistance * rayDirection, Color.red);
            rayCastHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, Parameters.PlatformMask);
			
			if  (!rayCastHit) 
			{
				continue;
			}
			return rayCastHit;
		}
		return rayCastHit;
	}

	
	protected RaycastHit2D CalculateCollisions (ref Vector2 deltaMovement, bool isHorizontalMovement) {
		Vector2 rayOrigin;
		RaycastHit2D raycastHit;
		if (isHorizontalMovement)
		{
			var verticalRayDistance = new Vector2 (0, _verticalDistanceBetweenRays);
			var isMovingRight = deltaMovement.x > 0;
			var verticalRayOrigin = isMovingRight ? bottomRightPoint : bottomLeftPoint;
			var verticalSign = isMovingRight ? 1 : -1;
			raycastHit = CastRays (ref deltaMovement.x, TotalVerticalRays, verticalRayOrigin, verticalRayDistance, isHorizontalMovement, out rayOrigin, sign: verticalSign, angleShift: 0, skipFirst: true);
			if (raycastHit.collider != null)
			{
				deltaMovement.x = 0;
			}
		}
		else 
		{
			var horizontalRayDistance = new Vector2 (_horizontalDistanceBetweenRays, 0);
			var isMovingUp = deltaMovement.y < 0;
			var horizontalRayOrigin = isMovingUp ?  bottomLeftPoint : topLeftPoint;
			var horizontalSign = isMovingUp ? 1 : -1;
			
			raycastHit = CastRays (ref deltaMovement.y, TotalHorizontalRays, horizontalRayOrigin, horizontalRayDistance, isHorizontalMovement, out rayOrigin, sign: horizontalSign, angleShift: 90.0f);
			if (raycastHit.collider != null)
			{
				deltaMovement.y = 0;
			}
		}
		return raycastHit;
	}
	
	protected void CalculateOrigins ()
	{
		_center = _transform.position + new Vector3 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		bottomRightPoint = new Vector2 (_center.x + _halfSize.x - SkinWidth, _center.y - _halfSize.y + SkinWidth); 
		bottomLeftPoint = new Vector2 (_center.x - _halfSize.x + SkinWidth, _center.y - _halfSize.y + SkinWidth);
		topLeftPoint = new Vector2 (_center.x - _halfSize.x + SkinWidth, _center.y + _halfSize.y - SkinWidth);
	}
	
	protected Vector2 CalculateRotatedPoint (Vector2 point)
	{
		Vector2 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * (point - _center); 
		return dir + _center; 
	}
	
	protected void CalculateDistanceBetweenRays () 
	{
		var colliderWidth = _boxCollider.size.x * Mathf.Abs (transform.localScale.x) - (2 * SkinWidth);
		_horizontalDistanceBetweenRays = colliderWidth / (TotalHorizontalRays - 1 );
		
		var colliderHeight = _boxCollider.size.y * Mathf.Abs (transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (TotalVerticalRays - 1);
	}
}
