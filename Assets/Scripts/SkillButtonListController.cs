using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SkillButtonController;

public class SkillButtonListController : MonoBehaviour
{
    [System.Serializable]
    public class SkillButtonVersionData
    {
        public Sprite[] m_SkillButtonSprites;
    } 

    [System.Serializable]
    public enum SkillButtonVersion
    {
        Shadow,
        Fire,
    }

    public enum SkillButtonsIndeces
    {
        Skill1Button = 0,
        Skill2Button = 1,
        Skill3Button = 2,
        Skill4Button = 3,
    }

    public SkillButtonController[] m_SkillButtons;
    public SkillButtonVersionData ShadowData;
    public SkillButtonVersionData FireData;

    private SkillButtonVersion mCurrentElement = SkillButtonVersion.Shadow;

    public void SetState(SkillButtonVersion newButtonsVersion)
    {
        SkillButtonVersionData newVersionData = ShadowData;

        switch (newButtonsVersion)
        {
            case SkillButtonVersion.Shadow:
                newVersionData = ShadowData;
                break;
            case SkillButtonVersion.Fire:
                newVersionData = FireData;
                break;
            default:
                break;
        }

        for (int i = 0; i < m_SkillButtons.Length; i++)
        {
            m_SkillButtons[i].ChangeButtonVersion(newVersionData.m_SkillButtonSprites[i]);
        }

        mCurrentElement = newButtonsVersion;
    }

    public void ChangeButtonState(SkillButtonsIndeces skillButton, SkillButtonStates newButtonState, float newCooldownTime = 0, float chargePercentage = 0)
    {
        m_SkillButtons[(int)skillButton].ChangeButtonState(newButtonState, newCooldownTime, chargePercentage);
    }

    public void ChangeButtonElements(SkillButtonVersion newElement)
    {
        SetState(newElement);
    }
}
