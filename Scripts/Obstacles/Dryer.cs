using UnityEngine;
using System.Collections;

public class Dryer : MonoBehaviour {

	private float _currentVelocity;
	private int _raysCount = 5;
	private Vector2[] _rayOrigin;
	private Vector2 _rayDirection;

	public float maxVelocity;
	public float rayLength;
	public LayerMask PlatformMask;
	public bool isPulling = false;

	public void Start ()
	{
		_rayOrigin = new Vector2 [_raysCount];
		CalculateOrigin ();
	}

	public void Update () {
		CastRays ();
	}
	
	private void CastRays () 
	{
		for (var i = 0; i < _raysCount; i++) 
		{
			Debug.DrawRay (_rayOrigin[i], rayLength * _rayDirection, Color.red);
			var raycastHit = Physics2D.Raycast(_rayOrigin[i], _rayDirection, rayLength, PlatformMask);
			
			if (raycastHit.collider != null && raycastHit.transform.gameObject.layer == LevelManager.PLAYER_LAYER)
			{
				CalculateCurrentVelocity((raycastHit.point - _rayOrigin[i]).magnitude);
				MoveTarget(raycastHit.transform);
				return;
			}
		}

	}

	private Vector2 GetDirection ()
	{
		var angle = (int)transform.eulerAngles.z;
		var sing = isPulling ? -1 : 1;
		switch (angle) 
		{
			case 0:
					return new Vector2 (0, -1 * sing);
					break;
			case 90:
					return new Vector2 (1 * sing, 0);
					break;
			case 180:
					return new Vector2 (0, 1 * sing);
					break;
			case 270:
				return new Vector2 (-1 * sing, 0);
					break;
			default :
					return _rayDirection;
					break;
		}
	}

    public float minVelocity = 0.1f;
	private void MoveTarget (Transform targetTransfom)
	{
        //var distance = Mathf.Clamp(maxVelocity - _currentVelocity, minVelocity, maxVelocity);
        var distance = maxVelocity - _currentVelocity;
        var moveDistance = distance * GetDirection();
		//var moveDistance = maxVelocity * GetDirection ();
        //if (_currentVelocity != 0)
        //{
        //     moveDistance /= _currentVelocity;
        //}

       

        Debug.Log(moveDistance);
		LevelManager.Instance.Player.ForceMovement (moveDistance);
		//targetTransfom.Translate( moveDistance, Space.World);
	}

	private void CalculateCurrentVelocity (float currentRayLength)
	{
		var percent = currentRayLength / rayLength;
        _currentVelocity = minVelocity + ((maxVelocity - minVelocity) * percent);
        //_currentVelocity = maxVelocity  * percent;
        //Debug.Log(_currentVelocity);
	}
	
	private void CalculateOrigin ()
	{
		var _transform = transform;
		var _boxCollider = GetComponent<BoxCollider2D> ();
		var _localScale = _transform.localScale;
		var _center = new Vector3 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		var _halfSize = new Vector2(_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;

		Vector2 centerPosition = _transform.position + _center;
		var bottomLeftPoint = new Vector2(centerPosition.x - _halfSize.x, centerPosition.y - _halfSize.y); 
		var _distanceBetweenRays = (_halfSize.x * 2) / (_raysCount - 1 );

		for (var i = 0; i < _raysCount; i++) 
		{
			var point =  new Vector2 (bottomLeftPoint.x + (i * _distanceBetweenRays), bottomLeftPoint.y);
			Vector2 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * (point - centerPosition); // get point direction relative to pivot, rotate it
			_rayOrigin[i] = dir + centerPosition; 
		}

		var angle = (transform.eulerAngles.z + 270.0f);
		angle = Mathf.Round (angle / 90) * 90;
		_rayDirection = new Vector2 (Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
	
	}
}
