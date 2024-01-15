using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 50f;
    public enum WeaponType { Melee, Projectile, PiercingProjectile, AoE };
    public WeaponType weaponType;

    private void OnTriggerEnter(Collider collision)
    {
        // Check if it is an enemy being attacked
        Health enemy = collision.GetComponent<Health>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            if (weaponType == WeaponType.Projectile)
            {
                Destroy(gameObject);
            }
        }
    }
}
