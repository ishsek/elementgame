using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardChoreographer : MonoBehaviour
{
    [System.Serializable]
    public struct HazardEvent
    {
        [SerializeField] public List<LavaJetHazardController> HazardRef;
        [SerializeField] public HazardGroup HazardGroup;
        [SerializeField] public bool Randomize;
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
                foreach (LavaJetHazardController hazard in fireEvent.HazardRef)
                {
                    hazard.SetExternallyControlled();
                }
            }

            if (fireEvent.HazardGroup != null)
            {
                foreach (LavaJetHazardController hazard in fireEvent.HazardGroup.GetHazardGroup())
                {
                    hazard.SetExternallyControlled();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        mCurrentSequenceTime += Time.deltaTime;
        
        while (HazardsList[mCurrentHazardsListIndex].PlayTiming <= mCurrentSequenceTime)
        {
            foreach (LavaJetHazardController hazard in HazardsList[mCurrentHazardsListIndex].HazardRef)
            {
                if (HazardsList[mCurrentHazardsListIndex].Randomize == true)
                {
                    if (Random.Range(0, 10) > 7) 
                    {
                        hazard?.PlayHazard();
                    }
                }
                else
                {
                    hazard?.PlayHazard();
                }
            }

            if (HazardsList[mCurrentHazardsListIndex].HazardGroup != null)
            {
                foreach (LavaJetHazardController hazard in HazardsList[mCurrentHazardsListIndex].HazardGroup.GetHazardGroup())
                {
                    if (HazardsList[mCurrentHazardsListIndex].Randomize == true)
                    {
                        if (Random.Range(0, 10) > 7)
                        {
                            hazard?.PlayHazard();
                        }
                    }
                    else
                    {
                        hazard?.PlayHazard();
                    }
                }
            }

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
