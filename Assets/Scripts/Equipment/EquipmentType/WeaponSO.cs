using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/Equipment/WeaponSO")]
public class WeaponSO : EquipmentSO
{
    public WeaponType w_type;
    public int damage;
    public TypeDmg typedmg;
}
