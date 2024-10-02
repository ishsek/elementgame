using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardGroup : MonoBehaviour
{
    [SerializeField] private List<LavaJetHazardController> m_HazardsInGroup;

    public List<LavaJetHazardController> GetHazardGroup()
    {
        return m_HazardsInGroup;
    }
}
