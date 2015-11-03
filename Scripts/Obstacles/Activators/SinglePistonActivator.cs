using UnityEngine;
using System.Collections;

public class SinglePistonActivator : MonoBehaviour, IAlarmSubscriber  {

    public AlarmObserver alarmObserver;
    public Piston piston;
    public bool isEnabledFromStart = false;

    public void Start()
    {
        if (alarmObserver != null)
        {
            alarmObserver.Subscribe(this);
        }
        ChangeComponentsAbility(isEnabledFromStart);
    }

    private void ChangeComponentsAbility(bool isEnabled)
    {
        if (piston != null)
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

    public void TriggerBehaviour(bool isEnabled)
    {
        ChangeComponentsAbility(isEnabled);
        //alarmObserver.UnSubscribe (this);
    }

    public void Reset()
    {
        ChangeComponentsAbility(isEnabledFromStart);
    }
	
}
