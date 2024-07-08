using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] private float m_ChargeSpeed;
    [SerializeField] private float m_ChargeTurnMaxDegrees;
    [SerializeField] private GameObject m_ChargeHitbox;
    private enum AttackStage
    {
        Roaring,
        Charging,
        Recovering,
    }
    private AttackStage mCurrentStage = AttackStage.Roaring;

    protected override void DoAttack()
    {
        LocatePlayer();
        if (mCurrentStage == AttackStage.Roaring)
        {
            RotateToPlayer();
        }
        else if (mCurrentStage == AttackStage.Charging)
        {
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(playerDirectionNorm), m_ChargeTurnMaxDegrees));
            rb.velocity = transform.forward * m_ChargeSpeed;
        }
    }

    public override void SetStateAttacking()
    {
        mCurrentStage = AttackStage.Roaring;
        base.SetStateAttacking();
    }

    public void Charge()
    {
        mCurrentStage = AttackStage.Charging;
        m_ChargeHitbox.SetActive(true);
    }

    public void Recover()
    {
        mCurrentStage = AttackStage.Recovering;
        m_ChargeHitbox.SetActive(false);
    }

    public override void SetStun(float duration)
    {
        base.SetStun(duration);
        m_ChargeHitbox.SetActive(false);
    }

    public override void SetStateDead()
    {
        base.SetStateDead();
        m_ChargeHitbox.SetActive(false);
        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyDeathTrigger());
    }
}
