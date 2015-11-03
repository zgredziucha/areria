using UnityEngine;
using System.Collections;

public class PathedProjectile : MonoBehaviour, ITakeDamage {

    private Transform _destination;
    private float  _speed;

    public GameObject DestroyEffect;

	public void Initialize (Transform destination, float speed)
    {
        _destination = destination;
        _speed = speed;
    }

	void Update () {

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
        Destroy(gameObject);
	}

    public void TakeDamage (int damage, GameObject instigator)
    {
        if (DestroyEffect != null)
        {
            Instantiate(DestroyEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
