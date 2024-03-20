using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public WeaponData handleData { get { return _weaponData;  } }
    public RuntimeAnimatorController WeaponAnimator { get { return _weaponAnimator;  } }
    public float AttackDamage { get { return _attackDamage; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public float AttackRange { get { return _attackRange; } }

    #region #���� ����
    [Header("���� ����"), Tooltip("�ش� ���⸦ ����� ���� local Transform �� ����")]
    [SerializeField] protected WeaponData _weaponData;

    [Header("���� ����")]
    [SerializeField] protected RuntimeAnimatorController _weaponAnimator;
    [SerializeField] protected float _attackDamage;
    [SerializeField] protected float _attackSpeed;
    [SerializeField] protected float _attackRange;
    #endregion

    public abstract void Attack(BaseState state);

}
