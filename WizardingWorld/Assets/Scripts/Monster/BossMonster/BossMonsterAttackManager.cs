using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackName
{
    MISSILE,
    LASER
}

public class BossMonsterAttackManager : MonoBehaviour
{
    private Dictionary<AttackName, float> _coolTimes = new Dictionary<AttackName, float>();
    private Dictionary<AttackName, bool> _isAvailable = new Dictionary<AttackName, bool>();
    private BossMonster _bossMonster;

    void Start()
    {
        _bossMonster = GetComponent<BossMonster>();

        // �ʱ� ��Ÿ�� ����
        SetCoolTime(AttackName.MISSILE, 5f);
        SetCoolTime(AttackName.LASER, 5f);
    }

    // ��Ÿ�Ӱ� ��� ���� ���� �ʱ�ȭ
    void SetCoolTime(AttackName attack, float time)
    {
        _coolTimes[attack] = time;
        _isAvailable[attack] = true;
    }

    // ��Ÿ�� �ڷ�ƾ
    public IEnumerator AttackCoolTime(AttackName attack)
    {
        if (_isAvailable[attack])
        {
            _isAvailable[attack] = false; // ���� �Ұ��� ���·� ����

            float timer = _coolTimes[attack];
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isAvailable[attack] = true; // ���� ���� ���·� ����
        }
    }

    // ���� �޼��� ����
    public void TryAttack(AttackName attack)
    {
        if (_isAvailable[attack])
        {
            Debug.Log("Mode " + attack);
            switch (attack)
            {
                case AttackName.MISSILE:
                    _bossMonster.stateMachine.ChangeState(StateName.BMMISSILE);
                    break;
                case AttackName.LASER:
                    _bossMonster.stateMachine.ChangeState(StateName.BMLASER); 
                    break;
            }
            StartCoroutine(AttackCoolTime(attack));
        }
        else
        {
            //Debug.Log(attack + " is cooling down.");
        }
    }
}
