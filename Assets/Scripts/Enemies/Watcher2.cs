using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class Watcher2 : Enemy
{
    [Header("Unit Specific Attack Settings")]
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float projectileLife = 2f;
    [SerializeField] private float BeamDuration = 1.5f;
    private bool mFiring = false;

    private void Fire()
    {
        GameObject intProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
        intProjectile.GetComponent<Rigidbody>().AddForce(projectileSpawn.forward * fireForce, ForceMode.Impulse);
        Destroy(intProjectile, projectileLife);
    }

    public override void SetStateAttacking()
    {
        mFiring = true;
        base.SetStateAttacking();
    }

    protected override void DoAttack()
    {
        if (mFiring)
        {
            LocatePlayer();
            RotateToPlayer();
            Fire();
            if (Time.time > mLastAttack + BeamDuration)
            {
                mFiring = false;
                SetStateTargeting();
            }
        }
    }

    public override void SetStun(float duration)
    {
        mFiring = false;
        base.SetStun(duration);
    }

}
