using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private Mage m_Mage;

    public void Aim()
    {
        m_Mage.Aim();
    }

    public void LockedOn()
    {
        m_Mage.LockedOn();
    }

    public void Fire()
    {
        m_Mage.Fire();
    }

    public void EndAttack()
    {
        m_Mage.EndAttack();
    }
}
