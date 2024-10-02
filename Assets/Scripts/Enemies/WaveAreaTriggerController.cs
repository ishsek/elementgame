using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAreaTriggerController : MonoBehaviour
{
    [SerializeField] private WaveAreaController m_WaveAreaController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && m_WaveAreaController.CheckWavesNeedStart())
        {
            m_WaveAreaController.WaveStartTriggered();
        }
    }
}
