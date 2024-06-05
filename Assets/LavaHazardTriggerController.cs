using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaHazardTriggerController : MonoBehaviour
{
    [SerializeField] private LavaJetHazardController m_LavaJetHazardController;
    [SerializeField] private float m_DamagePerSecond = 10;

    private Health playerHealthRef;
    private bool playerEntered = false;

    private void Update()
    {
        if ((playerEntered == true) && (m_LavaJetHazardController.GetFirePlaying() == true))
        {
            playerHealthRef?.TakeDamage(m_DamagePerSecond * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = true;
            playerHealthRef = other.GetComponent<Health>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = false;
        }
    }
}
