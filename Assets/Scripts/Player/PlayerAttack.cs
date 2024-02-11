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
    private float[] atkStartup = new float[] { 0.3f, 0.6f, 1.3f };
    private float[] mAtkDuration = new float[] { 0.3f, 0.5f, 0.45f};
    private float[] atkRecovery = new float[] { 0.25f, 0.5f, 0.5f};
    private bool mIsAttacking = false;
    private float mAtkTimer = 0f;
    private float mComboTimer = 99f;
    private float mComboWindow = 3f;
    private int mCurrentCombo = 0;
    private int mNextCombo = 0;
    private int mMaxCombo = 3;
    private bool mComboQueued = false;
    private string[] mComboList = new string[] { "SwordSlash1", "SwordSlash2", "SwordSlash3" };

    // Ranged variables
    public Transform projectileSpawn;
    public GameObject projectile;
    public float fireForce = 10f;
    public float shootCooldown = 0.5f;
    public float projectileLife = 2f;
    public float shootRecovery = 0.3f;
    private float shootTimer = 9999f;
    private bool isShooting = false;

    public Animator PlayerAnimator;

    // Aim control variables
    private bool isGamepad;
    private Vector2 aimInput;
    private Vector3 aimDirection;

    // Movement and Input References
    public InputActionAsset actions;
    public PlayerController Player;
    private InputAction aimAction;

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
        mComboTimer += Time.deltaTime;
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

    private void CheckMeleeTimer()
    {
        if (mIsAttacking)
        {
            // Start attack process
            mAtkTimer += Time.deltaTime;

            // Release character once recovery window has expired
            if (mAtkTimer >= mAtkDuration[mCurrentCombo] + atkStartup[mCurrentCombo] + atkRecovery[mCurrentCombo])
            {
                if (mComboQueued)
                {
                    rotateToAim();
                    CheckCombo();
                    mComboQueued = false;
                }
                else
                {
                    Player.canMove = true;
                    mIsAttacking = false;
                }
                mAtkTimer = 0;
            }
            // end attack if delay + attack time has expired
            else if (mAtkTimer >= mAtkDuration[mCurrentCombo] + atkStartup[mCurrentCombo])
            {
                Melee.SetActive(false);
            }
            // start attack if delay period has expired
            else if (mAtkTimer >= atkStartup[mCurrentCombo])
            {
                Melee.SetActive(true);
            }
        }
    }

    private void CheckCombo()
    {
        // If within combo window, play the attack and update the combo counter
        mCurrentCombo++;
        if (mComboTimer > mComboWindow || mCurrentCombo >= mMaxCombo)
        {
            mCurrentCombo = 0;
        }
        PlayerAnimator.SetTrigger(mComboList[mCurrentCombo]);
        mComboTimer = 0f;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!mIsAttacking)
            {
                rotateToAim();
                // Start melee hitbox timer
                mIsAttacking = true;

                // Call animator to play melee attack here
                //Refactor this to be a static location for the trigger names
                CheckCombo();
            }
            else if (!mComboQueued)
            {
                mComboQueued = true;
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

    public void OnDeviceChange(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}