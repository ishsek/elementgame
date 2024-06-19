using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 50f;
    public enum WeaponType { Melee, Projectile, PiercingProjectile, AoE };
    public WeaponType weaponType;
    [SerializeField] private bool m_BlocksAttacks = false;
    [SerializeField] private bool m_Blockable = true;
    [SerializeField] private float m_StunLength = 0;
    [SerializeField] private float m_Knockback = 0;


    private void OnTriggerEnter(Collider collision)
    {
        // Check if it is an enemy being attacked
        if (collision.TryGetComponent(out Health HP))
        {
            HP.TakeDamage(damage);
            if (m_StunLength > 0)
            {
                if (collision.TryGetComponent(out Enemy Enemy))
                {
                    Enemy.SetStun(m_StunLength);
                }
            }
            if (m_Knockback > 0)
            {
                if (collision.TryGetComponent(out Rigidbody rb))
                {
                    Vector3 PushDirection = transform.forward;
                    PushDirection = PushDirection * m_Knockback;
                    rb.AddForce(PushDirection, ForceMode.Impulse);
                }
            }
            if (weaponType == WeaponType.Projectile)
            {
                Destroy(gameObject);
            }
        }

        if (collision.TryGetComponent(out Weapon OtherWeapon))
        {
            if (m_Blockable && OtherWeapon.m_BlocksAttacks)
            {
                if (weaponType == WeaponType.Projectile || weaponType == WeaponType.PiercingProjectile)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
