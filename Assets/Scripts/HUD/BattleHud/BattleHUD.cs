using System.Collections.Generic;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] List<SkillSlot> slots;
    [SerializeField] SkillSlot selected;
    public PjController current;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrent(PjController pj)
    {
        current = pj;
    }

    public void SetSelectedSlot(SkillSlot slot)
    {
        selected = slot;
    }

    public void UpdateSkills()
    {
        for(int i = 0; i < 4; i++)
        {
            slots[i].skill = current.read_skills[i];
            slots[i].UpdateImage();
        }
    }
}
