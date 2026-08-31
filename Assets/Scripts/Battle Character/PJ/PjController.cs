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

    [SerializeField] WeaponSO weapon;
    [SerializeField] ArmorSO helmet, chestplate, bracelet, gloves, legs, greaves;

    [SerializeField] protected List<SkillData> skills_unlocked = new List<SkillData>();
    [SerializeField] protected List<SkillData> skills_equipped = new List<SkillData>();
    [SerializeField] protected List<SkillData> weapon_skills = new List<SkillData>();
    public IReadOnlyList<SkillData> read_skills => skills_equipped;
    public IReadOnlyList<SkillData> read_weapon_skills => weapon_skills;

    [SerializeField] BattleOptions battle_options;
    [SerializeField] int option_index = 0;
    [SerializeField] int skill_index = 0;
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
        InitWeaponSkills();
        skills_equipped = skills_unlocked;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.K)) { start_turn?.Invoke(this); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) Unequip(weapon);
        if (Input.GetKeyDown(KeyCode.H)) { StartCoroutine(OnGoingTurn()); }
    }

    #region Init
    public override void InitStats()
    {
        base.InitStats();
    }

    public void InitWeaponSkills()
    {
        if (weapon == null) return;
        foreach(SkillSO so in weapon.w_skills.weapon_skills)
        {
            SkillData data = new SkillData(so);
            weapon_skills.Add(data);
        }
    }
    #endregion

    #region Clear
    public void ClearWeaponSkill()
    {
        if(weapon == null) return;
        weapon_skills.Clear();
    }
    #endregion

    #region Equipment 
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

    #endregion

    #region Skill Functions
    protected void ReduceCooldown()
    {
        foreach (SkillData dt in skills_equipped)
        {
            dt.cooldown = Mathf.Clamp(dt.cooldown - 1, 0, 99);
        }
    }

    public void AddSkill(SkillSO skill)
    {
        SkillData data = new SkillData(skill);

        skills_unlocked.Add(data);
    }

    public void EquipSkill(SkillData data)
    {
        if (skills_equipped.Contains(data)) return;
        data.cooldown = 0;
        skills_equipped.Add(data);
    }
    public void UnequipSkill(SkillData data)
    {
        if (!skills_equipped.Contains(data)) return;
        skills_equipped.Remove(data);
    }

    #endregion

    #region Turn

    public override IEnumerator OnGoingTurn()
    {
        start_turn?.Invoke(this);
        step = TurnStep.ChooseAction;
        option_index = 0;
        battle_options = 0;
        while(step != TurnStep.Finish)
        {
            switch(step)
            {
                case TurnStep.ChooseAction:
                    ChoosingAction();
                    break;

                case TurnStep.ChooseSkill:
                    ChoosingSkill();
                    break;

            }
            yield return null;
        }
    }

    void ChoosingAction()
    {
        InputManager.GetInputs();
        option_index = Mathf.Clamp(option_index + InputManager.hud_horizontal, 0, Enum.GetValues(typeof(BattleOptions)).Length - 1);
        battle_options = (BattleOptions)option_index;
        if (InputManager.interact) { SelectOption(battle_options); }
    }

    void ChoosingSkill()
    {
        InputManager.GetInputs();
        switch (battle_options)
        {
            case BattleOptions.Attack:
                skill_index = Mathf.Clamp(skill_index + InputManager.hud_horizontal, 0, weapon_skills.Count - 1);
                skill_current = weapon_skills[skill_index]; break;

            case BattleOptions.Skill:
                skill_index = Mathf.Clamp(skill_index + InputManager.hud_horizontal, 0, skills_equipped.Count - 1);
                skill_current = skills_equipped[skill_index]; break;
        }
        if (InputManager.interact && skill_current != null) { step = TurnStep.ChooseTarget; }
        if (InputManager.esc) { skill_index = 0; step = TurnStep.ChooseAction; }
    }

    private void SelectOption(BattleOptions bp)
    {
        step = TurnStep.ChooseSkill;
    }
    #endregion
}
