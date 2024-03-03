using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shadow : MonoBehaviour
{

    [Header("Melee")]
    public GameObject Melee;
    private float[] atkStartup = new float[] { 0.3f, 0.6f, 1.3f };
    private float[] mAtkDuration = new float[] { 0.3f, 0.5f, 0.45f };
    private float[] atkRecovery = new float[] { 0.25f, 0.5f, 0.5f };
    private bool mIsAttacking = false;
    private float mAtkTimer = 0f;
    private float mComboTimer = 99f;
    private float mComboWindow = 3f;
    private int mCurrentCombo = 0;
    private int mMaxCombo = 3;
    private bool mComboQueued = false;
    private string[] mComboList = new string[] { "SwordSlash1", "SwordSlash2", "SwordSlash3" };

    [Header("Ranged")]
    public Transform projectileSpawn;
    public GameObject projectile;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float shootCooldown = 0.5f;
    [SerializeField] private float projectileLife = 2f;
    [SerializeField] private float shootRecovery = 0.3f;
    [SerializeField] private float shootDelay = 0.1f;
    private bool hasFired = false;
    private float shootTimer = 9999f;
    private bool isShooting = false;

    [Header("Dodging")]
    public AnimationCurve DodgeCurve;
    [SerializeField] private float DodgeSpeed;
    [SerializeField] private float DodgeDuration;
    private float DodgeTime;

    [Header("References")]
    public PlayerController Player;
    public Animator MyAnimator;

    void Update()
    {
        CheckMeleeTimer();
        CheckShootTimer();
        shootTimer += Time.deltaTime;
        mComboTimer += Time.deltaTime;
    }

    public void Attack1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!mIsAttacking)
            {
                Player.HaltMovement();
                Player.SetStateAttacking();
                Player.rotateToAim();
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

    public void Attack2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (shootTimer > shootCooldown)
            {
                Player.HaltMovement();
                Player.SetStateAttacking();
                Player.rotateToAim();
                isShooting = true;
                hasFired = false;

                // Fire Projectile
                shootTimer = 0;

            }
        }
    }

    public void Ability1()
    {

    }

    public void Ability2()
    {

    }

    public void Ability3()
    {

    }

    public void Ability4()
    {

    }

    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Player.OnDodge();
        }
    }

    private void CheckShootTimer()
    {
        if (isShooting)
        {
            if (shootTimer > shootDelay + shootRecovery)
            {
                Player.SetStateNormal();
                Player.canMove = true;
                isShooting = false;
            }
            else if (shootTimer > shootDelay)
            {
                if (!hasFired)
                {
                    GameObject intProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
                    intProjectile.GetComponent<Rigidbody>().AddForce(projectileSpawn.forward * fireForce, ForceMode.Impulse);
                    Destroy(intProjectile, projectileLife);
                    hasFired = true;
                }
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
                    Player.rotateToAim();
                    CheckCombo();
                    mComboQueued = false;
                }
                else
                {
                    Player.SetStateNormal();
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
        MyAnimator.SetTrigger(mComboList[mCurrentCombo]);
        mComboTimer = 0f;
    }
}
