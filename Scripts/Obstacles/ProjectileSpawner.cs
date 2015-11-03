using UnityEngine;
using System.Collections;

public class ProjectileSpawner : MonoBehaviour {
    public Transform Destination;
    public GameObject Projectile;

    public float Speed;
    public float FireRate;

    private float _maxShootInSeconds;

    public void Start ()
    {
        _maxShootInSeconds = FireRate;

    }

    public void Update ()
    {
        if ((_maxShootInSeconds -= Time.deltaTime) > 0) 
        {
            return;
        }

        _maxShootInSeconds = FireRate;
        GameObject projectile = Instantiate(Projectile, transform.position, transform.rotation) as GameObject;
        projectile.GetComponent <PathedProjectile>().Initialize(Destination, Speed);
    }

    public void OnDrawGizmos ()
    {
        if (Destination == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Destination.position);
    }
}
