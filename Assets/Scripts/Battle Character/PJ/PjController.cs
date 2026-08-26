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
            Equip(weapon);
        }
        if (Input.GetKeyDown(KeyCode.K)) { start_turn?.Invoke(this); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) ReduceCooldown();
    }

    public void AddStat(Stats stats, int value)
    {
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

    #region Equipment 
    public void Equip(EquipmentSO eq)
    {
        if(eq.e_type == EquipmentType.Weapon)
        {
            var armor = eq as WeaponSO;
            weapon = armor;
            weapon.Apply(this);
        }

        if(eq.e_type == EquipmentType.Armor)
        {
            var armor = eq as ArmorSO;
            switch (armor.a_type) 
            {
                case ArmorType.Helmet:
                    helmet = armor; break;
                case ArmorType.Chestplate:
                    chestplate = armor; break;
                case ArmorType.Bracelet:
                    bracelet = armor; break;
                case ArmorType.Gloves:
                    gloves = armor; break;
                case ArmorType.Legs:
                    legs = armor; break;
                case ArmorType.Greaves:
                    greaves = armor; break;
            }
            armor.Apply(this);
        }
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
