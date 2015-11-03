using UnityEngine;
using System.Collections;

public class StandardProjectile : MonoBehaviour, ITakeDamage 
{
    public int Damage;
    public GameObject DestroyedEffect;

    public float TimeToLive;
   
    public GameObject Owner { get; protected set; }

    public LayerMask CollisionMask;

    public delegate void PlayerDetection();
    public event PlayerDetection onPlayerDetect;

    protected void CountDown()
    {
        if  ((TimeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjectile();
        }
    }

    public void Update ()
    {
        CountDown();
    }

    public void Initialize(GameObject owner)
    {
        Owner = owner;
    }

    protected virtual void CheckCollisions(Collider2D other)
    {
        if (Owner == null || other.transform.IsChildOf(transform))
        {
            return;
        }

        var parent = other.transform.parent;
        var isOwner = other.gameObject == Owner || (parent != null && parent.gameObject == Owner);
        if (isOwner)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            OnCollidePlayer();
            return;
        }

        if ((CollisionMask.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        var takeDamage = (ITakeDamage)other.GetComponent(typeof(ITakeDamage));
        if (takeDamage != null)
        {
            OnCollideTakeDamage(other, takeDamage);
            return;
        }

        OnCollideOther();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
      CheckCollisions(other);
    }

    public void TakeDamage (int damage, GameObject instibator)
    {
        DestroyProjectile();
    }

  
    public virtual void DestroyProjectile ()
    {
        if (DestroyedEffect != null)
        {
            Instantiate(DestroyedEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    protected virtual void OnCollideOther ()
    {
        DestroyProjectile();
    }

    private void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
    {
        takeDamage.TakeDamage(Damage, gameObject);
        DestroyProjectile();
    }

    public virtual void OnCollidePlayer()
    {
        if (onPlayerDetect != null)
        {
            onPlayerDetect();
        }

    }

    public void OnDamagePlayer()
    {
        LevelManager.Instance.TakeDamage(Damage);
        DestroyProjectile();
    }

    public void OnEnable ()
    {
        LevelManager.onPlayerDied += DestroyProjectile;
    }

    public void OnDisable ()
    {
        LevelManager.onPlayerDied -= DestroyProjectile;
    }
}
