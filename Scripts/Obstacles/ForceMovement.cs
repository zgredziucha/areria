using UnityEngine;
using System.Collections;

public class ForceMovement : MonoBehaviour {

	private Transform playerTransform; 
	private Vector3 _activeGlobalPlatformPoint,
	_activeLocalPlatformPoint;

	public static bool IsBusy { get; set;}
	private bool _amIOwner;

	public void Start ()
	{
		IsBusy = false;
		_amIOwner = false;
	}
	public void OnTriggerStay2D (Collider2D other) 
	{
		if (!IsBusy || _amIOwner) 
		{	
			HandleForceMovement();
			if (other.CompareTag ("Player")) 
			{
				_amIOwner = true;
				IsBusy = true;
				playerTransform = other.transform.parent;
				if (playerTransform != null) {
					
					_activeGlobalPlatformPoint = playerTransform.position;
					_activeLocalPlatformPoint = transform.InverseTransformPoint(playerTransform.position);
					//Debug.Log("ENTER " + gameObject.name);
				}
			}
		}

	}

	public void OnTriggerExit2D (Collider2D other) 
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
