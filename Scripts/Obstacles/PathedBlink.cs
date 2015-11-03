using UnityEngine;
using System.Collections;

public class PathedBlink : LaserBlink {

	public FollowPath pathMove;

	public void OnEnable()
	{
		pathMove.onChangeDirection += ChangeLaserApperance;
	}
	
	public void OnDisable ()
	{
		pathMove.onChangeDirection -= ChangeLaserApperance;
	}
}
