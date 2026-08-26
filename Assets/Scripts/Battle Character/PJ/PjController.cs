using System.Collections;
using UnityEditor.Search;
using UnityEngine;
using System;
using System.Linq;
public class PjController : BattleChar
{
    [SerializeField] WeaponSO test;

    [SerializeField] WeaponSO weapon;
    [SerializeField] ArmorSO helmet, chestplate, bracelet, gloves, legs, greaves;
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

    public void AddStat(Stats stats, int value)
    {
        if (value < 0) return;
        switch (stats)
        {
            case Stats.Strength:
                strength += value; break;
            case Stats.Dexterity:
                dexterity += value; break;
            case Stats.Constitution:
                constitution += value; break;
            case Stats.Int:
                intelligence += value; break;
            case Stats.Mind:
                mind += value; break;
            case Stats.Slashing:
                r_slashing += value; break;
            case Stats.Bludgeoning:
                r_bludgeoning += value; break;
            case Stats.Pierce:
                r_pierce += value; break;
            case Stats.Fire:
                r_fire += value; break;
            case Stats.Ice:
                r_ice += value; break;
            case Stats.Lightning:
                r_lightning += value; break;
            case Stats.Purity:
                r_purity += value; break;
        }
    }
    public void RemoveStat(Stats stats, int value)
    {
        if (value < 0) return;
        switch (stats)
        {
            case Stats.Strength:
                strength -= value; break;
            case Stats.Dexterity:
                dexterity -= value; break;
            case Stats.Constitution:
                constitution -= value; break;
            case Stats.Int:
                intelligence -= value; break;
            case Stats.Mind:
                mind -= value; break;
            case Stats.Slashing:
                r_slashing -= value; break;
            case Stats.Bludgeoning:
                r_bludgeoning -= value; break;
            case Stats.Pierce:
                r_pierce -= value; break;
            case Stats.Fire:
                r_fire -= value; break;
            case Stats.Ice:
                r_ice -= value; break;
            case Stats.Lightning:
                r_lightning -= value; break;
            case Stats.Purity:
                r_purity -= value; break;
        }
    }

    #region Equipment 
    public void Equip(EquipmentSO eq)
    {
        switch (eq)
        {
            case WeaponSO weaponSO:
                weapon = weaponSO;
                weapon.Apply(this);
                break;

            case ArmorSO armorSO:
                switch (armorSO.a_type)
                {
                    case ArmorType.Helmet:
                        helmet = armorSO;break;
                    case ArmorType.Chestplate:
                        chestplate = armorSO;break;
                    case ArmorType.Bracelet:
                        bracelet = armorSO;break;
                    case ArmorType.Gloves:
                        gloves = armorSO;break; 
                    case ArmorType.Legs:
                        legs = armorSO;break;
                    case ArmorType.Greaves:
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
    #endregion
}
