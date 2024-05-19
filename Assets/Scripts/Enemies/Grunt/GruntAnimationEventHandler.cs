using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GruntAnimationEvents : MonoBehaviour
{
    [SerializeField] private Grunt m_Grunt;
    public void MeleeEnable()
    {
        m_Grunt.EnableAttackHitbox();
    }
    public void MeleeDisable()
    {
        m_Grunt.DisableAttackHitbox();
    }

    public void EndAttack()
    {
        m_Grunt.EndAttack();
    }
}