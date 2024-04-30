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

        // 초기 쿨타임 설정
        SetCoolTime(AttackName.MISSILE, 5f);
        SetCoolTime(AttackName.LASER, 5f);
    }

    // 쿨타임과 사용 가능 상태 초기화
    void SetCoolTime(AttackName attack, float time)
    {
        _coolTimes[attack] = time;
        _isAvailable[attack] = true;
    }

    // 쿨타임 코루틴
    public IEnumerator AttackCoolTime(AttackName attack)
    {
        if (_isAvailable[attack])
        {
            _isAvailable[attack] = false; // 공격 불가능 상태로 설정

            float timer = _coolTimes[attack];
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isAvailable[attack] = true; // 공격 가능 상태로 복귀
        }
    }

    // 공격 메서드 예시
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
