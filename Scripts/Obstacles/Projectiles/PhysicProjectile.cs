using UnityEngine;
using System.Collections;

public class PhysicProjectile : StandardProjectile {

    public Rigidbody2D projectileBody;

    public float randomValue = 5.0f;

    public void AddForce(float force, Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > 0)
        {
            direction.x *= force;
            direction.y = Random.Range(-randomValue, randomValue);
        }
        else
        {
            direction.x = Random.Range(-randomValue, randomValue);
            direction.y *= force;
        }
 
        projectileBody.AddForce( direction, ForceMode2D.Impulse);
    }

    public override void DestroyProjectile()
    {
        if (DestroyedEffect != null)
        {
            Instantiate(DestroyedEffect, transform.position, transform.rotation);
        }
        Destroy(projectileBody.gameObject);
    }

    protected override void CheckCollisions(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnCollidePlayer();
            return;
        }
    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {
       
    }
}
