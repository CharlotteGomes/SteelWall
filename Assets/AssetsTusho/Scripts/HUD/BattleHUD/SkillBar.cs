using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class SkillBar : MonoBehaviour
{
    const int VISIBLE_SLOTS = 3;
    [SerializeField] GameObject slotprefab;
    [SerializeField] List<Transform> Anchors = new List<Transform>();
    [SerializeField] List<SkillSlot> slots = new List<SkillSlot>();
    [SerializeField] List<SkillBase> skills = new List<SkillBase>();
    [SerializeField] PJController currentCharacter;
    [SerializeField] RectTransform canvasRect;
    RectTransform rectTransform;

    public void SettingCharacter(PJController character)
    {
        currentCharacter = character;
        canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        Vector3 worldPos = currentCharacter.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            Camera.main,
            out Vector2 uiPos
        );
        rectTransform.anchoredPosition = uiPos;
    }

    public void GetSkills()
    {
        skills = currentCharacter.SkillList;
    }

    public void ClearSkills()
    {
        skills.Clear();
    }

    public void SpawnSlots()
    {
        for (int i = 0; i < VISIBLE_SLOTS; i++)
        {
            GameObject slotobj = Instantiate(slotprefab, Anchors[i]);
            SkillSlot slotscr = slotobj.GetComponent<SkillSlot>();
            slotscr.ReceiveSkill(skills[i]);
            slots.Add(slotscr);
        }
    }

    public void DestroySlots()
    {
        for(int i = slots.Count - 1; i >= 0; i--)
        {
            Destroy(slots[i].gameObject);
        }
        slots.Clear();
    }

    public void RefreshSlots()
    {
        int idx = currentCharacter.skillIdx;

        SetupSlot(slots[0], idx - 1); ; // LEFT
        SetupSlot(slots[1], idx);     // CENTER
        SetupSlot(slots[2], idx + 1); // RIGHT
    }

    public void MoveAndResize(int dir)
    {
        
        if (dir == 1) 
        {
            SkillSlot last = slots[2];
            slots.RemoveAt(2);
            slots.Insert(0, last);
        }
        if(dir == -1)
        {
            
            SkillSlot first = slots[0];
            slots.Remove(slots[0]);
            slots.Add(first);
        }
        slots[0].SetAnchor(Anchors[0], 0.7f);
        slots[1].SetAnchor(Anchors[1], 1f);
        slots[2].SetAnchor(Anchors[2], 0.7f);
        RefreshSlots();
    }

    void SetupSlot(SkillSlot slot, int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Count)
        {
            slot.gameObject.SetActive(false);
            return;
        }

        slot.gameObject.SetActive(true);
        slot.ReceiveSkill(skills[skillIndex]);
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    
    void Update()
    {          
    }
}
