using UnityEngine;
using System.Collections;

public class EnergyPath : MonoBehaviour {

	public EnergyRing energyRing;

    private Collider2D _collider;

	public void OnTriggerStay2D (Collider2D other) 
	{
		if (energyRing.isActive && other.CompareTag ("Player")) 
		{
			energyRing.GiveEnergy (); 
		}
	}

    public void OnTriggerExit2D(Collider2D other)
    {
        NotifyAboutTakenEnergy();
    }

    private void NotifyAboutTakenEnergy ()
    {
        int value = (int)energyRing.EnergyTaken;
        if (value > 0)
        {
            LevelManager.Instance.Energy.DisplayText(value);
        }
    }

    private void TurnOn ()
    {
        _collider.enabled = true;
    }

    private void TurnOff ()
    {
        _collider.enabled = false;
        NotifyAboutTakenEnergy();
    }

    public void OnEnable()
    {
        energyRing.onDeactivate += TurnOff;
        energyRing.onActivate+= TurnOn;
    }

    private void OnDisable()
    {
        energyRing.onDeactivate -= TurnOff;
        energyRing.onActivate -= TurnOn;
    }

    public void Start ()
    {
        _collider = GetComponent<Collider2D>();
    }

}
