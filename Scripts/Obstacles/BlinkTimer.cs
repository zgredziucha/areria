using UnityEngine;
using System.Collections;

public class BlinkTimer : LaserBlink {

	public float changeDelay = 2.0f;
	private bool _isChangeReady = true;
	public IEnumerator WaitToChange ()
	{
		_isChangeReady = false;
		yield return new WaitForSeconds (changeDelay);
		ChangeLaserApperance ();
		_isChangeReady = true;
		
	}
	
	public void Update ()
	{
		if (_isChangeReady) 
		{
			StartCoroutine("WaitToChange");
		}
	}
}
