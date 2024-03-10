using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public bool HasWeapon { get { return _hasWeapon; } }
    public string Name { get { return _name; } }
    public float AttackDamage { get { return _attackDamage; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public float AttackRange { get { return _attackRange; } }


    [Header("공격 정보")]
    [SerializeField] protected bool _hasWeapon;
    [SerializeField] protected string _name;
    [SerializeField] protected float _attackDamage;
    [SerializeField] protected float _attackSpeed;
    [SerializeField] protected float _attackRange;

    public abstract void Attack(BaseState state);

}
