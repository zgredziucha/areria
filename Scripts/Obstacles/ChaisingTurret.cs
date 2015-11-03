using UnityEngine;
using System.Collections;

public class ChaisingTurret : MonoBehaviour {
    
    public ChaisingTarget chaisingTarget;
    public float Speed;
    public float FireRate;
    public GameObject projectile;

    private float _maxShootInSeconds;
    private Transform TargetTransform;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            chaisingTarget.Target = other.transform;
            chaisingTarget.isChasing = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            chaisingTarget.isChasing = false;
            chaisingTarget.Target = null;
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _maxShootInSeconds <= 0)
        {
            chaisingTarget.Target = other.transform;
            chaisingTarget.isChasing = true;
        }
    }


    public void Start()
    {
        _maxShootInSeconds = 0;
    }

    public void OnEnable ()
    {
        chaisingTarget.onPlayerDetect += OnTargetDetect;
    }

    public void OnDisable ()
    {
        chaisingTarget.onPlayerDetect -= OnTargetDetect;
    }

    
    public void OnTargetDetect (Transform targetTransform)
    {
        //TargetTransform = _maxShootInSeconds > 0 ? null : targetTransform;
        if (_maxShootInSeconds <= 0)
        {
            TargetTransform = targetTransform;
            chaisingTarget.isChasing = false;
            chaisingTarget.Target = null;
        }
       
    }

    public void Update()
    {
        _maxShootInSeconds -= Time.deltaTime;

        if (TargetTransform == null ) 
        {
            return;
        }

        _maxShootInSeconds = FireRate;
       
        GameObject _projectile = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
        _projectile.GetComponent<ChaisingProjectile>().Initialize(gameObject, TargetTransform, Speed);

        TargetTransform = null;
        
       
    }
}
