using UnityEngine;
using System.Collections;

public class ChaisingTarget: MonoBehaviour {

private Vector2 _rayOrigin;
	
    private BoxCollider2D _boxCollider;
	private float _halfSize;
	private Vector3 _localScale;
	private Vector3 _center;
    private Transform _transform;

	public LayerMask PlatformMask;
	public float rayLength = 2;
	public int additionalAngles = 0;

    void Start () {
		_transform = transform;
		_boxCollider = GetComponent<BoxCollider2D> ();
		_localScale = _transform.localScale;
		_center = new Vector3 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		_halfSize = (_boxCollider.size.x * Mathf.Abs (_localScale.x)) / 2;
        isChasing = false;
	}


    public delegate void PlayerDetectEvent(Transform playerTransform);
	public event PlayerDetectEvent onPlayerDetect;
	
	public void OnPlayerDetect(Transform playerTransform)
	{
		if (onPlayerDetect != null)
		{
			onPlayerDetect(playerTransform);
		}
	}

    public Transform Target { get; set; }
    public bool isChasing { get; set; }

    void Update () {
        if (isChasing)
        {
            ChaisePoint(Target);
            CalculateOrigin();
            CastRays();
        }
    }


    private void ChaisePoint (Transform target)
	{
        var newRotation = Quaternion.LookRotation(transform.position - target.position, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
	}


	private void CastRays () 
	{
		var rayDistance = rayLength;
		var angle = (transform.eulerAngles.z + additionalAngles) * Mathf.Deg2Rad;
		var rayDirection = new Vector2 (Mathf.Cos(angle), Mathf.Sin(angle));

		Debug.DrawRay (_rayOrigin, rayDistance * rayDirection, Color.red);
		var raycastHit = Physics2D.Raycast(_rayOrigin, rayDirection, rayDistance, PlatformMask);

		if (raycastHit.collider != null && raycastHit.transform.gameObject.layer == LevelManager.PLAYER_LAYER)
		{
            OnPlayerDetect(raycastHit.transform);
            isChasing = false;
		} 
	
	}
	
	private void CalculateOrigin ()
	{
		Vector2 centerPosition = _transform.position + _center;
		var point = new Vector2(centerPosition.x + _halfSize, centerPosition.y); 
		Vector2 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z + additionalAngles) * (point - centerPosition); 
		_rayOrigin = dir + centerPosition; 
	}
}

