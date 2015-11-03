using UnityEngine;
using System.Collections;

public class TickingTrigger : MonoBehaviour {

    public SpriteRenderer renderer;
    public StandardProjectile projectile;

    private bool _isCounting = false;
    private bool _isInRange = false;

    public float LiveTime;
    public float TickCount;
    private float _tickSeconds;
    private float _timeToDestry;
    public float explodeRadius;
    public LayerMask explodeLayerMask;
    public SpriteRenderer explosionRange;

    //public void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        projectile.Direction = null;
    //        _isCounting = true;
    //        explosionRange.enabled = true;
    //    }
    //}

    private void OnPlayerDetect ()
    {
        _isCounting = true;
        explosionRange.enabled = true;
    }

    public void OnEnable()
    {
        projectile.onPlayerDetect += OnPlayerDetect;

    }

    public void OnDisable ()
    {
        projectile.onPlayerDetect -= OnPlayerDetect;
    }


    public void Start()
    {
        TickCount *= 2;
        _tickSeconds = LiveTime / TickCount;
        _timeToDestry = LiveTime;
    }



    public void Update ()
    {
        if (_isCounting)
        {
            if ((_timeToDestry -= Time.deltaTime) <= 0)
            {
               Collider2D player = Physics2D.OverlapCircle(transform.position, explodeRadius, explodeLayerMask);
               if (player != null)
               {
                   projectile.OnDamagePlayer();
               }
               else
               {
                   projectile.DestroyProjectile();
               }
               return;

            }

            if ((_tickSeconds -= Time.deltaTime) <= 0)
            {
                renderer.enabled = !renderer.enabled;
                _tickSeconds = LiveTime / TickCount;
            }
        }
    }
}
//public void OnTriggerStay2D(Collider2D other)
//{
//    if (other.CompareTag("Player"))
//    {
//        _isInRange = true;
//    }
//}

//public void OnTriggerExit2D(Collider2D other)
//{
//    if (other.CompareTag("Player"))
//    {
//        _isInRange = false;
//    }
//}

//void OnDrawGizmosSelected()
//{
//    Gizmos.color = Color.yellow;
//    Gizmos.DrawSphere(transform.position, explodeRadius);
//}