using UnityEngine;
using System.Collections;

public class PortalSimpleController : MonoBehaviour {

	public Vector3 GetSpawnPoit () 
	{
		var _boxCollider = GetComponent<BoxCollider2D> ();
		var localScale = transform.localScale;
		
		var _offset = new Vector2 (_boxCollider.size.x * localScale.x, _boxCollider.size.y * Mathf.Abs (localScale.y)) / 2;
		var _center = transform.position + new Vector3 (_boxCollider.offset.x * localScale.x, _boxCollider.offset.y * localScale.y);
		var point = new Vector3 (_center.x + _offset.x, _center.y, 0);
		Vector3 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * (point - _center); 
		return dir + _center;
	}
}
