using UnityEngine;
using System.Collections;

public class FallingSawSpawner : MonoBehaviour {

    public Transform Destination;
    public FallingProjectile projectile;
    
    public float Speed;

    private BoxCollider2D _boxCollider;

    public void Start ()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void OnEnable ()
    {
        LevelManager.onPlayerAlive += Reset;
    }

    public void OnDisable ()
    {
        LevelManager.onPlayerAlive -= Reset;
    }

    public void Reset ()
    {
        _boxCollider.enabled = true;
        projectile.transform.position = transform.position;
        projectile.enabled = true;
    }



    public void DropProjectile ()
    { 
        projectile.Initialize(Destination, Speed, gameObject);
        projectile.enabled = true;
        _boxCollider.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DropProjectile();
        }
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