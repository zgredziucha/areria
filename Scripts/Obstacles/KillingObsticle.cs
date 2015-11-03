using UnityEngine;
using System.Collections;

public class KillingObsticle : MonoBehaviour {

	public float damage = 100;
	public void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.CompareTag ("Player")) 
		{
			LevelManager.Instance.TakeDamage(damage);
		}
	}
}
