using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour {

	private bool _isAggressive = false;

	public GameObject aggressiveParts;
	public GameObject friendlyParts;

	private IAlarmSubscriber aggressivePartsActivator;
	public IAlarmSubscriber friendlyPartsActivator;

	private float _startTime;
	public float changeTime = 5.0f;

	public EnergyRing energyRing;

	public void Start ()
	{
		if (aggressiveParts != null)
		{
			aggressivePartsActivator = (IAlarmSubscriber)aggressiveParts.GetComponent(typeof(IAlarmSubscriber));
		}

		if (friendlyParts != null)
		{
			friendlyPartsActivator = (IAlarmSubscriber)friendlyParts.GetComponent(typeof(IAlarmSubscriber));
		}

		_startTime = Time.time;

        LevelManager.onPlayerDied += Reset;

	}

    public void OnDestroy ()
    {
        LevelManager.onPlayerDied -= Reset;
    }

	public void OnEnable ()
	{
		energyRing.onDeactivate += OnEnargyEnd;
	}

	public void OnDisable ()
	{
		energyRing.onDeactivate -= OnEnargyEnd;
        //LevelManager.onPlayerDied += InitializeEnergy;
	}

	public void OnEnargyEnd ()
	{
		this.enabled = false;
	}

    public void Reset()
    {
        _startTime = Time.time;
        this.enabled = true;
    }

	public void Update () 
	{
		CountDown ();
	}

	private void SwitchState ()
	{
		_isAggressive = !_isAggressive;
		friendlyPartsActivator.TriggerBehaviour (!_isAggressive);
		aggressivePartsActivator.TriggerBehaviour (_isAggressive);
	}

	private void CountDown ()
	{
		if ( Time.time - _startTime > changeTime)
		{
			SwitchState ();
			_startTime = Time.time;
		}
	}




}
