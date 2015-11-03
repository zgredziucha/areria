using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KillingRingActivator : MonoBehaviour, IAlarmSubscriber {
	
	public AlarmObserver alarmObserver;
	private List<SpriteRenderer> renderers = new List<SpriteRenderer> (); 
	private PolygonCollider2D trigger;

	public void Start ()
	{
		if ( alarmObserver != null) 
		{
			alarmObserver.Subscribe(this);
		}
		
		InitializeLists ();
	}
	
	private void InitializeLists ()
	{

		int children = transform.childCount;
		for (int i = 0; i < children; ++i)
		{
			var child = transform.GetChild(i);
			var renderer = child.GetComponent<SpriteRenderer>();
			var collider = child.GetComponent<PolygonCollider2D>();

			if (renderer != null)
			{
				renderers.Add(renderer);
			}
			if (collider != null)
			{
				trigger = collider;
			}
		}
	}
	
	public void TriggerBehaviour (bool isEnabled)
	{
		for (var i = 0; i < renderers.Count; i++)
		{
			renderers[i].enabled = isEnabled;
		}

		trigger.enabled = isEnabled;

        //if ( alarmObserver != null) 
        //{
        //    alarmObserver.UnSubscribe (this);
        //}
	}

    public void Reset ()
    {

    }
}