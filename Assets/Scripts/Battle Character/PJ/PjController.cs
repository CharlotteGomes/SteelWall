using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;
public class PjController : BattleChar
{
    [SerializeField] WeaponSO test;

    [SerializeField] WeaponSO weapon;
    [SerializeField] ArmorSO helmet, chestplate, bracelet, gloves, legs, greaves;

    [SerializeField] protected List<SkillData> skills_unlocked = new List<SkillData>();
    [SerializeField] protected List<SkillData> skill_equipped = new List<SkillData>();
    [SerializeField] protected List<SkillData> weapon_skills = new List<SkillData>();

    [SerializeField] int option_index = 0;
    [SerializeField] int skill_index = 0;
    private void Awake()
    {
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Equip(test);
        }
        if (Input.GetKeyDown(KeyCode.K)) { start_turn?.Invoke(this); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) Unequip(weapon);
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
    public override IEnumerator StartSkill()
    {
        if(skill_current.cooldown != 0) { yield break; }
        yield return base.StartSkill();
    }

    public override IEnumerator OnGoingSkill()
    {
        /*
        float time = Time.time;
        float qte_start = skill_current.qte_start + time;
        float qte_end = skill_current.qte_end + time;
        bool crit = false;
        while (Time.time < skill_current.clip.length + time)
        {
            InputManager.GetInputs();
            if (Time.time > qte_start && crit == false)
            {
                if (InputManager.finisher) { ExecuteFinisher(); Debug.Log("Critou"); crit = true; }
            }
            yield return null;
         }*/
        yield return StartCoroutine(FinishingSkill());
    }

    void ExecuteFinisher()
    {
        /*if(skill_current == null) { return;}
        foreach (SkillEffectSO f in skill_current.finisher)
        {
            f.Execute(ctx);
        }*/
    }

    protected void ReduceCooldown()
    {
        foreach (SkillData dt in skill_equipped)
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
        if (skill_equipped.Contains(data)) return;
        data.cooldown = 0;
        skill_equipped.Add(data);
    }
    public void UnequipSkill(SkillData data)
    {
        if (!skill_equipped.Contains(data)) return;
        skill_equipped.Remove(data);
    }

    #endregion
}
