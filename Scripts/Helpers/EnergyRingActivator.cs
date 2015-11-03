using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergyRingActivator : MonoBehaviour, IAlarmSubscriber {

	public AlarmObserver alarmObserver;
	public Transform paths;
	private List<SpriteRenderer> renderers = new List<SpriteRenderer> (); 
	private List<BoxCollider2D> energyPaths = new List<BoxCollider2D> (); 

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

			if (renderer != null)
			{
				renderers.Add(renderer);
			}
		}

		children = paths.childCount;
		for (int i = 0; i < children; ++i)
		{
			var child = paths.GetChild(i);
			var renderer = child.GetComponent<SpriteRenderer>();
			var energyPath = child.GetComponent<BoxCollider2D>();
			if (renderer != null)
			{
				renderers.Add(renderer);
			}
			if (energyPath != null)
			{
				energyPaths.Add(energyPath);
			}
		}
	}

	public void TriggerBehaviour (bool isEnabled)
	{
		for (var i = 0; i < renderers.Count; i++)
		{
			renderers[i].enabled = isEnabled;
		}
		for (var i = 0; i < energyPaths.Count; i++)
		{
			energyPaths[i].enabled = isEnabled;
		}
        //if ( alarmObserver != null) 
        //{
        //    alarmObserver.UnSubscribe (this);
        //}
	}

    public void Reset ()
    {
        //TriggerBehaviour();
    }
}
