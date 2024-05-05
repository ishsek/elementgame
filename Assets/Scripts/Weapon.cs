using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 50f;
    public enum WeaponType { Melee, Projectile, PiercingProjectile, AoE };
    public WeaponType weaponType;
    [SerializeField] private float mStunLength = 0;
    [SerializeField] private float mKnockback = 0;


    private void OnTriggerEnter(Collider collision)
    {
        // Check if it is an enemy being attacked
        if (collision.TryGetComponent(out Health HP))
        {
            HP.TakeDamage(damage);
            if (mStunLength > 0)
            {
                if (collision.TryGetComponent(out Enemy Enemy))
                {
                    Enemy.SetStun(mStunLength);
                }
            }
            if (mKnockback > 0)
            {
                if (collision.TryGetComponent(out Rigidbody rb))
                {
                    //Vector3 PushDirection = collision.transform.position - transform.position;
                    Vector3 PushDirection = transform.forward;
                    //PushDirection.y = 0;
                    //PushDirection = PushDirection.normalized;
                    Debug.Log(PushDirection);
                    PushDirection = PushDirection * mKnockback;
                    rb.AddForce(PushDirection, ForceMode.Impulse);
                    Debug.Log(PushDirection);
                }
            }
            if (weaponType == WeaponType.Projectile)
            {
                Destroy(gameObject);
            }
        }
    }
}
