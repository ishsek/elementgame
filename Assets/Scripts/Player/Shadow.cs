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
    private Action[] mPrimaryActionArray = new Action[] { Action.PrimaryAttack1, Action.PrimaryAttack2, Action.PrimaryAttack3 };
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
    private Action[] mSecondaryActionArray = new Action[] { Action.SecondaryAttack1, Action.SecondaryAttack2 };


    [Header("Ranged")]
    public Transform projectileSpawn;
    public GameObject projectile;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private float projectileLife = 2f;
    [SerializeField] private float m_ShotMovementSpeed = 0.1f;
    [SerializeField] private float m_ShotMovementDuration = 0.1f;
    [SerializeField] private float m_RangedAnimationSpeed;
    [SerializeField] private AnimationCurve m_ShotMovement;
    private float mShotMovementTimer = 0;
    private bool mRangedStepping = false;
    private float mLastShot = -9999;
    private bool mAbility1Queued = false;

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
    private bool mAbility2Queued = false;

    [Header("Dash Attack")]
    [SerializeField] private GameObject m_DashWeapon;
    [SerializeField] private Weapon m_DashWeaponScript;
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
    private bool mAbility3Queued = false;

    [Header("Black Hole")]
    public GameObject VoidAim;
    [SerializeField] private GameObject m_VoidAoE;
    [SerializeField] private GameObject m_VoidAimIndicator;
    [SerializeField] private float m_VoidCD;
    private float mVoidLastCastTime = -9999;
    private bool mAimingVoid = false;
    private Vector3 AimSpawn;
    private bool mAbility4Queued = false;

    [Header("Dodging")]
    public AnimationCurve DodgeCurve;
    public AnimationCurve DodgeCurveNoAnim;
    [SerializeField] private float DodgeSpeed;
    [SerializeField] private float DodgeDuration;
    [SerializeField] private float m_DodgeCD;
    private float mLastDodgeTime = -9999;

    [Header("Stance Switching")]
    private bool mElementIsActive = false;

    [Header("Healing")]
    [SerializeField] private Health m_PlayerHP;
    [SerializeField] private float m_HealAmount;
    [SerializeField] private float m_HealCD;
    private float mLastHealTime = -9999;
    private bool mAbility5Queued = false;

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
                mPrimaryActive = true;
                Player.HaltMovement();
                Player.SetStateAttacking();
                Player.rotateToAim();
                // If within combo window, play the attack and update the combo counter
                mCurrentCombo++;
                if (Time.time > mLastPrimary + m_ComboWindow || mCurrentCombo >= mMaxCombo)
                {
                    mCurrentCombo = 0;
                }
                MyAnimator.SetTrigger(mComboList[mCurrentCombo]);
                mLastPrimary = Time.time;
            }
            else if (!mComboQueued)
            {
                mComboQueued = true;
                mCurrentCombo++;
                if (Time.time > mLastPrimary + m_ComboWindow || mCurrentCombo >= mMaxCombo)
                {
                    mCurrentCombo = 0;
                }
                //MyAnimator.SetTrigger(mComboList[mCurrentCombo]);
                mLastPrimary = Time.time;
                Player.QueueRotation();
                mActionQueue.Enqueue(mPrimaryActionArray[mCurrentCombo]);
            }
        }
    }

    public void EndPrimaryAttack()
    {
        if (mPrimaryActive)
        {
            EndAction();
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
        if (mPrimaryActive)
        {
            Melee.SetActive(false);
        }
    }

    //private void CheckCombo()
    //{
    //    // If within combo window, play the attack and update the combo counter
        
    //    if (!Player.IsNormal())
    //    {
    //        Player.QueueRotation();
    //        mActionQueue.Enqueue(mPrimaryActionArray[mSecondaryCurrentCombo]);
    //    }
    //}

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
                // Start melee hitbox timer
                mSecondaryActive = true;
                Player.HaltMovement();
                Player.SetStateAttacking();
                Player.rotateToAim();
                // If within combo window, play the attack and update the combo counter
                mSecondaryCurrentCombo++;
                if (Time.time > mLastSecondary + m_SecondaryComboWindow || mSecondaryCurrentCombo >= mSecondaryMaxCombo)
                {
                    mSecondaryCurrentCombo = 0;
                }
                MyAnimator.SetTrigger(mSecondaryComboList[mSecondaryCurrentCombo]);
                mLastSecondary = Time.time;
            }
            else if (!mSecondaryComboQueued)
            {
                mSecondaryComboQueued = true;
                mSecondaryCurrentCombo++;
                if (Time.time > mLastSecondary + m_SecondaryComboWindow || mSecondaryCurrentCombo >= mSecondaryMaxCombo)
                {
                    mSecondaryCurrentCombo = 0;
                }
                //MyAnimator.SetTrigger(mSecondaryComboList[mSecondaryCurrentCombo]);
                mLastSecondary = Time.time;
                Player.QueueRotation();
                mActionQueue.Enqueue(mSecondaryActionArray[mSecondaryCurrentCombo]);
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
            EndAction();
        }
    }

    //private void CheckSecondaryCombo()
    //{
    //    // If within combo window, play the attack and update the combo counter
    //    mSecondaryCurrentCombo++;
    //    if (Time.time > mLastSecondary + m_SecondaryComboWindow || mSecondaryCurrentCombo >= mSecondaryMaxCombo)
    //    {
    //        mSecondaryCurrentCombo = 0;
    //        mSecondaryComboQueued = false;
    //    }
    //    MyAnimator.SetTrigger(mSecondaryComboList[mSecondaryCurrentCombo]);
    //    mLastSecondary = Time.time;
    //    //mSecondaryComboTimer = 0f;
    //}

    public void SecondaryStep1()
    {
        mSecondaryMovementTimer = 0;
        mSecondaryCurrentStep = 0;
        mSecondaryStepping = true;
    }

    public void SecondaryStep2()
    {
        mSecondaryMovementTimer = 0;
        mSecondaryCurrentStep = 1;
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
                Player.GetSkillButtonListController()?.ChangeButtonState(SkillButtonListController.SkillButtonsIndeces.Skill3Button,
                                                                        SkillButtonController.SkillButtonStates.Cooldown, 
                                                                        newCooldownTime: shootCooldown);
            }
            else if (!mAbility1Queued)
            {
                mAbility1Queued = true;
                //MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowProjectile());
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
                Player.rb.velocity = transform.forward * m_ShotMovementSpeed * m_ShotMovement.Evaluate(MovePercentage * m_RangedAnimationSpeed);
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
            if (context.performed && !mChargingStab)
            {
                if (Player.IsNormal())
                {
                    MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowStabCharge());
                    Player.HaltMovement();
                    Player.SetStateAttacking();
                    mStabChargeTime = 0;
                    mChargingStab = true;
                }
                else if (!mAbility2Queued)
                {
                    mAbility2Queued = true;
                    mActionQueue.Enqueue(Action.Ability2);
                }
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
                Player.GetSkillButtonListController()?.ChangeButtonState(SkillButtonListController.SkillButtonsIndeces.Skill1Button, 
                                                                        SkillButtonController.SkillButtonStates.Cooldown, 
                                                                        newCooldownTime: m_StabCD);
            }
        }
    }

    private void ChargeStab()
    {
        if (mChargingStab)
        {
            Player.rotateToAim();
            mStabChargeTime += Time.deltaTime;
            Player.GetSkillButtonListController()?.ChangeButtonState(SkillButtonListController.SkillButtonsIndeces.Skill1Button, 
                                                                    SkillButtonController.SkillButtonStates.Charging, 
                                                                    chargePercentage: mStabChargeTime / m_StabMaxChargeTime);
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
            if (context.performed && !mChargingDash)
            {
                if (Player.IsNormal())
                {
                    MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowDashCharge());
                    Player.HaltMovement();
                    Player.SetStateAttacking();
                    mDashChargeTime = 0;
                    mChargingDash = true;
                }
                else if (!mAbility3Queued)
                { 
                    mAbility3Queued = true;
                    mActionQueue.Enqueue(Action.Ability3);
                }
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
                Player.GetSkillButtonListController()?.ChangeButtonState(SkillButtonListController.SkillButtonsIndeces.Skill4Button, 
                                                                        SkillButtonController.SkillButtonStates.Cooldown, 
                                                                        newCooldownTime: m_DashCD);
            }
        }
    }
    private void ChargeDash()
    {
        if (mChargingDash)
        {
            Player.rotateToAim();
            mDashChargeTime += Time.deltaTime;
            Player.GetSkillButtonListController()?.ChangeButtonState(SkillButtonListController.SkillButtonsIndeces.Skill4Button, 
                                                                    SkillButtonController.SkillButtonStates.Charging, 
                                                                    chargePercentage : (mDashChargeTime / m_DashMaxChargeTime));
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
                //Player.SetStateNormal(); moved to EndAction animation event
                Player.EnableEnemyCollision();
            }
        }
    }

    //Blackhole
    public void Ability4(InputAction.CallbackContext context)
    {
        if (Time.time > mVoidLastCastTime + m_VoidCD)
        {
            if (context.performed && !mAimingVoid)
            {
                if (Player.IsNormal())
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
                else if (!mAbility4Queued)
                { 
                    mAbility4Queued = true;
                    mActionQueue.Enqueue(Action.Ability4);
                }
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
                    Player.GetSkillButtonListController()?.ChangeButtonState(SkillButtonListController.SkillButtonsIndeces.Skill2Button, 
                                                                            SkillButtonController.SkillButtonStates.Cooldown, 
                                                                            newCooldownTime: m_VoidCD); 
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
                // Add UI ref if needed
                // Something needs to be added here to reset any properties on the currently charging attack if there was one interrupted.
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
                if (Player.IsNormal())
                {
                    mLastHealTime = Time.time;
                    InterruptAttack();
                    Player.HaltMovement();
                    Player.SetStateAttacking();
                    MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowHeal());
                }
                else if (!mAbility5Queued)
                {
                    mAbility5Queued = true;
                    mActionQueue.Enqueue(Action.Ability5);
                }
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
                    MyAnimator.SetTrigger(mComboList[0]);
                    Player.RotateToQueuedClick();
                    mPrimaryActive = true;
                    mComboQueued = false;
                    break;
                case Action.PrimaryAttack2:
                    MyAnimator.SetTrigger(mComboList[1]);
                    Player.RotateToQueuedClick();
                    mPrimaryActive = true;
                    mComboQueued = false;
                    break;
                case Action.PrimaryAttack3:
                    MyAnimator.SetTrigger(mComboList[2]);
                    Player.RotateToQueuedClick();
                    mPrimaryActive = true;
                    mComboQueued = false;
                    break;
                case Action.SecondaryAttack1:
                    MyAnimator.SetTrigger(mSecondaryComboList[0]);
                    Player.RotateToQueuedClick();
                    mSecondaryActive = true;
                    mSecondaryComboQueued = false;
                    break;
                case Action.SecondaryAttack2:
                    MyAnimator.SetTrigger(mSecondaryComboList[1]);
                    Player.RotateToQueuedClick();
                    mSecondaryActive = true;
                    mSecondaryComboQueued = false;
                    break;
                case Action.Ability1:
                    MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowProjectile());
                    mAbility1Queued = false;
                    Player.RotateToQueuedClick();
                    break;
                case Action.Ability2:
                    MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowStabCharge());
                    mStabChargeTime = 0;
                    mChargingStab = true;
                    mAbility2Queued = false;
                    break;
                case Action.Ability3:
                    MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowDashCharge());
                    mDashChargeTime = 0;
                    mChargingDash = true;
                    mAbility3Queued = false;
                    break;
                case Action.Ability4:
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
                    mAbility4Queued = false;
                    break;
                case Action.Ability5:
                    mLastHealTime = Time.time;
                    MyAnimator.SetTrigger(AnimationTriggersStatic.GetShadowHeal());
                    mAbility5Queued = false;
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
                    mComboQueued = false;
                    break;
                case Action.PrimaryAttack2:
                    MyAnimator.ResetTrigger(mComboList[1]);
                    mComboQueued = false;
                    break;
                case Action.PrimaryAttack3:
                    MyAnimator.ResetTrigger(mComboList[2]);
                    mComboQueued = false;
                    break;
                case Action.SecondaryAttack1:
                    MyAnimator.ResetTrigger(mSecondaryComboList[0]);
                    mSecondaryComboQueued = false;
                    break;
                case Action.SecondaryAttack2:
                    MyAnimator.ResetTrigger(mSecondaryComboList[1]);
                    mSecondaryComboQueued = false;
                    break;
                case Action.Ability1:
                    MyAnimator.ResetTrigger(AnimationTriggersStatic.GetShadowProjectile());
                    mAbility1Queued = false;
                    break;
                case Action.Ability2:
                    MyAnimator.ResetTrigger(AnimationTriggersStatic.GetShadowStabCharge());
                    mAbility2Queued = false;
                    break;
                case Action.Ability3:
                    MyAnimator.ResetTrigger(AnimationTriggersStatic.GetShadowDashCharge());
                    mAbility3Queued = false;
                    break;
                case Action.Ability4:
                    MyAnimator.ResetTrigger(AnimationTriggersStatic.GetShadowBlackholeCharge());
                    mAbility4Queued = false;
                    break;
                case Action.Ability5:
                    break;
            }
        }

        // Reset all primary attack values
        if (mPrimaryActive)
        {
            Player.HaltMovement();
            mPrimaryActive = false;
            mPrimaryStepping = false;
            Melee.SetActive(false);
        }
        //Player.HaltMovement();

        if (mSecondaryActive)
        {
            //MyAnimator.SetTrigger(AnimationTriggersStatic.GetInterruptToIdle());
            MyAnimator.ResetTrigger(mSecondaryComboList[mSecondaryCurrentCombo]);
            Player.rb.velocity = new Vector3(0, 0, 0);
            mSecondaryActive = false;
            mSecondaryStepping = false;
            Secondary.SetActive(false);
        }

        // Interrupt Ranged Attack
        if (mRangedStepping)
        {
            Player.HaltMovement();
            mRangedStepping = false;
        }
        

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
