using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour {

	public enum FollowType
	{
		MoveTowards,
		Lerp
	}

	public FollowType Type = FollowType.MoveTowards;
	public MovementPath path;
	public float speed;
	public float maxDistance;

	public delegate void ChangingDirectionEvent ();
	public event ChangingDirectionEvent onChangeDirection;
	
	public void OnChangePoint()
	{
		if (onChangeDirection != null)
		{
			onChangeDirection();
		}
	}

	private IEnumerator<Transform> _currentPoint;

	public void Start () 
	{
		if (path != null) 
		{
			_currentPoint = path.GetPathEnumerator();
			_currentPoint.MoveNext();
			OnChangePoint();

			if (_currentPoint.Current == null)
			{
				return;
			}

			transform.position = _currentPoint.Current.position;
		}

	}

	public void Update () 
	{
		if (_currentPoint == null || _currentPoint.Current == null) 
		{ 
			return;
		}

		if (Type == FollowType.MoveTowards)
		{
			transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
		}
		else if (Type == FollowType.Lerp)
		{
			transform.position = Vector3.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
		}

		var distance = (transform.position - _currentPoint.Current.position).sqrMagnitude;
		if (distance < maxDistance * maxDistance)
		{
			_currentPoint.MoveNext();
			OnChangePoint();
		}
	}

}
