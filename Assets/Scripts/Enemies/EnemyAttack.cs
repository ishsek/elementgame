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
    public float atkRecovery = 0.3f;
    public float cooldown = 1f; //seconds
    private float cdTimer = 9999f;

    public Enemy Attacker;

    // Update is called once per frame
    void Update()
    {
        CheckMeleeTimer();
        cdTimer += Time.deltaTime;
    }

    public void OnAttack()
    {
        if (!isAttacking)
        {
            // Check cd
            if (cdTimer > cooldown)
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
    }

    private void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            // Start attack process
            atkTimer += Time.deltaTime;

            // Release character once recovery window has expired
            if (atkTimer >= atkDuration + atkDelay + atkRecovery)
            {
                Attacker.Mobilize();
                isAttacking = false;
                atkTimer = 0;
                cdTimer = 0;
            }
            // end attack if delay + attack time has expired
            else if (atkTimer >= atkDuration + atkDelay)
            {
                Melee.SetActive(false);
            }
            // start attack if delay period has expired
            else if (atkTimer >= atkDelay)
            {
                Melee.SetActive(true);
            }
        }
    }
}
