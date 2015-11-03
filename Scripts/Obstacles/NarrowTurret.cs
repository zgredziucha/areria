using UnityEngine;
using System.Collections;

public class NarrowTurret : StandardTurret {

    

    public void Start ()
    {
        _isShooting = true;
        shootTimer = shootFrequency;
    }

    public void Update()
    {
        if (_isShooting)
        {
            StartCoroutine("Shoot");
        }
        else
        {
            shootTimer -= Time.deltaTime;
            if (isFireEnabled)
            {
                shootTimer = shootFrequency;
                _isShooting = true;

            }
        }
    }
}
