using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardChoreographer : MonoBehaviour
{
    [System.Serializable]
    public struct HazardEvent
    {
        [SerializeField] public LavaJetHazardController HazardRef;
        [SerializeField] public float PlayTiming;
    }

    [SerializeField] private List<HazardEvent> HazardsList;

    private float mCurrentSequenceTime;
    private int mCurrentHazardsListIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (HazardEvent fireEvent in HazardsList)
        {
            if (fireEvent.HazardRef != null)
            {
                fireEvent.HazardRef.SetExternallyControlled();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        mCurrentSequenceTime += Time.deltaTime;
        
        while (HazardsList[mCurrentHazardsListIndex].PlayTiming <= mCurrentSequenceTime)
        {
            HazardsList[mCurrentHazardsListIndex].HazardRef?.PlayHazard();
            mCurrentHazardsListIndex++;

            if (mCurrentHazardsListIndex >= HazardsList.Count)
            {
                mCurrentHazardsListIndex = 0;
                mCurrentSequenceTime = 0;
                break;
            }
        }
    }
}
