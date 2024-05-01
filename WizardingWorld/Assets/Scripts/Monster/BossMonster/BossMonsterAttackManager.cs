using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackName
{
    MISSILE,
    LASER,
    CHASING,
    MELEE
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
        SetCoolTime(AttackName.MISSILE, 20f);
        SetCoolTime(AttackName.LASER, 12f);
        SetCoolTime(AttackName.CHASING, 5f);
        
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
        if (_isAvailable[attack] && _bossMonster.currentStateIdle)
        {
            Debug.Log("Mode " + attack + " " + _isAvailable[attack]);

            switch (attack)
            {
                case AttackName.MISSILE:
                    _bossMonster.stateMachine.ChangeState(StateName.BMMISSILE);
                    break;
                case AttackName.LASER:
                    _bossMonster.stateMachine.ChangeState(StateName.BMLASER); 
                    break;
                case AttackName.CHASING:
                    _bossMonster.stateMachine.ChangeState(StateName.BMCHASE); break;
            }
        }
    }
}
