using UnityEngine;
using System.Collections;

public class EnergyRing : MonoBehaviour {

	public SpriteRenderer OnTexture;
	public SpriteRenderer OffTexture;

	public Color MaxEnergyColor = new Color (255f, 255f, 255f, 255f);
	public Color MinEnergyColor = new Color(255f, 255f, 255f, 0f);

	public bool isActive { get; private set; }

	public float maxEnergyValue = 20;
    private float _energyTaken;

	private float _currentEnergyValue;
	private bool _isOverlapFirstTime = true;

	public delegate void DeactivateEvent ();
	public event DeactivateEvent onDeactivate;
	public event DeactivateEvent onActivate;

	public bool isAssasin = false;
    private bool _isAssasinVisible = false;

    public float EnergyTaken { get {
        var taken = _energyTaken - _currentEnergyValue;
        _energyTaken = _currentEnergyValue;
        return taken;
    } }
    

    //public float GetEnergyTaken ()
    //{
    //    var taken = _energyTaken - _currentEnergyValue;
    //    _energyTaken = _currentEnergyValue;
    //    return taken;
    //}

    public void OnEnable ()
    {
        CharactersController.onSwitch += ResolveApperance;
        Character.onNewAbility += ChangeRingApperacne;
    }

    public void OnDisable()
    {
        CharactersController.onSwitch -= ResolveApperance;
        Character.onNewAbility -= ChangeRingApperacne;
    }

    public void ResolveApperance (Character currentCharacter)
    {
        if (isAssasin)
        {
            _isAssasinVisible = currentCharacter.seeAssasins;
            if (_isAssasinVisible)
            {
                ChangeRingApperacne();
            }
            else 
            {
                Hide();
            }
        }
        
    }

	public void Start ()
	{
		Reset ();
        if (isAssasin && !_isAssasinVisible)
        {
            Hide();
        }
	}

    

	public void GiveEnergy () 
	{
		float energyValue = maxEnergyValue / 100;
		if (_currentEnergyValue < 0) 
		{
			Deactivate();
			return;
		}

		_currentEnergyValue -= energyValue;
		LevelManager.Instance.AddEnergy (energyValue);
		ChangeRingApperacne();
		if (_isOverlapFirstTime)
		{
			LevelManager.Instance.currentCheckPoint.SubscribeEnergyRing(this);
			_isOverlapFirstTime = false;
		}
	}

	public void Activate ()
	{
		Reset ();
		ChangeRingApperacne ();
		if(isAssasin && !_isAssasinVisible)
		{
			Hide();
		}
		if (onActivate != null)
		{
			onActivate();
		}
	}

	private void Reset ()
	{
		_isOverlapFirstTime = true;
		_currentEnergyValue = maxEnergyValue;
        _energyTaken = maxEnergyValue;
		isActive = true;
	}

	private void Hide ()
	{
		OnTexture.color =  new Color(255f, 255f, 255f, 0);
	}

	public void Deactivate ()
	{ 
		isActive = false;
		if (onDeactivate != null)
		{
			onDeactivate();
		}
	}

	public void ChangeRingApperacne ()
	{
		var energyPercent = _currentEnergyValue / maxEnergyValue;
		OnTexture.color =  new Color(255f, 255f, 255f, energyPercent);
		//OnTexture.color = Color.Lerp (MinEnergyColor, MaxEnergyColor, energyPercent);
	}

//	private float blinkingPointer = 1;
//	private float startTime = 0;
//	public float blinkTime = 3;
//
//	private void SetBlinkingPointer (float maxValue)
//	{
//		var deltaTime = Time.time - startTime;
//		blinkingPointer *= deltaTime * 3;
//		if (deltaTime > blinkTime)
//		{
//			startTime = Time.time;
//			blinkingPointer = 1;
//		}
//
//	}
//	public void Update ()
//	{
//		var maxeEergyPercent = _currentEnergyValue / maxEnergyValue;
//	    SetBlinkingPointer (maxeEergyPercent);
//		OnTexture.color = new Color (255f, 255f, 255f, maxeEergyPercent/blinkingPointer);
//	}


}
