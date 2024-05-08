using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    [SerializeField] private GameObject m_AttackHitbox;
    public override void SetStun(float duration)
    {
        base.SetStun(duration);
        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyInterruptTrigger());
        if (mMoving)
        {
            mMoving = false;
        }
        m_AttackHitbox.SetActive(false);
    }

    public override void SetStateAgro()
    {
        base.SetStateAgro();
        if (!mMoving)
        {
            EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
            mMoving = true;
        }   
    }
  

    public override void SetStateLeash()
    {
        base.SetStateLeash();
        if (!mMoving)
        {
            EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
            mMoving = true;
        }
    }

    public override void SetStateAttacking()
    {
        if (state != State.Attacking)
        {
            EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyAttackTrigger());
            mMoving = false;
            base.SetStateAttacking();
        }
    }

    public override void SetStateNormal()
    {
        base.SetStateNormal();
        if (mMoving)
        {
            EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyIdleTrigger());
            mMoving = false;
        }
    }
    public void EndAttack()
    {
        if (state == State.Attacking)
        {
            SetStateTargeting();
        }
    }
}