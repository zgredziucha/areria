using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalController : PortalSimpleController {

	public Transform EnergyPointer;
	public Portal portal;
	public FadeTexture portalTexture;

	private List<SpriteRenderer> pointRenderers = new List<SpriteRenderer> (); 
	public int energyPointCost = 20;

	private int _energyPointCount;
	private int _currentEnergyCount;

	public int startPoints = 1;

	public void Start ()
	{
		int children = EnergyPointer.childCount;
		for (int i = 0; i < children; ++i)
		{
			pointRenderers.Add(EnergyPointer.GetChild(i).GetComponent<SpriteRenderer>());
		}

		InitializeEnergy  ();
		portal.activatePortalAction += Activate;
	}

    public void OnEnable ()
    {
        LevelManager.onPlayerDied += InitializeEnergy;
    }

    public void OnDisable ()
    {
        LevelManager.onPlayerDied -= InitializeEnergy;
    }

	private void InitializeEnergy ()
	{
		_energyPointCount = pointRenderers.Count;
		_currentEnergyCount = startPoints;
		portal.isTeleportPossible = _currentEnergyCount > 0;

		if (_currentEnergyCount <= 0)
		{
			portalTexture.FadeOut();
		}  
        else
        {
            portalTexture.FadeIn();
        }                                                                            

		for (var i = 0; i < _energyPointCount; i++)
		{
			pointRenderers[i].enabled = i < _currentEnergyCount;
		}
	}

	public void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.CompareTag ("Player") && 
		    _currentEnergyCount <= 0 && 
		    !LevelManager.Instance.Energy.IfEnergyWillKill((float)energyPointCost)) 
		{
			LevelManager.Instance.Player.SubscribeKey(KeyCode.Space, delegate() { 
				//Debug.Log ("aaa");
				AddEnegry();
			});
		}
	}
	
	public void OnTriggerExit2D (Collider2D other) 
	{
		var player = other.GetComponent<PlayerController> ();
		if (player != null) {
			player.UnsubscribeKey(KeyCode.Space);
		}
	}

	public void AddEnegry ()
	{
//		if (_currentEnergyCount + 1 <= _energyPointCount) 
		if (_currentEnergyCount <= 0 &&
		    !LevelManager.Instance.Energy.IfEnergyWillKill((float)energyPointCost))
		{
			LevelManager.Instance.TakeDamage((float)energyPointCost);
			pointRenderers[_currentEnergyCount].enabled = true;
			_currentEnergyCount++;
			portal.isTeleportPossible = true;
			portalTexture.FadeIn();
		}
	}
	
	public void Activate ()
	{
		if (_currentEnergyCount > 0) 
		{
			_currentEnergyCount --;
			pointRenderers[_currentEnergyCount].enabled = false;

			if (_currentEnergyCount <= 0) 
			{
				portal.isTeleportPossible = false;
				portalTexture.FadeOut();
			}
		}
	}

//	public Vector3 GetSpawnPoit () 
//	{
//		var _boxCollider = GetComponent<BoxCollider2D> ();
//		var localScale = transform.localScale;
//		
//		var _offset = new Vector2 (_boxCollider.size.x * localScale.x, _boxCollider.size.y * Mathf.Abs (localScale.y)) / 2;
//		var _center = transform.position + new Vector3 (_boxCollider.center.x * localScale.x, _boxCollider.center.y * localScale.y);
//		var point = new Vector3 (_center.x + _offset.x, _center.y, 0);
//		Vector3 dir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * (point - _center); 
//		return dir + _center;
//	}
}
