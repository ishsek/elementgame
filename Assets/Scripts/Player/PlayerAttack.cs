using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Diagnostics;

public class PlayerAttack : MonoBehaviour
{
    // Melee variables
    public GameObject Melee;
    bool isAttacking = false;
    float atkDuration = 0.3f;
    float atkTimer = 0f;

    // Ranged variables
    public Transform aim;
    public GameObject projectile;
    public float fireForce = 10f;
    public float shootCooldown = 0.5f;
    private float shootTimer = 999999f;
    public float projectileLife = 2f;


    // Update is called once per frame
    void Update()
    {
        CheckMeleeTimer();
        shootTimer += Time.deltaTime;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isAttacking)
            {
                Melee.SetActive(true);
                isAttacking = true;
                // Call animator to play melee attack here
            }
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        //UnityEngine.Debug.Log("Firing");
        if (context.performed)
        {
            if (shootTimer > shootCooldown)
            {
                shootTimer = 0;
                GameObject intProjectile = Instantiate(projectile, aim.position, aim.rotation);
                intProjectile.GetComponent<Rigidbody>().AddForce(aim.forward * fireForce, ForceMode.Impulse);
                Destroy(intProjectile, projectileLife);
            }
        }
    }

    void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            atkTimer += Time.deltaTime;
            if (atkTimer >= atkDuration)
            {
                atkTimer = 0;
                isAttacking = false;
                Melee.SetActive(false);
            }
        }
    }
}