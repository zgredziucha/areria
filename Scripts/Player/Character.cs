using UnityEngine;
using System.Collections;
using System;

public enum CharacterName
{
    Mantis,
    Moth
}


[Serializable]
public class Character {
	public MeshRenderer meshRenderer;
	public BoxCollider2D boxCollider;
	public PlayerMovement movement;

    public bool seeAssasins;
    public int ID;
    public CharacterName name;

    public delegate void NewAbility();
    public static event NewAbility onNewAbility;

	public void Enabled (bool _isEnabled)
	{
		meshRenderer.enabled = _isEnabled;
		boxCollider.enabled = _isEnabled;
		movement.enabled = _isEnabled;

        movement.ResetMovement();
	}

    public void SetSeeAssasinsAbility()
    {
        seeAssasins = true;
        if ( onNewAbility != null)
        {
            onNewAbility();
        }
    }

    public void SetLocalScale (float localScale)
    {
        movement.SetFacingDirection(localScale);
    }

    public float GetLocalScale ()
    {
        return movement.transform.localScale.x;
    }
}
