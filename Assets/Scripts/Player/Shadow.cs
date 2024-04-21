using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Shadow : MonoBehaviour
{
    [Header("References")]
    public PlayerController Player;
    public Animator MyAnimator;

    [Header("Melee")]
    public GameObject Melee;
    [SerializeField] private float m_PrimaryAttackMoveSpeed;
    [SerializeField] private List<AnimationCurve> m_PrimaryAttackCurves;
    [SerializeField] private float m_PrimaryStepSpeed;
    [SerializeField] private float m_PrimaryStepDuration;
    //[SerializeField] private List<float> m_AtkStartup;// = new float[] { 0.3f, 0.6f, 1.3f };
    //[SerializeField] private List<float> m_AtkActive;// = new float[] { 0.3f, 0.5f, 0.45f };
    //[SerializeField] private List<float> m_AtkRecovery;// = new float[] { 0.25f, 0.5f, 0.5f };
    [SerializeField] private AnimationCurve m_PrimaryStepCurve;
    private float mAttackMovementTimer = 0;
    private bool mPrimaryStepping = false;
    //private bool mIsAttacking = false;
    //private float mAtkTimer = 0f;
    private float mComboTimer = 99f;
    private float mComboWindow = 3f;
    private int mCurrentCombo = 0;
    private int mMaxCombo = 3;
    private bool mComboQueued = false;
    private bool mPrimaryActive = false;
    private string[] mComboList = new string[] { AnimationTriggersStatic.GetPlayerAttack1(), AnimationTriggersStatic.GetPlayerAttack2(), AnimationTriggersStatic.GetPlayerAttack3() };

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

    [Header("Stab")]
    [SerializeField] private GameObject m_StabWeaponObject;
    [SerializeField] private Weapon m_StabWeaponScript;
    [SerializeField] private float m_StabCD;
    [SerializeField] private float m_StabMaxChargeTime;
    [SerializeField] private float m_StabMaxDamage;
    [SerializeField] private float m_StabBaseSpeed;
    [SerializeField] private float m_StabDuration;
    [SerializeField] private AnimationCurve StabAttackCurve;
    private float mStabChargeTime;
    private float mStabLastCast = -99999;
    private float mStabTime = 0;
    private bool mChargingStab = false;
    private bool mStabAttacking = false;

    [Header("Dash Attack")]
    [SerializeField] private GameObject m_DashWeapon;
    [SerializeField] private float m_DashCD;
    [SerializeField] private float m_DashMaxChargeTime;
    [SerializeField] private float m_DashMaxDamage;
    [SerializeField] private float m_DashMaxLength;
    [SerializeField] private float m_DashBaseSpeed;
    [SerializeField] private AnimationCurve DashAttackCurve;
    private float mDashTime;
    private float mDashChargeTime;
    private float mDashLastCast = -9999;
    private bool mChargingDash = false;
    private bool mDashAttacking = false;
    private float mDashDuration;

    [Header("Black Hole")]
    public GameObject VoidAim;
    [SerializeField] private GameObject m_VoidAoE;
    [SerializeField] private GameObject m_VoidAimIndicator;
    [SerializeField] private float m_VoidCD;
    private float mVoidLastCastTime = -9999;
    private bool mAimingVoid = false;

    [Header("Dodging")]
    public AnimationCurve DodgeCurve;
    [SerializeField] private float DodgeSpeed;
    [SerializeField] private float DodgeDuration;
    [SerializeField] private float m_DodgeCD;
    private float mLastDodgeTime = -9999;

    void Update()
    {
        //CheckMeleeTimer();
        HandlePrimaryStep();
        CheckShootTimer();
        ChargeDash();
        HandleDashAttack();
        ChargeStab();
        HandleStab();
        shootTimer += Time.deltaTime;
        mComboTimer += Time.deltaTime;
    }

    public void Attack1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Player.IsNormal())
            {
                Player.HaltMovement();
                Player.SetStateAttacking();
                Player.rotateToAim();
                // Start melee hitbox timer
                // mIsAttacking = true;
                mPrimaryActive = true;
                //mAtkTimer = 0;
                CheckCombo();
            }
            else if (mPrimaryActive && !mComboQueued)
            {
                if (mCurrentCombo + 1 < mMaxCombo)
                {
                    mComboQueued = true;
                    Player.QueueRotation();
                    CheckCombo();
                }
            }
        }
    }

    public void EnablePrimaryCollider()
    {
        if (mPrimaryActive)
        {
            Melee.SetActive(true);
        }
    }

    public void DisablePrimaryCollider()
    {
        Melee.SetActive(false);
    }

    public void EndPrimaryAttack()
    {
        if(mPrimaryActive)
        {
            if (mComboQueued)
            {
                //mAtkTimer = 0;
                Player.RotateToQueuedClick();
                mComboQueued = false;
            }
            else
            {
                Player.SetStateNormal();
                //mIsAttacking = false;
                mPrimaryActive = false;
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

    public void PrimaryStep()
    {
        mAttackMovementTimer = 0;
        mPrimaryStepping = true;
    }

    private void HandlePrimaryStep()
    {
        if (mPrimaryActive && mPrimaryStepping)
        {
            if (mAttackMovementTimer < m_PrimaryStepDuration)
            {
                mAttackMovementTimer += Time.deltaTime;
                float MovePercentage = mAttackMovementTimer / m_PrimaryStepDuration;
                Player.rb.velocity = transform.forward * m_PrimaryStepSpeed * m_PrimaryStepCurve.Evaluate(MovePercentage);
            }
            else
            {
                mPrimaryStepping = false;
            }
        }
    }

    public void Attack2(InputAction.CallbackContext context)
    {
        if (shootTimer > shootCooldown && Player.IsNormal() && context.performed)
        {
                Player.HaltMovement();
                Player.SetStateAttacking();
                Player.rotateToAim();
                isShooting = true;
                //mIsAttacking = true;
                hasFired = false;

                // Fire Projectile
                shootTimer = 0;
        }
    }
    //Projectile
    public void Ability1()
    {

    }
    //Stab
    public void Ability2(InputAction.CallbackContext context)
    {
        if (Time.time > mStabLastCast + m_StabCD)
        {
            if (Player.IsNormal() && context.performed && !mChargingStab)
            {
                Player.HaltMovement();
                Player.SetStateAttacking();
                mStabChargeTime = 0;
                mChargingStab = true;
            }
            else if (mChargingStab && context.canceled)
            {
                mChargingStab = false;
                mStabLastCast = Time.time;
                if (mStabChargeTime > m_StabMaxChargeTime)
                {
                    mStabChargeTime = m_StabMaxChargeTime;
                }
                m_StabWeaponScript.damage = mStabChargeTime / m_StabMaxChargeTime * m_StabMaxDamage;
                mStabAttacking = true;
                m_StabWeaponObject.SetActive(true);
                mStabTime = 0;
            }
        }
    }

    private void ChargeStab()
    {
        if (mChargingStab)
        {
            Player.rotateToAim();
            mStabChargeTime += Time.deltaTime;
        }
    }

    private void HandleStab()
    {
        if (mStabAttacking)
        {
            mStabTime += Time.deltaTime;
            Player.rb.velocity = transform.forward * m_StabBaseSpeed * StabAttackCurve.Evaluate(mStabTime / m_StabDuration);
            if (mStabTime > m_StabDuration)
            {
                mStabAttacking = false;
                m_StabWeaponObject.SetActive(false);
                Player.SetStateNormal();
            }
        }
    }

    // Dash
    public void Ability3(InputAction.CallbackContext context)
    {
        if (Time.time > mDashLastCast + m_DashCD)
        {
            if (Player.IsNormal() && context.performed && !mChargingDash)
            {
                Player.HaltMovement();
                Player.SetStateAttacking();
                mDashChargeTime = 0;
                mChargingDash = true;
            }
            else if (mChargingDash && context.canceled)
            {
                mChargingDash = false;
                mDashLastCast = Time.time;
                if (mDashChargeTime > m_DashMaxChargeTime)
                {
                    mDashChargeTime = m_DashMaxChargeTime;
                }
                mDashDuration = mDashChargeTime / m_DashMaxChargeTime * m_DashMaxLength;
                Player.DisableEnemyCollision();
                mDashTime = 0;
                mDashAttacking = true;
                m_DashWeapon.SetActive(true);
            }
        }
    }
    private void ChargeDash()
    {
        if (mChargingDash)
        {
            Player.rotateToAim();
            mDashChargeTime += Time.deltaTime;
        }
    }
    private void HandleDashAttack()
    {
        if (mDashAttacking)
        {
            mDashTime += Time.deltaTime;
            Player.rb.velocity = transform.forward * m_DashBaseSpeed * DashAttackCurve.Evaluate(mDashTime / mDashDuration);
            if (mDashTime > mDashDuration)
            {
                mDashAttacking = false;
                m_DashWeapon.SetActive(false);
                Player.SetStateNormal();
                Player.EnableEnemyCollision();
            }
        }
    }

    //Black hole
    public void Ability4(InputAction.CallbackContext context)
    {
        if (Time.time > mVoidLastCastTime + m_VoidCD)
        {
            if (Player.isGamepad)
            {
                if (Player.IsNormal() && context.performed && !mAimingVoid)
                {
                    mAimingVoid = true;
                    Player.SetStateControllerAiming();
                    Vector3 AimSpawn = transform.position;
                    AimSpawn.y = 0;
                    VoidAim = Instantiate(m_VoidAimIndicator, AimSpawn, transform.rotation);
                    Player.AimPreview = VoidAim;
                }
                else if (mAimingVoid && context.canceled)
                {
                    if (VoidAim != null)
                    {
                        GameObject InstBlackHole = Instantiate(m_VoidAoE, VoidAim.transform.position, VoidAim.transform.rotation);
                        Player.SetStateNormal();
                        Player.AimPreview = null;
                        Destroy(VoidAim);
                        mAimingVoid = false;
                        mVoidLastCastTime = Time.time;
                    }
                }
                //if (context.started)
                //{
                //    Debug.Log("start");
                //}
                //else if (context.performed)
                //{
                //    Debug.Log("perf");
                //}
                //else if (context.canceled)
                //{
                //    Debug.Log("cancel");
                //}
            }
            else
            {
                if (context.performed)
                {
                    GameObject InstBlackHole = Instantiate(m_VoidAoE, Player.aimDirection, transform.rotation);
                    mVoidLastCastTime = Time.time;
                }
            }
        }
    }

    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Time.time > mLastDodgeTime + m_DodgeCD)
            {
                mLastDodgeTime = Time.time;
                InterruptAttack();
                Player.OnDodge();
            }
        }
    }

    private void CheckShootTimer()
    {
        if (isShooting)
        {
            if (shootTimer > shootDelay + shootRecovery)
            {
                Player.SetStateNormal();
                isShooting = false;
                //mIsAttacking = false;
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

    //private void CheckMeleeTimer()
    //{
    //    if (mPrimaryActive)
    //    {
    //        // Start attack process
    //        float AttackDuration = m_AtkActive[mCurrentCombo] + m_AtkStartup[mCurrentCombo] + m_AtkRecovery[mCurrentCombo];
    //        mAtkTimer += Time.deltaTime;
    //        Player.rb.velocity = transform.forward * m_PrimaryAttackCurves[mCurrentCombo].Evaluate(mAtkTimer / AttackDuration) * m_PrimaryAttackMoveSpeed;
    //        // Release character once recovery window has expired
    //        if (mAtkTimer >= AttackDuration)
    //        {
    //            if (mComboQueued)
    //            {
    //                mAtkTimer = 0;
    //                Player.rotateToAim();
    //                CheckCombo();
    //                mComboQueued = false;
    //            }
    //            else
    //            {
    //                Player.SetStateNormal();
    //                mIsAttacking = false;
    //                mPrimaryActive = false;
    //            }
    //        }
    //        // end attack if delay + attack time has expired
    //        else if (mAtkTimer >= m_AtkActive[mCurrentCombo] + m_AtkStartup[mCurrentCombo])
    //        {
    //            //Melee.SetActive(false);
    //        }
    //        // start attack if delay period has expired
    //        else if (mAtkTimer >= m_AtkStartup[mCurrentCombo])
    //        {
    //            //Melee.SetActive(true);
    //        }
    //    }
    //}
    private void InterruptAttack()
    {
        // Return to Normal State
        Player.SetStateNormal();

        // Clear Queued Attack Clicks
        Player.ClearRotationQueue();

        // Interrupt Melee
        //mIsAttacking = false;
        if (mPrimaryActive)
        {
            MyAnimator.SetTrigger(AnimationTriggersStatic.GetInterruptToIdle());
            Player.rb.velocity = new Vector3(0, 0, 0);
        }
        mPrimaryActive = false;
        mPrimaryStepping = false;
        Melee.SetActive(false);
        mComboQueued = false;

        // Interrupt Ranged
        isShooting = false;

        // Interrupt Stab
        mChargingStab = false;
        mStabAttacking = false;
        m_StabWeaponObject.SetActive(false);

        // Interrupt Dash
        mChargingDash = false;
        mDashAttacking = false;
        m_DashWeapon.SetActive(false);
    }

}
