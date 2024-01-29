using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Diagnostics;
using System.Drawing;
using UnityEditor.Experimental.GraphView;

public class PlayerAttack : MonoBehaviour
{
    // Melee variables
    public GameObject Melee;
    bool isAttacking = false;
    float atkDuration = 0.3f;
    float atkTimer = 0f;
    public float atkStartup = 0.5f;
    public float atkRecovery = 0.3f;
    public float atkCooldown = 1f;
    private float atkCDTimer = 9999f;


    // Ranged variables
    public Transform projectileSpawn;
    public GameObject projectile;
    public float fireForce = 10f;
    public float shootCooldown = 0.5f;
    private float shootTimer = 9999f;
    public float projectileLife = 2f;
    public float shootRecovery = 0.3f;
    private bool isShooting = false;

    public Animator PlayerAnimator;

    // Aim control variables
    private bool isGamepad;
    private Vector2 aimInput;
    private Vector3 aimDirection;

    public InputActionAsset actions;
    private InputAction aimAction;

    // Reference allowing interaction with player movement.
    // **Consider removing this variable and merging this entire script with PlayerController.**
    public PlayerController Player;

    private void Awake()
    {
        aimAction = actions.FindActionMap("Player").FindAction("Aim");
    }

    void Update()
    {
        HandleAim();
        CheckMeleeTimer();
        CheckShootRecovery();
        shootTimer += Time.deltaTime;
        atkCDTimer += Time.deltaTime;
    }

    void OnEnable()
    {
        actions.FindActionMap("Player").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("Player").Disable();
    }

    private void HandleAim()
    {
        // Get input value for aim
        aimInput = aimAction.ReadValue<Vector2>();

        // Handle gamepad control
        if (isGamepad)
        {
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

    private void rotateToAim()
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
                rotateToAim();

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
                isShooting = true;
                rotateToAim();

                // Fire Projectile
                shootTimer = 0;
                GameObject intProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
                intProjectile.GetComponent<Rigidbody>().AddForce(projectileSpawn.forward * fireForce, ForceMode.Impulse);
                Destroy(intProjectile, projectileLife);
            }
        }
    }

    private void CheckShootRecovery()
    {
        if (isShooting)
        {
            if (shootTimer > shootRecovery)
            {
                Player.canMove = true;
                isShooting = false;
            }
        }
    }

    void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            // Start attack process
            atkTimer += Time.deltaTime;

            // Release character once recovery window has expired
            if (atkTimer >= atkDuration + atkStartup + atkRecovery)
            {
                Player.canMove = true;
                isAttacking = false;
                atkTimer = 0;
                atkCDTimer = 0;
            }
            // end attack if delay + attack time has expired
            else if (atkTimer >= atkDuration + atkStartup)
            {
                Melee.SetActive(false);
            }
            // start attack if delay period has expired
            else if (atkTimer >= atkStartup)
            {
                Melee.SetActive(true);
            }
        }
    }

    public void OnDeviceChange(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}