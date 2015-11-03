using UnityEngine;
using System.Collections;

public class PistonController : MonoBehaviour {

	public KillingZone killingZone_01;
	public KillingZone killingZone_02;

	private bool[] triggersState = new bool[2];
	
	public float damage = 100;

	public void Start ()
	{
		ResetTriggersState ();
	}

	private void ResetTriggersState ()
	{
		triggersState [0] = false;
		triggersState [1] = false;
	}

	public void OnEnable () 
	{
		if (killingZone_01 != null && killingZone_02 != null)
		{
			killingZone_01.onChangeState += OnChangeTriggerState;
			killingZone_02.onChangeState += OnChangeTriggerState;
		}
	}

	public void OnDisable ()
	{
		if (killingZone_01 != null && killingZone_02 != null)
		{
			killingZone_01.onChangeState -= OnChangeTriggerState;
			killingZone_02.onChangeState -= OnChangeTriggerState;
		}
	}

	private bool CheckKillingAbility ()
	{
		return (triggersState[0] && triggersState[1]);
	}

	private void OnChangeTriggerState (bool isPlayerColliding, KillingZone owner)
	{
		var index = owner == killingZone_01 ? 0 : 1;
		triggersState[index] = isPlayerColliding;
		if (CheckKillingAbility ())
		{
			LevelManager.Instance.TakeDamage(damage);
			ResetTriggersState ();
		}

	}
}
