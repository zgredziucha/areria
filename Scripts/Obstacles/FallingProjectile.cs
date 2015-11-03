using UnityEngine;
using System.Collections;

public class FallingProjectile : MonoBehaviour {

    private Transform _destination;
    private float _speed;
    private GameObject _spawner;

    public GameObject DestroyEffect;

    public void Initialize(Transform destination, float speed, GameObject spawner)
    {
        _destination = destination;
        _speed = speed;
        _spawner = spawner;
    }

    void Update()
    {
        if (_destination == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, _destination.position, Time.deltaTime * _speed);
        var distanceSquared = (_destination.position - transform.position).sqrMagnitude;

        if (distanceSquared > .01f * .01f)
        {
            return;
        }

        if (DestroyEffect != null)
        {
            Instantiate(DestroyEffect, transform.position, transform.rotation);
        }

        _destination = null;
        this.enabled = false;
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        if (DestroyEffect != null)
        {
            Instantiate(DestroyEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
