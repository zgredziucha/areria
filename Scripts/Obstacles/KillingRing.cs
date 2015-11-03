using UnityEngine;
using System.Collections;

public class KillingRing : MonoBehaviour {

	public float energySpeed = 20;
	
	public void TakeDamage () 
	{
		float energyValue = energySpeed / 100;
		LevelManager.Instance.TakeDamage (energyValue);
	}

	public void OnTriggerStay2D (Collider2D other) 
	{
		if (other.CompareTag ("Player")) 
		{
			TakeDamage ();
		}
	}
	
}
