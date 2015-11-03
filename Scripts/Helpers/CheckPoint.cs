using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckPoint : MonoBehaviour {

	//private BoxCollider2D _boxCollider;
	private Vector3 _spawnPoint;
	private float _spawnRotation = 0;

	private List<EnergyRing> _subscribedRings = new List<EnergyRing> ();
    private List<Observer> _subscribedObservers = new List<Observer>();

	public void Start ()
	{
		SetSpawnPoit ();
	}

	public void SetSpawnPoit () 
	{
		var _boxCollider = GetComponent<BoxCollider2D> ();
		var localScale = transform.localScale;
		
		var _halfSize = new Vector2 (_boxCollider.size.x * Mathf.Abs (localScale.x), _boxCollider.size.y * Mathf.Abs (localScale.y)) / 2;
		var _center = transform.position + new Vector3 (_boxCollider.offset.x * localScale.x, _boxCollider.offset.y * localScale.y);
		
		_spawnPoint = new Vector3 (_center.x + _halfSize.x , _center.y, 0); 
	}

	public void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.CompareTag("Player"))
        {
			var collider = GetComponent<BoxCollider2D>();
			collider.enabled = false; 
			LevelManager.Instance.SetCurrentPoint(this);
		}
	}

	public void SpawnPlayer (PlayerController player)
	{
		if ( _spawnPoint == Vector3.zero) 
		{
			SetSpawnPoit ();
//			_spawnPoint = new Vector3(transform.position.x + 1, transform.position.y, 0) ;
		}
		player.RespawnAt (_spawnPoint, transform.eulerAngles.z);
		ActivateSubscribed ();
	}

	public void SubscribeEnergyRing (EnergyRing energyRing)
	{
		if (!_subscribedRings.Contains(energyRing))
		{
			_subscribedRings.Add (energyRing);
		}
	}

    public void SubscribeObserver(Observer observer)
    {
        if (!_subscribedObservers.Contains(observer))
        {
            _subscribedObservers.Add(observer);
        }
    }

    public void ActivateSubscribed()
    {
        for (var i = 0; i < _subscribedRings.Count; i++)
        {
            _subscribedRings[i].Activate();
        }
        _subscribedRings.Clear();

        for (var i = 0; i < _subscribedObservers.Count; i++)
        {
            _subscribedObservers[i].StartWorking();
        }
        _subscribedObservers.Clear();

        //		Debug.Log (_subscribedRings.Count);
    }
}
