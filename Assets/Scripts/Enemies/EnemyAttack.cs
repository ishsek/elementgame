using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAttack : MonoBehaviour
{
    public Animator EnemyAnimator;

    public GameObject Melee;
    bool isAttacking = false;
    public float atkDuration = 0.3f;
    float atkTimer = 0f;
    public float atkDelay = 0.3f;
    public float cooldown = 1f; //seconds
    private float lastAttackedAt = -9999f;

    public Enemy Attacker;

    // Update is called once per frame
    void Update()
    {
        CheckMeleeTimer();
    }

    public void OnAttack()
    {
        if (!isAttacking)
        {
            //Melee.SetActive(true);
            isAttacking = true;
            // Call animator to play melee attack here
            if (EnemyAnimator != null)
            {
                EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyAttackTrigger());
            }

            Attacker.Immobilize();
        }
    }

    void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            // Check attack cooldown
            if (Time.time > lastAttackedAt + cooldown)
            {
                // Start attack process
                atkTimer += Time.deltaTime;
                // end attack if delay + attack time has expired
                if (atkTimer >= atkDuration + atkDelay)
                {
                    atkTimer = 0;
                    isAttacking = false;
                    Melee.SetActive(false);
                    Attacker.Mobilize();
                    // Set timer for attack cooldown
                    lastAttackedAt = Time.time;
                }
                // start attack if delay period has expired
                else if (atkTimer >= atkDelay)
                {
                    Melee.SetActive(true);
                }
            }
        }
    }
}
