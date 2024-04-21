using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private Shadow m_Shadow;
    public void ShadowMeleeEnable()
    {
        m_Shadow.EnablePrimaryCollider();
    }

    public void ShadowMeleeDisable()
    {
        m_Shadow.DisablePrimaryCollider();
    }

    public void ShadowPrimaryStep()
    {
        m_Shadow.PrimaryStep();
    }
    public void ShadowPrimaryEnd()
    {
        m_Shadow.EndPrimaryAttack();
    }
}
