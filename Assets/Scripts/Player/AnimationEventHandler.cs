using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private Shadow m_Shadow;
    public void ShadowMeleeEnable()
    {
        m_Shadow.EnablePrimaryCollider();
    }

    public void ShadowMeleeDisable()
    {
        m_Shadow.DisablePrimaryCollider();
    }

    public void ShadowPrimaryStep()
    {
        m_Shadow.PrimaryStep();
    }

    public void ShadowPrimaryEnd()
    {
        m_Shadow.EndPrimaryAttack();
    }

    public void ShadowSecondaryEnable()
    {
        m_Shadow.EnableSecondaryCollider();
    }

    public void ShadowSecondaryDisable()
    {
        m_Shadow.DisableSecondaryCollider();
    }

    public void ShadowSecondaryStep1()
    {
        m_Shadow.SecondaryStep1();
    }
    public void ShadowSecondaryStep2()
    {
        m_Shadow.SecondaryStep2();
    }

    public void ShadowSecondaryEnd()
    {
        m_Shadow.EndSecondaryAttack();
    }

    public void ShadowProjectileEnd()
    {
        m_Shadow.EndProjectileAttack();
    }

    public void ShadowProjectileFire()
    {
        m_Shadow.FireProjectile();
    }

    public void ShadowProjectileStep()
    {
        m_Shadow.RangedStep();
    }

    public void ShadowStabCharge()
    {
        m_Shadow.FireProjectile();
    }

    public void ShadowStabFire()
    {
        m_Shadow.FireProjectile();
    }

    public void ShadowDashCharge()
    {
        m_Shadow.FireProjectile();
    }

    public void ShadowDashFire()
    {
        m_Shadow.FireProjectile();
    }

    public void ShadowHealTrigger()
    {
        m_Shadow.Heal();
    }

    public void ShadowHealEnd()
    {
        m_Shadow.EndProjectileAttack();
    }

    public void DisableModel()
    {
        m_Shadow.DisableElement();
    }

    public void EndAction()
    {
        m_Shadow.EndAction();
    }
}
