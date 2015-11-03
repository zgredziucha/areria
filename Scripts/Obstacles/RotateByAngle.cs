using UnityEngine;
using System.Collections;

public class RotateByAngle : MonoBehaviour {
	
	public float angleShift;

	private Transform _transform;
	private bool _isRotationEnds = true;
	private float _angle = 0;
	public float rotationTime = 2.0f;
	private float _rotated = 0;
	private bool _isChaising = false;

	public void Start () 
	{
		_transform = transform;
		_angle = angleShift;

	}
	
	public void Update () 
	{
		if (!_isRotationEnds)
		{
			RotateWhileSnapping (_angle);
			return;
		}

		if (!_isChaising)
		{
			ChaisePoint ();
		}
		else 
		{
			Return();
		}
	}
	
	private void Return ()
	{
		_angle = -_angle;
		_isRotationEnds = false;
		_isChaising = false;
	}
	
	private void ChaisePoint ()
	{
		_isRotationEnds = false;
		_isChaising = true;
	}
	
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
			//transform.Rotate( 0, 0, _rotated - Mathf.Abs(angle));
			_rotated = 0;
			_isRotationEnds = true;
		}
	}
}
