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
    private BossMonsterController _bossMonsterController;

    void Start()
    {
        _bossMonsterController = GetComponent<BossMonsterController>();

        // 초기 쿨타임 설정
        SetCoolTime(AttackName.MISSILE, 20f);
        SetCoolTime(AttackName.LASER, 12f);
        SetCoolTime(AttackName.CHASING, 5f);
        
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

    public void TryAttack(AttackName attack)
    {
        if (_isAvailable[attack] && _bossMonsterController.bossMonster.currentStateIdle)
        {
            _bossMonsterController.WhoToAttack();

            switch (attack)
            {
                case AttackName.MISSILE:
                    _bossMonsterController.bossMonster.stateMachine.ChangeState(StateName.BMMISSILE);
                    break;
                case AttackName.LASER:
                    _bossMonsterController.bossMonster.stateMachine.ChangeState(StateName.BMLASER); 
                    break;
                case AttackName.CHASING:
                    _bossMonsterController.bossMonster.stateMachine.ChangeState(StateName.BMCHASE); break;
            }
        }
    }
}
