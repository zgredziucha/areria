using UnityEngine;
using System.Collections;

public class ForceMovementByRaycast : MonoBehaviour {

	private Transform playerTransform; 
	private Vector3 _activeGlobalPlatformPoint,
					_activeLocalPlatformPoint;
	
	private Transform _transform;
	private Vector3 _centerPosition;
	private Vector2 _halfSize;
	private float  _distanceBetweenRays;
	
	public static bool IsBusy { get; set;}
	private bool _amIOwner;
	private int _raysCount = 7;
	private Vector2[] _rayOrigin;
	private Vector2 _rayDirection;

	private float rayLength = 0.1f;
	public LayerMask PlatformMask;
	
	public void Start ()
	{
		_rayOrigin = new Vector2 [_raysCount];
		IsBusy = false;
		_amIOwner = false;

		InitializeParams ();
	}


	public void Update ()
	{
		CalculateOrigin ();
		CastRays (); 
	}

	private void CastRays () 
	{
		if (!IsBusy || _amIOwner) 
		{	
			HandleForceMovement ();

			for (var i = 0; i < _raysCount; i++) 
			{
				Debug.DrawRay (_rayOrigin [i], rayLength * _rayDirection, Color.red);
				var raycastHit = Physics2D.Raycast (_rayOrigin [i], _rayDirection, rayLength, PlatformMask);
				
				if (raycastHit.collider != null && raycastHit.transform.gameObject.layer == LevelManager.PLAYER_LAYER) 
				{
					_amIOwner = true;
					IsBusy = true;
					playerTransform = raycastHit.transform.parent;
					if (playerTransform != null) 
					{
						
						_activeGlobalPlatformPoint = playerTransform.position;
						_activeLocalPlatformPoint = transform.InverseTransformPoint (playerTransform.position);
						//Debug.Log("ENTER " + gameObject.name);
					}
					return;
				}
			}
		}

		if (_amIOwner)
		{
			IsBusy = false;
			_amIOwner = false;
			playerTransform = null;
			//Debug.Log ("EXIT " + gameObject.name);
		}
		
	}

	private void InitializeParams ()
	{
		 _transform = transform;

		var _boxCollider = GetComponent<BoxCollider2D> ();
		var _localScale = _transform.localScale;
		var _center = new Vector3 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

		 _halfSize = new Vector2(_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		 _distanceBetweenRays = (_halfSize.x * 2) / (_raysCount - 1 );

		var angle = Mathf.Round (transform.eulerAngles.z / 90) * 90;

		_centerPosition = new Vector2 (- (Utils.Sin(angle) * _center.y),  (Utils.Cos(angle) * _center.y));
		_centerPosition = new Vector2 (_centerPosition.x + (Utils.Cos(angle) * _center.x), _centerPosition.y + (Utils.Sin(angle) * _center.x));
	}

	private void CalculateOrigin ()
	{
		var angle = Mathf.Round (transform.eulerAngles.z / 90) * 90;
		Vector2 center = _transform.position + _centerPosition;
		var bottomLeftPoint = new Vector2(center.x - _halfSize.x, center.y - _halfSize.y); 
	
		for (var i = 0; i < _raysCount; i++) 
		{
			var point =  new Vector2 (bottomLeftPoint.x + (i * _distanceBetweenRays), bottomLeftPoint.y);
			Vector2 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * (point - center); // get point direction relative to pivot, rotate it
			_rayOrigin[i] = dir + center; 
		}

		var directionAngle = angle + 270.0f; 
		_rayDirection = new Vector2 (Mathf.Cos(directionAngle * Mathf.Deg2Rad), Mathf.Sin(directionAngle * Mathf.Deg2Rad));
	}

	public void OnPlayerStay (Collider2D other) 
	{
		if (!IsBusy || _amIOwner) 
		{	
			HandleForceMovement();
			var player = other.GetComponent<PlayerController> ();
			if (player != null) 
			{	
				_amIOwner = true;
				IsBusy = true;
				playerTransform = other.transform;
				if (playerTransform != null) {
					
					_activeGlobalPlatformPoint = playerTransform.position;
					_activeLocalPlatformPoint = transform.InverseTransformPoint(playerTransform.position);
					//Debug.Log("ENTER " + gameObject.name);
				}
			}
		}
		
	}
	
	public void OnPlayerExit (Collider2D other) 
	{
		if (_amIOwner)
		{
			var player = other.GetComponent<PlayerController> ();
			if (player != null) 
			{
				IsBusy = false;
				_amIOwner = false;
				playerTransform = null;
				//Debug.Log ("EXIT " + gameObject.name);
			}
		}
		
	}
	
	
	private void HandleForceMovement()
	{
		if (playerTransform != null)
		{
			var newGlobalPlatformPoint = transform.TransformPoint(_activeLocalPlatformPoint);
			var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
			
			if (moveDistance != Vector3.zero)
			{
				playerTransform.Translate( moveDistance, Space.World);
			}
		}
		playerTransform = null;
	}
}
