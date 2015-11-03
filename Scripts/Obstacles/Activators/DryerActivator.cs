using UnityEngine;
using System.Collections;

public class DryerActivator : MonoBehaviour, IAlarmSubscriber {

	public Observer alarmObserver;
	public Dryer dryer;
	public KillingObsticle killingZoneMechanism;
	public BoxCollider2D killingZoneTrigger;
	public SpriteRenderer killingZoneRenderer;

	public bool isEnabledFromStart = false;

	public void Start ()
	{
		if ( alarmObserver != null) 
		{
			alarmObserver.Subscribe(this);
		}
		ChangeComponentsAbility (isEnabledFromStart);
	}

	private void ChangeComponentsAbility (bool isEnabled)
	{
		if (dryer != null) 
		{
			dryer.enabled = isEnabled;
		}
		if (killingZoneMechanism != null) 
		{
			killingZoneMechanism.enabled = isEnabled;
		}
		if (killingZoneTrigger != null) 
		{
			killingZoneTrigger.enabled = isEnabled;
		}
		if (killingZoneRenderer != null) 
		{
			killingZoneRenderer.enabled = isEnabled;
		}
	}

	public void TriggerBehaviour (bool isEnabled)
	{
		ChangeComponentsAbility (isEnabled);
		//alarmObserver.UnSubscribe (this);
	}

    public void Reset ()
    {
        ChangeComponentsAbility(isEnabledFromStart);
    }
}
