using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour {
	private Transform _transform;
	private Vector2 _rayOrigin;
	private BoxCollider2D _boxCollider;
	private float _halfSize;
	private Vector3 _localScale;
	private Vector3 _center;


	public LayerMask PlatformMask;
	public float rayLength;
	public Transform ForegroundSprite;
	public int additionalAngles = 0;

//	public SpriteRenderer laserRenderer;
//	public BoxCollider2D laserCollider;

	void Start () {
		_transform = transform;
		
		_boxCollider = GetComponent<BoxCollider2D> ();
		_localScale = _transform.localScale;
		_center = new Vector3 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		_halfSize = (_boxCollider.size.x * Mathf.Abs (_localScale.x)) / 2;

	}
	
//	public void TurnOff () 
//	{
//		if (laserRenderer != null && laserCollider != null)
//		{
//			laserRenderer.enabled = false;
//			laserCollider.enabled = false;
//		}
//	}
//	
//	public void TurnOn () 
//	{
//		if (laserRenderer != null && laserCollider != null)
//		{
//			laserRenderer.enabled = true;
//			laserCollider.enabled = true;
//		}
//	}

	public delegate void PlayerDetectEvent ();
	public event PlayerDetectEvent onPlayerDetect;
	
	public void OnPlayerDetect()
	{
		if (onPlayerDetect != null)
		{
			onPlayerDetect();
		}
	}


	public void ChangeLaserBeamApperance (float beamLength)
	{
		var percent = beamLength / rayLength;
		ForegroundSprite.localScale = new Vector3 (percent, 1, 1);
	}

	void Update () {
		CalculateOrigin ();
		CastRays ();
	}

	private void CastRays () 
	{
		var rayDistance = rayLength;
		var angle = (transform.eulerAngles.z + additionalAngles) * Mathf.Deg2Rad;
		var rayDirection = new Vector2 (Mathf.Cos(angle), Mathf.Sin(angle));

		Debug.DrawRay (_rayOrigin, rayDistance * rayDirection, Color.red);
		var raycastHit = Physics2D.Raycast(_rayOrigin, rayDirection, rayDistance, PlatformMask);

		if (raycastHit.collider != null)
		{
			var beamLength = (raycastHit.point - _rayOrigin).magnitude;
			ChangeLaserBeamApperance(beamLength);

			if (raycastHit.transform.gameObject.layer == LevelManager.PLAYER_LAYER)
			{
				//Debug.Log("PLAYER");
				OnPlayerDetect();
			}
		} 
		else 
		{
			ChangeLaserBeamApperance(rayLength);
		}
	}
	
	private void CalculateOrigin ()
	{
        var offset = 0.005f;
		Vector2 centerPosition = _transform.position + _center;
		var point = new Vector2(centerPosition.x + _halfSize + offset, centerPosition.y); 
		Vector2 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z + additionalAngles) * (point - centerPosition); // get point direction relative to pivot, rotate it
		_rayOrigin = dir + centerPosition; 
	}
}
