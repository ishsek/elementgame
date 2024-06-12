using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    [SerializeField] private GameObject m_AttackHitbox;
    public override void SetStun(float duration)
    {
        base.SetStun(duration);
        m_AttackHitbox.SetActive(false);
    }

    public override void SetStateDead()
    {
        base.SetStateDead();
        m_AttackHitbox.SetActive(false);
        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyDeathTrigger());
    }
}