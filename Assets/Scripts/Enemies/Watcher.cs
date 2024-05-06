using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : Enemy
{
    [Header("Unit Specific Attack Settings")]
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float projectileLife = 2f;

    private void Fire()
    {
        GameObject intProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
        intProjectile.GetComponent<Rigidbody>().AddForce(projectileSpawn.forward * fireForce, ForceMode.Impulse);
        Destroy(intProjectile, projectileLife);
    }

    public override void SetStateAttacking()
    {
        base.SetStateAttacking();
        Fire();
        LocatePlayer();
        CheckAgro();
    }
}
