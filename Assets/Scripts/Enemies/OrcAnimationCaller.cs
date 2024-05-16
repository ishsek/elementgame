using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrcAnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject m_MeleeHitbox;
    [SerializeField] private Orc m_Orc;
    public void MeleeEnable()
    {
        m_MeleeHitbox.SetActive(true);
    }
    public void MeleeDisable()
    {
        m_MeleeHitbox.SetActive(false);
    }

    public void EndAttack()
    {
        m_Orc.EndAttack();
    }
}