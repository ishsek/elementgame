using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] GameObject m_Model;
    private bool mElementIsActive = false;
    public void EnableElement()
    {
        m_Model.SetActive(true);
        mElementIsActive = true;
        //Player.DodgeCurve = DodgeCurveNoAnim;
        //Player.DodgeDuration = DodgeDuration;
        //Player.DodgeSpeed = DodgeSpeed;
    }

    public void DisableElement()
    {
        mElementIsActive = false;
        m_Model.SetActive(false);
    }
}
