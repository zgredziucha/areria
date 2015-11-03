using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	public Vector2 margin;		
	public Vector2 smooth;		

	public Transform player;		
	public BoxCollider2D cameraBounds;
	private Vector2 _maxBound, _minBound;


	public bool IsFollowing { get; set; }

	public void Start () 
	{
		IsFollowing = true;
		_maxBound = cameraBounds.bounds.max;
		_minBound = cameraBounds.bounds.min;
	}

	public void FixedUpdate ()
	{
		if (IsFollowing)
		{
			TrackPlayer();
		}
	}
	
	
	void TrackPlayer ()
	{
		float targetX = transform.position.x;
		float targetY = transform.position.y;

		if(Mathf.Abs(targetX - player.position.x) > margin.x)
		{
			targetX = Mathf.Lerp(targetX, player.position.x, smooth.x * Time.deltaTime);
		}

		if(Mathf.Abs(targetY - player.position.y) > margin.y)
		{
			targetY = Mathf.Lerp(targetY, player.position.y, smooth.y * Time.deltaTime);
		}

		//ortographics size is half a size of y dimesion
		var cameraHalfWidth = GetComponent<Camera>().orthographicSize * ((float) Screen.width / Screen.height);

		targetX = Mathf.Clamp(targetX, _minBound.x + cameraHalfWidth, _maxBound.x - cameraHalfWidth);
		targetY = Mathf.Clamp(targetY,_minBound.y + GetComponent<Camera>().orthographicSize, _maxBound.y - GetComponent<Camera>().orthographicSize);

		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}
}
