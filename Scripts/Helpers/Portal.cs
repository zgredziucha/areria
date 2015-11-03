using UnityEngine;
using System.Collections;

public delegate void ActivatePortal();

public class Portal : MonoBehaviour {

	public PortalSimpleController destinationPortal;
	public bool isTeleportPossible { get; set; }

	public ActivatePortal activatePortalAction;

	public void OnTriggerEnter2D (Collider2D other) 
	{
		if (isTeleportPossible && other.CompareTag ("Player")) 
		{
			if (activatePortalAction != null)
			{
				activatePortalAction();
			}
			LevelManager.Instance.TeleportPlayer (this);
		}
	}

	public void SpawnPlayer (PlayerController player)
	{
		var _spawnPoint = destinationPortal.GetSpawnPoit ();
		player.RespawnAt (_spawnPoint, 0.0f);
	}


}
