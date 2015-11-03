using UnityEngine;
using System.Collections;

public class KillingZoneActivator : MonoBehaviour, IAlarmSubscriber
{

    public bool isEnabledFromStart = false;
    public Observer alarmObserver;

    public KillingObsticle killingZoneMechanism;
    public BoxCollider2D killingZoneTrigger;
    public SpriteRenderer killingZoneRenderer;

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
