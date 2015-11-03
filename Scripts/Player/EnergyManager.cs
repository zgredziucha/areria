using UnityEngine;
using System.Collections;

public class EnergyManager : MonoBehaviour {
   
    public float Points { get; private set; }
	public float Energy { get; private set; }
	public float maxEnergyVolume = 100;

	public float noMovementEneryWaistingValue = 0.1f;
	public float _currentEnergyWaistingValue = 0.1f;

	private float waistingDelay = 0.5f;
	private bool isWaistingReady = true;

	public delegate void ChangingEnergyValue (float energy);

	public static event ChangingEnergyValue onEnergyChanged;
    public static event ChangingEnergyValue onPointsChanged;
    
    public static void OnPointsChanged(float points)
    {
        if (onPointsChanged != null)
        {
            onPointsChanged(points);
        }
    }

	public static void OnEnergyChanged(float energy)
	{
		if (onEnergyChanged != null)
		{
			onEnergyChanged(energy);
		}
	}

	public void OnEnable ()
	{
		PlayerMovement.onPlayerMove += ChangeEnergyWastingValue;
	}
	public void OnDisable ()
	{
		PlayerMovement.onPlayerMove -= ChangeEnergyWastingValue;
	}

    public void ChangeEnergyWastingValue(float currentMultipier, float velocity)
    {
		_currentEnergyWaistingValue = currentMultipier * noMovementEneryWaistingValue * velocity;
	}

    //public void ChangeEnergyWastingValue(MoveMode movingState, float velocity)
    //{
    //    float currentMultipier = 1;
    //    if (movingState == MoveMode.Flying)
    //    {
    //        currentMultipier = flyMultiplier;
    //    }
    //    else if (movingState == MoveMode.Walking)
    //    {
    //        currentMultipier = walkMultiplier;
    //    }

    //    _currentEnergyWaistingValue = currentMultipier * noMovementEneryWaistingValue * velocity;

    //}



	void Update () {
		if (isWaistingReady)
		{
			StartCoroutine("WaistEnergy");
		}
	}

	public IEnumerator WaistEnergy ()
	{
		isWaistingReady = false;
		yield return new WaitForSeconds (waistingDelay);

		SubstractEnergy (_currentEnergyWaistingValue, notify: false);
		isWaistingReady = true;
	}

	public void Start() 
	{
		Energy = maxEnergyVolume;
        Points = 0;
		OnEnergyChanged(Energy);
	}

	public void AddEnergy (float value) 
	{
        TryAddPoints(value);
		Energy = Mathf.Clamp (value + Energy, 0, maxEnergyVolume);
		OnEnergyChanged(Energy);
	}

    public void SubstractEnergy(float value, bool notify = true)
    {
        Energy = Mathf.Clamp(Energy - value, 0, maxEnergyVolume);
        if (notify)
        {
            //int value = (int)energyRing.EnergyTaken;
            DisplayText(-(int)value);
        }
        
        OnEnergyChanged(Energy);
    }

    public void DisplayText (float value)
    {
        string style = "SubstractEnergy";
        if (value > 0)
        {
            style = "AddEnergy";
        }
         FloatingText.Show(string.Format("{0}", value), style, new FromWorldPointTextPositioner(Camera.main, transform.position, 50, 1.5f));
        //}
    }

    public void TryAddPoints (float value)
    {
        var extraEnergy = maxEnergyVolume - (value + Energy);

        if (extraEnergy < 0)
        {
            Points += Mathf.Abs(extraEnergy);
            OnPointsChanged(Points);
        }
    }

    public void ResetPoints ()
    {
        Points = .0f;
        OnPointsChanged(Points);
    }

	public bool IfEnergyWillKill (float energy)
	{
		return Energy - energy <= 0;
	}

	public void SetMax ()
	{
		Energy = maxEnergyVolume;
		OnEnergyChanged(Energy);
	}
}
