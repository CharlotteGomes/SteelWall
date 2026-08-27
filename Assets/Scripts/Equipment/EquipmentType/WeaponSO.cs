using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/Equipment/Weapon/WeaponBase")]
public class WeaponSO : EquipmentSO
{
    public WeaponType w_type;
    public int damage;
    public TypeDmg typedmg;
    public WeaponSkillsSO w_skills;
}
