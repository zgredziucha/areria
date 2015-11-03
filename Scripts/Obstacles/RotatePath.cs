using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotatePath : MonoBehaviour {

	public MovementPath path;
//	public float speed;
//	public float maxTolerance;

	private IEnumerator<Transform> _currentPoint;
	private Transform _transform;

	public void Start () 
	{
		_transform = transform;
		_startAngle = _transform.eulerAngles.z;

		if (path != null) 
		{
			_currentPoint = path.GetPathEnumerator();
			_currentPoint.MoveNext();
			
			if (_currentPoint.Current == null)
			{
				return;
			}
		}

		
	}

	public void Update () 
	{
		if (_currentPoint == null || _currentPoint.Current == null) 
		{ 
			return;
		}
//
//		if (!_isRotationEnds)
//		{
//			RotateWhileSnapping (_angle);
//			return;
//		}

//		if (!_isChaising)
//		{
//			ChaisePoint ();
//		}
//		else 
//		{
//			Return();
//		}
	}

//	private void Return ()
//	{
//		_angle = -_angle;
//		_isRotationEnds = false;
//		_isChaising = false;
//	}

	private void ChaisePoint ()
	{
		_currentPoint.MoveNext();

		Vector3 dir = (_currentPoint.Current.position - _transform.position);
		var angleShift = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		_angle = angleShift;
//		var angleSing = Mathf.Sign (angleShift ); 
//		_angle = angleSing * (Mathf.Abs(angleShift) - _startAngle);
		_isRotationEnds = false;
		_isChaising = true;
	}

	private bool _isRotationEnds = true;
	private float _angle = 0;
	public float rotationTime = 2.0f;
	private float _rotated = 0;
	private bool _isChaising = false;
	private float _startAngle = 0;

	private void RotateWhileSnapping (float angle)
	{
		var rotationVector = new Vector3 (0, 0, angle);
		_rotated += Mathf.Abs(angle) * (Time.deltaTime / rotationTime);
		
		if (Mathf.Abs(angle) > _rotated )
		{
			transform.Rotate( rotationVector * (Time.deltaTime / rotationTime) );
		} 
		else 
		{
			transform.Rotate( 0, 0, _rotated - Mathf.Abs(angle));
			_rotated = 0;
			_isRotationEnds = true;
		}
	}

}
