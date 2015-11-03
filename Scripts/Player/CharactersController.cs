using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharactersController : MonoBehaviour {

	public List<Character> characters = new List<Character>();
	private int _currentPlayerIndex = 0;
	private bool IsRespawning = false;
	public float switchTime = 1;
	private float _switchTimer = 1;
	public GameObject spawnEffect;

    public delegate void SwitchCharacterEvent(Character character);
    public static event SwitchCharacterEvent onSwitch;

    public delegate void LockCharacterEvent(Character character, bool isSingleCharacter);
    public static event LockCharacterEvent onLockCharacters;
    
    public float SwitchEnergyCost = 15.0f;
    public float currentSwitchEnergyCost { get; set;}

    private bool _isSwitchUnlock = false;

	private bool IsSwitchPossible ()
	{
		return !IsRespawning && _switchTimer < 0 && characters [_currentPlayerIndex].movement.IsGrounded ();
	}

    public void SetSwitchPossibility (bool isSwitchPossible)
    {
        _isSwitchUnlock = isSwitchPossible;
        if(_isSwitchUnlock)
        {
            _switchTimer = switchTime;
        }

        if (onLockCharacters != null)
        {
            onLockCharacters(characters[_currentPlayerIndex], _isSwitchUnlock);
        }
    }

    public void Start ()
    {
        currentSwitchEnergyCost = SwitchEnergyCost;
    }


	void Update () {

        if (_isSwitchUnlock)
        {
            _switchTimer -= Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.Q) && IsSwitchPossible())
            {
                //LevelManager.Instance.Energy.SubstractEnergy(currentSwitchEnergyCost);
                LevelManager.Instance.TakeDamage(currentSwitchEnergyCost);
                SwitchCharacter();
                SpawnEffect();
                _switchTimer = switchTime;
            } 
        }	
	}

	private void SpawnEffect () 
	{
		GameObject _particle = Instantiate (spawnEffect, transform.position, Quaternion.identity) as GameObject;
		_particle.transform.parent = transform;
		//Destroy (_particle, 0.5f);

	}

    public Character GetCurrentCharacter ()
    {
        return characters[_currentPlayerIndex];
    }


    private void SetCharacterByIndex (int characterIndex)
    {
        for (int i = 0; i < characters.Count; i ++)
        {
            if (i == characterIndex)
            {
                characters [i].Enabled (true);
                _currentPlayerIndex = i;
            }
            else
            {
                 characters[i].Enabled(false);
            }
           

        }

         if (onSwitch != null)
        {
            onSwitch(characters[_currentPlayerIndex]);
        }
    }

    public void SetCharacterByName (CharacterName name)
    {
        Character character = characters.Find( x => x.name == name);
        SetCharacterByIndex(character.ID);
    }
    

	private void SwitchCharacter()
	{
        var lastLocalScale = characters[_currentPlayerIndex].GetLocalScale();
		characters [_currentPlayerIndex].Enabled (false);
		_currentPlayerIndex++; 
		if (_currentPlayerIndex >= characters.Count) 
		{
			_currentPlayerIndex = 0;
		}
		characters [_currentPlayerIndex].Enabled (true);
        characters[_currentPlayerIndex].SetLocalScale(lastLocalScale);

        if (onSwitch != null)
        {
            onSwitch(characters[_currentPlayerIndex]);
        }

	}

	public void TurnOff()
	{
		IsRespawning = true;
		characters [_currentPlayerIndex].boxCollider.enabled = false;
        currentSwitchEnergyCost = SwitchEnergyCost;
	}
	
	public void TurnOn()
	{
		IsRespawning = false;
		characters [_currentPlayerIndex].boxCollider.enabled = true;
	}
}
