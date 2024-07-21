using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Shadow : MonoBehaviour
{
    [Header("References")]
    public PlayerController Player;
    public Animator MyAnimator;
    [SerializeField] GameObject m_ShadowModel;
    private enum Action
    {
        PrimaryAttack1,
        PrimaryAttack2,
        PrimaryAttack3,
        SecondaryAttack1,
        SecondaryAttack2,
        Ability1,
        Ability2,
        Ability3,
        Ability4,
        Ability5,
    }

    [Header("Melee")]
    public GameObject Melee;
    [SerializeField] private float m_PrimaryAttackMoveSpeed;
    [SerializeField] private List<AnimationCurve> m_PrimaryAttackCurves;
    [SerializeField] private float m_PrimaryStepSpeed;
    [SerializeField] private float m_PrimaryStepDuration;
    [SerializeField] private AnimationCurve m_PrimaryStepCurve;
    [SerializeField] private float m_ComboWindow = 2f;
    private float mAttackMovementTimer = 0;
    private bool mPrimaryStepping = false;
    private float mLastPrimary = -9999;
    private int mCurrentCombo = 0;
    private int mMaxCombo = 3;
    private bool mComboQueued = false;
    private bool mPrimaryActive = false;
    private string[] mComboList = new string[] { AnimationTriggersStatic.GetPlayerAttack1(), AnimationTriggersStatic.GetPlayerAttack2(), AnimationTriggersStatic.GetPlayerAttack3() };
    private Queue<Action> mActionQueue = new Queue<Action>();

    [Header("Secondary")]
    public GameObject Secondary;
    [SerializeField] private List<AnimationCurve> m_SecondaryStepCurves;
    [SerializeField] private List<float> m_SecondaryStepSpeed;
    [SerializeField] private List<float> m_SecondaryStepDuration;
    [SerializeField] private float m_SecondaryComboWindow = 2f;
    private float mSecondaryMovementTimer = 0;
    private bool mSecondaryStepping = false;
    private int mSecondaryCurrentStep = 0;
    private float mLastSecondary = -9999;
    private int mSecondaryCurrentCombo = 0;
    private int mSecondaryMaxCombo = 2;
    private bool mSecondaryComboQueued = false;
    private bool mSecondaryActive = false;
    private string[] mSecondaryComboList = new string[] { AnimationTriggersStatic.GetShadowHeavy1(), AnimationTriggersStatic.GetShadowHeavy2(), };


    [Header("Ranged")]
    public Transform projectileSpawn;
    public GameObject projectile;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private float projectileLife = 2f;
    [SerializeField] private float m_ShotMovementSpeed = 0.1f;
    [SerializeField] private float m_ShotMovementDuration = 0.1f;
    [SerializeField] private AnimationCurve m_ShotMovement;
    [SerializeField] private SkillButtonController m_RangedAttackUI;
    private float mShotMovementTimer = 0;
    private bool mRangedStepping = false;
    private float mLastShot = -9999;

    [Header("Stab")]
    [SerializeField] private GameObject m_StabWeaponObject;
    [SerializeField] private Weapon m_StabWeaponScript;
    [SerializeField] private float m_StabCD;
    [SerializeField] private float m_StabMaxChargeTime;
    [SerializeField] private float m_StabMaxDamage;
    [SerializeField] private float m_StabBaseSpeed;
    [SerializeField] private float m_StabDuration;
    [SerializeField] private AnimationCurve StabAttackCurve;
    [SerializeField] private SkillButtonController m_StabAttackUI;
    private float mStabChargeTime;
    private float mStabLastCast = -99999;
    private float mStabTime = 0;
    private bool mChargingStab = false;
    private bool mStabAttacking = false;

    [Header("Dash Attack")]
    [SerializeField] private GameObject m_DashWeapon;
    [SerializeField] private Weapon m_DashWeaponScript;
    [SerializeField] private float m_DashCD;
    [SerializeField] private float m_DashMaxChargeTime;
    [SerializeField] private float m_DashMaxDamage;
    [SerializeField] private float m_DashMaxLength;
    [SerializeField] private float m_DashBaseSpeed;
    [SerializeField] private AnimationCurve DashAttackCurve;
    [SerializeField] private SkillButtonController m_DashAttackUI;
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
    [SerializeField] private SkillButtonController m_BlackHoleUI;
    private float mVoidLastCastTime = -9999;
    private bool mAimingVoid = false;
    private Vector3 AimSpawn;

    [Header("Dodging")]
    public AnimationCurve DodgeCurve;
    public AnimationCurve DodgeCurveNoAnim;
    [SerializeField] private float DodgeSpeed;
    [SerializeField] private float DodgeDuration;
    [SerializeField] private float m_DodgeCD;
    [SerializeField] private SkillButtonController m_DodgeUI;
    private float mLastDodgeTime = -9999;

    [Header("Stance Switching")]
    private bool mElementIsActive = false;

    [Header("Healing")]
    [SerializeField] private Health m_PlayerHP;
    [SerializeField] private float m_HealAmount;
    [SerializeField] private float m_HealCD;
    private float mLastHealTime = -9999;

    void Update()
    {
        if (mElementIsActive)
        {
            HandlePrimaryStep();
            HandleSecondaryStep();
            HandleRangedStep();
            ChargeDash();
            HandleDashAttack();
            ChargeStab();
            HandleStab();
        }
    }

    public void EnableShadowElement()
    {
        m_ShadowModel.SetActive(true);
        mElementIsActive = true;
        Player.DodgeCurve = DodgeCurveNoAnim;
        Player.DodgeDuration = DodgeDuration;
        Player.DodgeSpeed = DodgeSpeed;
    }

    public void DisableElement()
    {
        mElementIsActive = false;
        //MyAnimator.SetTrigger(AnimationTriggersStatic.GetDisableElement());
        m_ShadowModel.SetActive(false);
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
            }
            CheckCombo();
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
                mPrimaryActive = false;
                EndAction();
            }
            
        }
    }

    private void CheckCombo()
    {
        // If within combo window, play the attack and update the combo counter
        mPrimaryActive = true;
        mCurrentCombo++;
        if (Time.time > mLastPrimary + m_ComboWindow || mCurrentCombo >= mMaxCombo)
        {
            mCurrentCombo = 0;
            mComboQueued = false;
        }
        MyAnimator.SetTrigger(mComboList[mCurrentCombo]);
        mLastPrimary = Time.time;
        if (!Player.IsNormal())
        {
            Player.QueueRotation();
            switch (mCurrentCombo)
            {
                case 0:
                    mActionQueue.Enqueue(Action.PrimaryAttack1);
                    break;

                case 1:
                    mActionQueue.Enqueue(Action.PrimaryAttack2);
                    break;

                case 2:
                    mActionQueue.Enqueue(Action.PrimaryAttack3);
                    break;
            }
        }
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
        if (context.performed)
        {
            if (Player.IsNormal())
            {
                Player.HaltMovement();
                Player.SetStateAttacking();
                Player.rotateToAim();
                // Start melee hitbox timer
                mSecondaryActive = true;
                //mAtkTimer = 0;
                CheckSecondaryCombo();
            }
            else if (mSecondaryActive && !mSecondaryComboQueued)
            {
                if (mSecondaryCurrentCombo + 1 < mSecondaryMaxCombo)
                {
                    mSecondaryComboQueued = true;
                    Player.QueueRotation();
                    CheckSecondaryCombo();
                }
            }
        }
    }

    public void EnableSecondaryCollider()
    {
        if (mSecondaryActive)
        {
            Secondary.SetActive(true);
        }
    }

    public void DisableSecondaryCollider()
    {
        Secondary.SetActive(false);
    }

    public void EndSecondaryAttack()
    {
        if (mSecondaryActive)
        {
            if (mSecondaryComboQueued)
            {
                //mAtkTimer = 0;
                Player.RotateToQueuedClick();
                mSecondaryComboQueued = false;
            }
            else
            {
                Player.SetStateNormal();
                mSecondaryActive = false;
            }
        }
    }

    private void CheckSecondaryCombo()
    {
        // If within combo window, play the attack and update the combo counter
        mSecondaryCurrentCombo++;
        if (Time.time > mLastSecondary + m_SecondaryComboWindow || mSecondaryCurrentCombo >= mSecondaryMaxCombo)
        {
            mSecondaryCurrentCombo = 0;
            mSecondaryComboQueued = false;
        }
        MyAnimator.SetTrigger(mSecondaryComboList[mSecondaryCurrentCombo]);
        mLastSecondary = Time.time;
        //mSecondaryComboTimer = 0f;
    }

    public void SecondaryStep()
    {
        mSecondaryMovementTimer = 0;
        if (mSecondaryComboQueued)
        {
            mSecondaryCurrentStep = mSecondaryCurrentCombo - 1;
        }
        else
        {
            mSecondaryCurrentStep = mSecondaryCurrentCombo;
        }
        mSecondaryStepping = true;
    }

    private void HandleSecondaryStep()
    {
        if (mSecondaryActive && mSecondaryStepping)
        {
            if (mSecondaryMovementTimer < m_SecondaryStepDuration[mSecondaryCurrentStep])
            {
                mSecondaryMovementTimer += Time.deltaTime;
                float MovePercentage = mSecondaryMovementTimer / m_SecondaryStepDuration[mSecondaryCurrentStep];
                Player.rb.velocity = transform.forward * m_SecondaryStepSpeed[mSecondaryCurrentStep] * m_SecondaryStepCurves[mSecondaryCurrentStep].Evaluate(MovePercentage);
            }
            else
            {
                mSecondaryStepping = false;
            }
        }
    }

    //Projectile
    public void Ability1(InputAction.CallbackContext context)
    {
        if (Time.time > (mLastShot + shootCooldown) && context.performed)
        {
            if (Player.IsNormal())
            {
                Player.HaltMovement();
                Player.SetStateAttacking();
                Player.rotateToAim();
                MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowProjectile());
                m_RangedAttackUI?.ChangeButtonState(SkillButtonController.SkillButtonStates.Cooldown, shootCooldown);
            }
            else
            {
                MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowProjectile());
                mActionQueue.Enqueue(Action.Ability1);
                Player.QueueRotation();
            }
        }
    }

    public void FireProjectile()
    {
        mLastShot = Time.time;
        GameObject intProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
        intProjectile.GetComponent<Rigidbody>().AddForce(projectileSpawn.forward * fireForce, ForceMode.Impulse);
        Destroy(intProjectile, projectileLife);
    }

    public void EndProjectileAttack()
    {
        Player.SetStateNormal();
    }

    public void HandleRangedStep()
    {
        if (mRangedStepping)
        {
            if (mShotMovementTimer < m_ShotMovementDuration)
            {
                mShotMovementTimer += Time.deltaTime;
                float MovePercentage = mShotMovementTimer / m_ShotMovementDuration;
                Player.rb.velocity = transform.forward * m_ShotMovementSpeed * m_ShotMovement.Evaluate(MovePercentage);
            }
            else
            {
                mRangedStepping = false;
            }
        }
    }

    public void RangedStep()
    {
        mShotMovementTimer = 0;
        mRangedStepping = true;
    }
    //Stab
    public void Ability2(InputAction.CallbackContext context)
    {
        if (Time.time > mStabLastCast + m_StabCD)
        {
            if (Player.IsNormal() && context.performed && !mChargingStab)
            {
                MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowStabCharge());
                Player.HaltMovement();
                Player.SetStateAttacking();
                mStabChargeTime = 0;
                mChargingStab = true;
            }
            else if (mChargingStab && context.canceled)
            {
                MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowStabFire());
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
                m_StabAttackUI?.ChangeButtonState(SkillButtonController.SkillButtonStates.Cooldown, m_StabCD);
            }
        }
    }

    private void ChargeStab()
    {
        if (mChargingStab)
        {
            Player.rotateToAim();
            mStabChargeTime += Time.deltaTime;
            m_StabAttackUI?.ChangeButtonState(SkillButtonController.SkillButtonStates.Charging, chargePercentage: mStabChargeTime / m_StabMaxChargeTime);
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
                MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowDashCharge());
                Player.HaltMovement();
                Player.SetStateAttacking();
                mDashChargeTime = 0;
                mChargingDash = true;
            }
            else if (mChargingDash && context.canceled)
            {
                MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowDashFire());
                mChargingDash = false;
                mDashLastCast = Time.time;
                if (mDashChargeTime > m_DashMaxChargeTime)
                {
                    mDashChargeTime = m_DashMaxChargeTime;
                }
                mDashDuration = mDashChargeTime / m_DashMaxChargeTime * m_DashMaxLength;
                m_DashWeaponScript.damage = mDashChargeTime / m_DashMaxChargeTime * m_DashMaxDamage;
                Player.DisableEnemyCollision();
                mDashTime = 0;
                mDashAttacking = true;
                m_DashWeapon.SetActive(true);
                m_DashAttackUI?.ChangeButtonState(SkillButtonController.SkillButtonStates.Cooldown, m_DashCD);
            }
        }
    }
    private void ChargeDash()
    {
        if (mChargingDash)
        {
            Player.rotateToAim();
            mDashChargeTime += Time.deltaTime;
            m_DashAttackUI.ChangeButtonState(SkillButtonController.SkillButtonStates.Charging, chargePercentage : (mDashChargeTime / m_DashMaxChargeTime));
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

    //Blackhole
    public void Ability4(InputAction.CallbackContext context)
    {
        if (Time.time > mVoidLastCastTime + m_VoidCD)
        {
            if (Player.IsNormal() && context.performed && !mAimingVoid)
            {
                Player.HaltMovement();
                MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowBlackholeCharge());
                mAimingVoid = true;
                Player.SetStateControllerAiming();
                if (Player.isGamepad)
                {
                    AimSpawn = transform.position;
                }
                else
                {
                    AimSpawn = Player.aimDirection;
                }
                AimSpawn.y = 0;
                VoidAim = Instantiate(m_VoidAimIndicator, AimSpawn, transform.rotation);
                Player.AimPreview = VoidAim;
            }
            else if (mAimingVoid && context.canceled)
            {
                if (VoidAim != null)
                {
                    MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowBlackholeFire());
                    GameObject InstBlackHole = Instantiate(m_VoidAoE, VoidAim.transform.position, VoidAim.transform.rotation);
                    Player.SetStateNormal();
                    Player.AimPreview = null;
                    Destroy(VoidAim);
                    mAimingVoid = false;
                    mVoidLastCastTime = Time.time;
                    m_BlackHoleUI?.ChangeButtonState(SkillButtonController.SkillButtonStates.Cooldown, m_VoidCD);
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
                DodgeInterrupt();
                Player.OnDodge();
                m_DodgeUI?.ChangeButtonState(SkillButtonController.SkillButtonStates.Cooldown, m_DodgeCD);
            }
        }
    }

    public void Heal()
    {
        m_PlayerHP.Heal(m_HealAmount);
    }

    public void CastHeal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Time.time > mLastHealTime + m_HealCD)
            {
                mLastHealTime = Time.time;
                InterruptAttack();
                Player.HaltMovement();
                Player.SetStateAttacking();
                MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowHeal());
            }
        }
    }

    public void EndAction()
    {
        if (mActionQueue.TryDequeue(out Action NextAction))
        {
            if (!Player.IsAttacking())
            {
                Player.SetStateAttacking();
                Player.HaltMovement();
            }
            switch(NextAction)
            {
                case Action.PrimaryAttack1:
                    Player.RotateToQueuedClick();
                    break;
                case Action.PrimaryAttack2:
                    Player.RotateToQueuedClick();
                    break;
                case Action.PrimaryAttack3:
                    Player.RotateToQueuedClick();
                    break;
                case Action.SecondaryAttack1:
                    Player.RotateToQueuedClick();
                    break;
                case Action.SecondaryAttack2:
                    Player.RotateToQueuedClick();
                    break;
                case Action.Ability1:
                    Player.RotateToQueuedClick();
                    break;
                case Action.Ability2:
                    break;
                case Action.Ability3:
                    break;
                case Action.Ability4:
                    break;
                case Action.Ability5:
                    break;
            }
        }
        else
        {
            Player.SetStateNormal();
        }
    }

    private void DodgeInterrupt()
    {
        InterruptAttack();
        //MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowDodge());
    }
    private void InterruptAttack()
    {
        MyAnimator.SetTrigger("InterruptAttack");

        // Return to Normal State
        Player.SetStateNormal();

        // Clear Queued Attack Clicks
        Player.ClearRotationQueue();

        while (mActionQueue.Any())
        {
            Action action = mActionQueue.Dequeue();
            switch (action)
            {
                case Action.PrimaryAttack1:
                    MyAnimator.ResetTrigger(mComboList[0]);
                    break;
                case Action.PrimaryAttack2:
                    MyAnimator.ResetTrigger(mComboList[1]);
                    break;
                case Action.PrimaryAttack3:
                    MyAnimator.ResetTrigger(mComboList[2]);
                    break;
                case Action.SecondaryAttack1:
                    MyAnimator.ResetTrigger(mSecondaryComboList[0]);
                    break;
                case Action.SecondaryAttack2:
                    MyAnimator.ResetTrigger(mSecondaryComboList[1]);
                    break;
                case Action.Ability1:
                    
                    break;
                case Action.Ability2:
                    break;
                case Action.Ability3:
                    break;
                case Action.Ability4:
                    break;
                case Action.Ability5:
                    break;
            }
        }

        // Interrupt Melee
        //mIsAttacking = false;
        if (mPrimaryActive)
        {
            //MyAnimator.SetTrigger(AnimationTriggersStatic.GetInterruptToIdle());
            MyAnimator.ResetTrigger(mComboList[mCurrentCombo]);
            Player.HaltMovement();
            mPrimaryActive = false;
            mPrimaryStepping = false;
            Melee.SetActive(false);
            mComboQueued = false;
        }

        if (mSecondaryActive)
        {
            //MyAnimator.SetTrigger(AnimationTriggersStatic.GetInterruptToIdle());
            MyAnimator.ResetTrigger(mSecondaryComboList[mSecondaryCurrentCombo]);
            Player.rb.velocity = new Vector3(0, 0, 0);
            mSecondaryActive = false;
            mSecondaryStepping = false;
            Secondary.SetActive(false);
            mSecondaryComboQueued = false;
        }

        // Interrupt Ranged Attack
        

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
