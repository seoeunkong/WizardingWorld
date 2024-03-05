using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptable Object/Zombie Data", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    private string _weaponName;
    public string WeaponName {  get { return _weaponName; } }

    [SerializeField]
    private float _attackDamage;
    public float attackDamage { get { return _attackDamage;} }

    [SerializeField]
    private float _attackRange;
    public float attackRange { get { return _attackRange;} }
}
