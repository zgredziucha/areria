using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Observer : MonoBehaviour {

    private List<IAlarmSubscriber> subscribers = new List<IAlarmSubscriber>();
    public bool doTurnOn = true;

    public virtual void OnEnable()
    {
        //LevelManager.onPlayerDied += StartWorking;
    }

    public virtual void OnDisable()
    {
        //LevelManager.onPlayerDied -= StartWorking;
    }


    public void Subscribe(IAlarmSubscriber subscriber)
    {
        if (!subscribers.Contains(subscriber))
        {
            subscribers.Add(subscriber);
        }
    }

    public void Broadcast()
    {
        StopWorking();
        for (var i = 0; i < subscribers.Count; i++)
        {
            subscribers[i].TriggerBehaviour(doTurnOn);
        }
    }

    public virtual void StopWorking()
    {
        LevelManager.Instance.currentCheckPoint.SubscribeObserver(this);
    }

    public virtual void StartWorking ()
    {
        for (var i = 0; i < subscribers.Count; i++)
        {
            subscribers[i].Reset();
        }
    }

    //public void UnSubscribe (IAlarmSubscriber subscriber)
    //{
    //    if (subscribers.Contains(subscriber))
    //    {
    //        subscribers.Remove (subscriber);
    //    }
    //}
}
