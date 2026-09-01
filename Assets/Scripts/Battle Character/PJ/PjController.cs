using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
public class PjController : BattleChar
{
    [SerializeField] List<SkillSO> test = new List<SkillSO>();

    [SerializeField] protected List<SkillSO> skills_unlocked = new List<SkillSO>();
    [SerializeField] protected List<SkillSO> skills_equipped = new List<SkillSO>();
    //[SerializeField] protected List<SkillSO> weapon_skills = new List<SkillSO>();
    public IReadOnlyList<SkillSO> read_skills => skills_equipped;
    //public IReadOnlyList<SkillSO> read_weapon_skills => weapon_skills;

    [ShowInInspector]
    public int skill_index { get; private set; } = 0;
    [ShowInInspector]
    public int intent_index { get; private set; } = 0;
    private void Awake()
    {
        life_current = 1000;
    }

    void Start()
    {
        foreach (SkillSO so in test)
        {
            AddSkill(so);
        }
        //InitWeaponSkills();
        skills_equipped = skills_unlocked;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.K)) { start_turn?.Invoke(this); }
        //if (Input.GetKeyDown(KeyCode.DownArrow)) Unequip(weapon);
        if (Input.GetKeyDown(KeyCode.H)) { StartCoroutine(OnGoingTurn()); }
    }

    #region Init
    public override void InitStats()
    {
        base.InitStats();
    }
    /*
    public void InitWeaponSkills()
    {
        if (weapon == null) return;
        foreach(SkillSO so in weapon.w_skills.weapon_skills)
        {
            SkillData data = new SkillData(so);
            weapon_skills.Add(data);
        }
    }*/
    #endregion

    #region Clear
    /*
    public void ClearWeaponSkill()
    {
        if(weapon == null) return;
        weapon_skills.Clear();
    }*/
    #endregion

    #region Equipment 
    /*
    public void Equip(EquipmentSO eq)
    {
        switch (eq)
        {
            case WeaponSO weaponSO:
                if (weapon != null) weapon.Remove(this);
                weapon = weaponSO;
                weapon.Apply(this);
                break;

            case ArmorSO armorSO:
                switch (armorSO.a_type)
                {
                    case ArmorType.Helmet:
                        if (helmet != null) helmet.Remove(this);
                        helmet = armorSO;break;
                    case ArmorType.Chestplate:
                        if (chestplate != null) chestplate.Remove(this);
                        chestplate = armorSO;break;
                    case ArmorType.Bracelet:
                        if (bracelet != null) bracelet.Remove(this);
                        bracelet = armorSO;break;
                    case ArmorType.Gloves:
                        if (gloves != null) gloves.Remove(this);
                        gloves = armorSO;break; 
                    case ArmorType.Legs:
                        if (legs != null) legs.Remove(this);
                        legs = armorSO;break;
                    case ArmorType.Greaves:
                        if (greaves != null) greaves.Remove(this);
                        greaves = armorSO;break;
                }
                armorSO.Apply(this);
                break;
        }
    }

    public bool Unequip(EquipmentSO eq)
    {
        if (eq == null) return false;
        switch (eq)
        {
            case WeaponSO weaponSO:
                if (weapon != weaponSO) return false;
                weapon.Remove(this); weapon = null;
                break;

            case ArmorSO armorSO:
                switch (armorSO.a_type)
                {
                    case ArmorType.Helmet:
                        if (helmet != armorSO) return false;
                        armorSO.Remove(this); helmet = null; break;
                    case ArmorType.Chestplate:
                        if (chestplate != armorSO) return false;
                        armorSO.Remove(this); chestplate = null; break;
                    case ArmorType.Bracelet:
                        if (bracelet != armorSO) return false;
                        armorSO.Remove(this); bracelet = null; break;
                    case ArmorType.Gloves:
                        if (gloves != armorSO) return false;
                        armorSO.Remove(this); gloves = null; break;
                    case ArmorType.Legs:
                        if (legs != armorSO) return false;
                        armorSO.Remove(this); legs = null; break;
                    case ArmorType.Greaves:
                        if (greaves != armorSO) return false;
                        armorSO.Remove(this); greaves = null; break;
                }
                break;
        }

        return true;
    }
    */
    #endregion

    #region Skill Functions
    protected void ReduceCooldown()
    {
        foreach (SkillSO dt in skills_equipped)
        {
            dt.cooldown = Mathf.Clamp(dt.cooldown - 1, 0, 99);
        }
    }

    public void AddSkill(SkillSO data)
    {
        if(skills_unlocked.Contains(data)) return;
        skills_unlocked.Add(data);
    }

    public void EquipSkill(SkillSO data)
    {
        if (skills_equipped.Contains(data)) return;
        skills_equipped.Add(data);
    }
    public void UnequipSkill(SkillSO data)
    {
        if (!skills_equipped.Contains(data)) return;
        skills_equipped.Remove(data);
    }

    #endregion

    #region Turn

    public override IEnumerator OnGoingTurn()
    {
        start_turn?.Invoke(this);
        step = TurnStep.ChooseSkill;
        intent_index = 0;
        skill_index = 0;
        skill_current = skills_equipped[skill_index];
        while (step != TurnStep.Finish)
        {
            switch(step)
            {
                case TurnStep.ChooseSkill:
                    ChoosingSkill();
                    yield return null;
                    break;

                case TurnStep.ChooseTarget:
                    ChoosingTarget();
                    yield return null;
                    break;

                case TurnStep.ExecutingSkill:
                    yield return StartCoroutine(StartSkill());
                    end_turn?.Invoke(this);
                    step = TurnStep.Finish;
                    break;
            }
            
        }
    }

    void ChoosingSkill()
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal != 0)
        {
            skill_index = Mathf.Clamp(skill_index + InputManager.hud_horizontal, 0, skills_equipped.Count - 1);
            skill_current = skills_equipped[skill_index];
        }
        if (InputManager.interact && skill_current != null) { step = TurnStep.ChooseTarget; }
    }

    void ChoosingTarget()
    {
        InputManager.GetInputs();
        if(skill_current.target == TypeTarget.Self) { ctx.Target = ctx.Self; step = TurnStep.ExecutingSkill; return; }
        if(skill_current.target == TypeTarget.Enemy || skill_current.target == TypeTarget.Party)
        {
            ctx.Target = ctx.GetListChar(skill_current.target)[skill_current.pos[0]]; step = TurnStep.ExecutingSkill; return;
        }
        if (InputManager.hud_horizontal != 0)
        {
            intent_index = Mathf.Clamp(intent_index + InputManager.hud_horizontal, 0, skill_current.pos.Length - 1);
            ctx.Target = ctx.GetListChar(skill_current.target)[skill_current.pos[intent_index]];
        }
        if (InputManager.interact && skill_current != null) { step = TurnStep.ExecutingSkill; }
    }
    #endregion
}
