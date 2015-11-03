using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlarmObserver : Observer {

	public LaserBeam laserBeam;
    public RotateByAngle rotator;
    
	public void OnEnable()
	{
		laserBeam.onPlayerDetect += Broadcast;
        base.OnEnable();
	}
	
	public void OnDisable ()
	{
		laserBeam.onPlayerDetect -= Broadcast;
        base.OnDisable();
	}

	public override void StopWorking ()
    {
        base.StopWorking();
        laserBeam.ChangeLaserBeamApperance(.0f);
        laserBeam.enabled = false;
        rotator.enabled = false;
    }

    public override void StartWorking()
    {
        base.StartWorking();
        laserBeam.enabled = true;
        rotator.enabled = true;
    }



}
