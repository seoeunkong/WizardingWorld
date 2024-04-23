using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_", menuName = "Scriptable Object/Inventory System/Monster", order = 4)]

public class MonsterData : CountableData
{
    public float MaxHP { get { return _maxHP; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float DashSpeed { get { return _dashSpeed; } }
    public float AttackPower { get { return _attackPower; } }
    public float RotateSpeed { get { return _rotateSpeed; } }
    public int Level { get { return _level; } }

    [Header("∏ÛΩ∫≈Õ Ω∫≈»")]
    [SerializeField] protected float _maxHP;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _dashSpeed;
    [SerializeField] protected float _attackPower;
    [SerializeField] protected float _rotateSpeed;
    [SerializeField] protected int _level;
}
