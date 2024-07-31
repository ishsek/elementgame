using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaJetHazardController : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_LavaParticles;
    [SerializeField] private ParticleSystem m_LavaAnticipator;

    [SerializeField] private bool m_ExternallyControlled = false;
    [SerializeField] private float m_TimeBetweenFires = 5f;
    [SerializeField] private float m_AnticipatorDuration = 1;
    [SerializeField] private float m_FlameDuration = 1;

    private float mCurrentTime = 0;
    private bool mFirePlaying = false;
    private bool mFireAnimActivated = false;
    private bool mFireExternallyActivated = false;

    // Update is called once per frame
    void Update()
    {
        if (m_ExternallyControlled == true)
        {
            if (mFireExternallyActivated == true)
            {
                UpdateFire();
            }
        }
        else
        {
            UpdateFire();
        }
    }

    private void UpdateFire()
    {
        mCurrentTime += Time.deltaTime;

        if ((mFirePlaying == false) && (mCurrentTime >= m_TimeBetweenFires))
        {
            m_LavaAnticipator.Play();
            mFirePlaying = true;
        }
        else if ((mFirePlaying == true) && (mFireAnimActivated == false) && (mCurrentTime >= (m_TimeBetweenFires + m_AnticipatorDuration)))
        {
            mFireAnimActivated = true;
            m_LavaParticles.Play();
        }
        else if ((mFirePlaying == true) && (mFireAnimActivated == true) && (mCurrentTime >= (m_TimeBetweenFires + m_AnticipatorDuration + m_FlameDuration)))
        {
            mCurrentTime = 0;
            mFirePlaying = false;
            mFireAnimActivated = false;
            mFireExternallyActivated = false;
        }
    }

    public bool GetFirePlaying()
    {
        return mFireAnimActivated;
    }

    public void PlayHazard()
    {
        m_LavaParticles.Stop();
        m_LavaAnticipator.Stop();
        mFirePlaying = false;
        mFireAnimActivated = false;
        mFireExternallyActivated = true;
        m_TimeBetweenFires = 0;
        mCurrentTime = 0;
    }

    public void SetExternallyControlled()
    {
        m_ExternallyControlled = true;
    }
}
