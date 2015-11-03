using UnityEngine;
using System.Collections;

public class PhysicKillingObstacle : PhysicProjectile {

    public void OnEnable()
    {
        onPlayerDetect += OnDamagePlayer;
    }

    public void OnDisable()
    {
        onPlayerDetect -= OnDamagePlayer;
    }
    
}
