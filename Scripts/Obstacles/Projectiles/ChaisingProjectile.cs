using UnityEngine;
using System.Collections;

public class ChaisingProjectile : StandardProjectile
{
    public Transform Direction { get; set; }
    public float InitialVelocity { get; private set; }

    public void Initialize(GameObject owner, Transform direction, float initialVelocity)
    {
        //transform.right = direction;
        Owner = owner;
        Direction = direction;
        InitialVelocity = initialVelocity;
    }

    public void Update()
    {
        if ((TimeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjectile();
            return;
        }

        if (Direction == null)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, Direction.position, Time.deltaTime * InitialVelocity);
        //transform.Translate(Direction.position * (Mathf.Abs(InitialVelocity) + Speed) Speed * Time.deltaTime, Space.World);
    }

    public override void OnCollidePlayer()
    {
        Direction = null;
        base.OnCollidePlayer();
    }


}
