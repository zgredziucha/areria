using UnityEngine;
using System.Collections;

public class HardcoreTurret : StandardTurret {

	public void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player") && isFireEnabled)
        {
            _isShooting = true;
        }
    }

    public void Update ()
    {
        shootTimer -= Time.deltaTime;
        if (_isShooting)
        {
            StartCoroutine("Shoot");
        }
    }

}
