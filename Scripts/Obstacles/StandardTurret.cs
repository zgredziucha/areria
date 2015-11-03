using UnityEngine;
using System.Collections;

public class StandardTurret : MonoBehaviour
{

    public GameObject projectile;
    public int projectilesCount;
    public float projectileSpeed;
    public float timeBetweenProjectiles;
    public float shootFrequency = 3.0f;
    public Transform barrel;

    protected float shootTimer = 0;
    protected int _firedProjectiles = 0;
    protected bool _isShooting = false;

    protected bool isFireEnabled { get { return shootTimer <= 0 && _firedProjectiles == 0; } }

    public IEnumerator Shoot()
    {
        _isShooting = false;

        yield return new WaitForSeconds(timeBetweenProjectiles);

        _firedProjectiles++;
        GameObject _projectile = Instantiate(projectile, barrel.position, barrel.rotation) as GameObject;
        PhysicProjectile _projectileController = _projectile.GetComponentInChildren<PhysicProjectile>();
        _projectileController.Initialize(gameObject);

        var angle = Utils.Round90(transform.eulerAngles.z);
        var direction = angle == 90 || angle == 270 ? new Vector2(-Utils.Sin(angle), 0) : new Vector2(0, Utils.Cos(angle));
        _projectileController.AddForce(projectileSpeed, direction);

        if (_firedProjectiles >= projectilesCount)
        {
            _firedProjectiles = 0;
            shootTimer = shootFrequency;
        }
        else
        {
            _isShooting = true;
        }

    }
}
