using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonListController : MonoBehaviour
{
    public SkillButtonController[] m_SkillButtons;
    public SkillButtonVersionData ShadowData;
    public SkillButtonVersionData FireData;

    [System.Serializable]
    public class SkillButtonVersionData
    {
        public Sprite m_Skill1Button;
        public Sprite m_Skill2Button;
        public Sprite m_Skill3Button;
        public Sprite m_Skill4Button;
    } 

    [System.Serializable]
    public enum SkillButtonVersion
    {
        Shadow,
        FIre,
    }

    public void SetState(SkillButtonVersion newButtonsVersion)
    {
        switch (newButtonsVersion)
        {
            case SkillButtonVersion.Shadow:
                break;
            case SkillButtonVersion.FIre:
                break;
            default:
                break;
        }
    }
}
