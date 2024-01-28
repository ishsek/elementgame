using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Diagnostics;
using System.Drawing;

public class PlayerAttack : MonoBehaviour
{
    // Melee variables
    public GameObject Melee;
    bool isAttacking = false;
    float atkDuration = 0.3f;
    float atkTimer = 0f;

    // Ranged variables
    public Transform projectileSpawn;
    public GameObject projectile;
    public float fireForce = 10f;
    public float shootCooldown = 0.5f;
    private float shootTimer = 999999f;
    public float projectileLife = 2f;

    public Animator PlayerAnimator;

    // Aim control variables
    private bool isGamepad;
    private Vector2 aimInput;
    private Vector3 aimDirection;

    // Reference allowing interaction with player movement.
    // **Consider removing this variable and merging this entire script with PlayerController.**
    public PlayerController Player;

    void Update()
    {
        CheckMeleeTimer();
        shootTimer += Time.deltaTime;
    }

    public void onAim(InputAction.CallbackContext context)
    {
        // Get input value for aim
        aimInput = context.ReadValue<Vector2>();

        // Handle gamepad control
        if (isGamepad)
        {
            print(aimInput);
            aimDirection = Vector3.right * aimInput.x + Vector3.forward * aimInput.y;
        }
        // Handle mouse control
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(aimInput);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                aimDirection = ray.GetPoint(rayDistance);
            }
        }
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isAttacking)
            {
                // Lock player movement
                Player.canMove = false;

                // Rotate player to aim direction
                if (isGamepad)
                {
                    if (aimDirection.sqrMagnitude > 0.0f)
                    {
                        transform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
                    }
                }
                else
                {
                    LookAt(aimDirection);
                }

                // Start melee hitbox timer
                Melee.SetActive(true);
                isAttacking = true;

                // Call animator to play melee attack here
                //Refactor this to be a static location for the trigger names
                PlayerAnimator.SetTrigger("SwordSlash1");
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
                // Lock player movement
                Player.canMove = false;

                // Rotate player to aim direction
                if (aimDirection.sqrMagnitude > 0.0f)
                {
                    transform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
                }

                // Fire Projectile
                shootTimer = 0;
                GameObject intProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
                intProjectile.GetComponent<Rigidbody>().AddForce(projectileSpawn.forward * fireForce, ForceMode.Impulse);
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
                Player.canMove = true;
            }
        }
    }

    public void OnDeviceChange(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}