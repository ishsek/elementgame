using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRapidTriggerController : MonoBehaviour
{
    [SerializeField] private Transform m_PushDirectionTransform;
    [SerializeField] private float m_PushForce;

    private Rigidbody mPlayerRigidbodyRef;

    private void Update()
    {
        if (mPlayerRigidbodyRef != null)
        {
            mPlayerRigidbodyRef.AddForce(m_PushDirectionTransform.forward * m_PushForce);
            Debug.Log("m_push = " + m_PushDirectionTransform.forward + ", m_force = " + m_PushForce + ", m_total =" + m_PushDirectionTransform.forward * m_PushForce);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
        if (other.tag == "Player")
        {
            mPlayerRigidbodyRef = other.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            mPlayerRigidbodyRef = null;
        }
    }
}
