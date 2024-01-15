using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAttack : MonoBehaviour
{
    public GameObject Melee;
    bool isAttacking = false;
    public float atkDuration = 0.3f;
    float atkTimer = 0f;
    public float atkDelay = 0.3f;

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
        }
    }

    void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            atkTimer += Time.deltaTime;
            // attack delay
            if (atkTimer >= atkDelay)
            {
                Melee.SetActive(true);
            }
            // launch actual attack
            else if (atkTimer >= atkDuration + atkDelay)
            {
                atkTimer = 0;
                isAttacking = false;
                Melee.SetActive(false);
            }
        }
    }
}
