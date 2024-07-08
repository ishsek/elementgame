using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerAnimEvents : MonoBehaviour
{
    [SerializeField] private Charger m_Charger;

    public void StartCharging()
    {
        m_Charger.Charge();
    }

    public void StopCharging()
    {
        m_Charger.Recover();
    }

    public void EndAttack()
    {
        m_Charger.EndAttack();
    }
}
