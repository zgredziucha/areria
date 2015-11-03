using UnityEngine;
using System.Collections;

public class PistonActivator : MonoBehaviour, IAlarmSubscriber {
	
	public AlarmObserver alarmObserver;
	public PistonController pistonController;
	public Piston pistonUp;
	public Piston pistonDown;
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
		if (pistonController != null) 
		{
			pistonController.enabled = isEnabled;
		}
		if (pistonUp != null) 
		{
            HandlePiston(isEnabled, pistonUp);
		}
		if (pistonDown != null) 
		{
			HandlePiston(isEnabled, pistonDown);
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

    public void HandlePiston(bool isEnabled, Piston piston)
    {
        if (isEnabled)
        {
            piston.enabled = isEnabled;
        }
        else
        {
            piston.StopWorking();
        }
    }
	
}
