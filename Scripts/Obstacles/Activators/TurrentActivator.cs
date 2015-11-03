using UnityEngine;
using System.Collections;

public class TurrentActivator : MonoBehaviour, IAlarmSubscriber
{

    public Observer alarmObserver;
    public BoxCollider2D trigger;
    public StandardTurret turret;

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
        if (turret != null)
        {
            turret.enabled = isEnabled;
        }
        if (trigger != null)
        {
            trigger.enabled = isEnabled;
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
