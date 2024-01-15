using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 50f;

    private void OnTriggerEnter(Collider collision)
    {
        // Check if it is an enemy being attacked
        Health enemy = collision.GetComponent<Health>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
