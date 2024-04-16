using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaJetHazardController : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_LavaParticles;
    [SerializeField] private ParticleSystem m_LavaAnticipator;
    [SerializeField] private float m_TimeBetweenFires = 5f;
    [SerializeField] private float m_FlameDuration = 1;

    private float mCurrentTime = 0;
    private bool mFirePlaying = false;

    private void Start()
    {
        m_LavaAnticipator.Play();
    }

    // Update is called once per frame
    void Update()
    {
        mCurrentTime += Time.deltaTime;

        if ((mCurrentTime >= m_TimeBetweenFires) && (mFirePlaying == false))
        {
            m_LavaParticles.Play();
            mFirePlaying = true;
        }
        else if ((mCurrentTime >= m_TimeBetweenFires + m_FlameDuration) && (mFirePlaying = true))
        {
            m_LavaAnticipator.Play();
            mFirePlaying = false;
            mCurrentTime = 0;
        }
    }
}
