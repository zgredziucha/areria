using UnityEngine;
using System.Collections;

public class EnergyBlink : MonoBehaviour {

	public EnergyRing energyRing;
	public SpriteRenderer _texture;
	
	private float maxValue = 100;
	private float _currentValue = 1;
	private float value = 1.0f;
	private bool isGrowing = false;
	private bool _isFadingOut = false;

	public void OnTriggerStay2D (Collider2D other) 
	{
		if (energyRing.isActive) 
		{
            if (other.CompareTag("Player"))
            {
				Blink (); 
			}
		}
	}

	public void OnEnable ()
	{
		energyRing.onDeactivate += OnEnargyEnd;
		LevelManager.onPlayerDied += FadeOut;
		LevelManager.onPlayerTeleport += OnPlayerTeleport;
        
	}
	
	public void OnDisable ()
	{
		energyRing.onDeactivate -= OnEnargyEnd;
		LevelManager.onPlayerDied -= FadeOut;
		LevelManager.onPlayerTeleport -= OnPlayerTeleport;
	}

	public void OnEnargyEnd ()
	{
		_isFadingOut = true;
	}

	public void OnTriggerExit2D (Collider2D other) 
	{
		if (energyRing.isActive) 
		{
            if (other.CompareTag("Player"))
            {
				FadeOut ();
			}
		}
	}

	private void OnPlayerTeleport (bool isTeleportEnds) 
	{
		if (isTeleportEnds)
		{
			FadeOut ();
		}
	}

	private void FadeOut ()
	{
		_isFadingOut = true;
	}

	public void Update ()
	{
		if (_isFadingOut)
		{
			_currentValue -= value;
			isGrowing = false;
			ChangeApperacne();
			if (_currentValue <= 1) 
			{
				_isFadingOut = false;
				isGrowing = true;
			}
		}
	}

	public void Blink () 
	{
		if (_currentValue <= 0 || _currentValue >= maxValue) 
		{
			Reverse ();
		}

		if (isGrowing)
		{
			_currentValue -= value;
		}
		else
		{
			_currentValue += value;
		}

		ChangeApperacne();
	}

	public void ChangeApperacne ()
	{
		var percent = _currentValue / maxValue;
		_texture.color =  new Color(255f, 255f, 255f, percent);
	}

	private void Reverse ()
	{
		isGrowing = !isGrowing;
	}
}
