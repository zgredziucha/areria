using UnityEngine;
using System.Collections;

public class LaserActivator : MonoBehaviour, IAlarmSubscriber {

	public AlarmObserver alarmObserver;
	public FollowPath followPath;
	public LaserBeam laserBeam;
	public BoxCollider2D laserCollider;
	public SpriteRenderer laserRenderer;
	public LaserBlink laserBlink;

    public bool isEnabledFromStart = false;

	public void Start ()
	{
		if ( alarmObserver != null) 
		{
			alarmObserver.Subscribe(this);
		}
        ChangeComponentsAbility(isEnabledFromStart);
	}

    private void ChangeComponentsAbility(bool isEnabled)
    {
        if (followPath != null)
        {
            followPath.enabled = isEnabled;
        }
        if (laserBeam != null)
        {
            laserBeam.enabled = isEnabled;
        }
        if (laserCollider != null)
        {
            laserCollider.enabled = isEnabled;
        }
        if (laserRenderer != null)
        {
            laserRenderer.enabled = isEnabled;
        }
        if (laserBlink != null)
        {
            laserBlink.enabled = isEnabled;
        }
    }

	public void TriggerBehaviour (bool isEnabled)
	{
        ChangeComponentsAbility(isEnabled);
		//alarmObserver.UnSubscribe (this);
	}

    public void Reset ()
    {
        ChangeComponentsAbility(isEnabledFromStart);
    }

}
