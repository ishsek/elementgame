using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    public override void SetStun(float duration)
    {
        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyIdleTrigger());
        if (state == State.Attacking)
        {
            EnemyAnimator.ResetTrigger(AnimationTriggersStatic.GetEnemyAttackTrigger());
        }
        EnemyAnimator.ResetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
        base.SetStun(duration);
    }

    public override void SetStateAgro()
    {
        base.SetStateAgro();
        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
    }
  

    public override void SetStateLeash()
    {
        base.SetStateLeash();
        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
    }

    public override void SetStateAttacking()
    {
        base.SetStateAttacking();
        EnemyAnimator.ResetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyAttackTrigger());
    }

    public override void SetStateNormal()
    {
        base.SetStateNormal();
        EnemyAnimator.ResetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyIdleTrigger());
    }
    public void EndAttack()
    {
        SetStateNormal();
    }
}