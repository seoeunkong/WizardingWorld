using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static MonsterController;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.SceneView;

public class BossMonsterController : CharacterController, IStateChangeable
{
    public BossMonster bossMonster { get; private set; }

    #region #공격 
    public Transform attackTarget { get; private set; }
    [Header("공격 확률")]
    [SerializeField][Range(0, 10)] private int _playerWeight;
    [SerializeField][Range(0, 10)] private int _palWeight;

    [Header("원거리 미사일 공격")]
    [SerializeField] private GameObject _missile;
    [SerializeField] private int _missileCount;
    [SerializeField] private float _missileAngle;
    [SerializeField] private float _missileSpeed;
    public Vector3[] missiles;
    public Transform missileStartPos;
    public GameObject[] _missileObjects { get; private set; }

    [Header("원거리 레이저 공격")]
    public GameObject laserObject;
    [SerializeField] private float _laserRange;
    [SerializeField] private float _lineTimer;
    [SerializeField] private float _laserSpeed;
    public LineRenderer laserLine { get; private set; }
    public float lineTimer { get { return _lineTimer; } }
    public float laserSpeed { get { return _laserSpeed; } }
    public float laserRange { get { return _laserRange; } }

    public int missileCount { get { return _missileCount; } }

    [Header("근접 공격")]
    public GameObject effectObject;
    [SerializeField] private float _fieldOfView;
    [SerializeField] private float _checkDistance;
    public void SetOffEffect() => effectObject.SetActive(false);
    public void SetOnEffect() => effectObject.SetActive(true);
    #endregion

    #region #쿨타임 시스템
    private BossMonsterAttackManager _attackManager;
    public const float missileCoolTime = 5f;
    public const float laserCoolTime = 7f;
    public void StartCoolTime(AttackName attackName) => StartCoroutine(_attackManager.AttackCoolTime(attackName));
    #endregion

    #region #추격 
    public const float chasingTime = 10f;
    public const float stopChasingDist = 15f;
    #endregion

    #region #Hp
    public Slider _bossSliderBar; //임시
    void UpdateBossHp(float hp) => _bossSliderBar.value = hp / bossMonster.maxHP;
    #endregion

    public bool Dead { get; private set; }

    void Start()
    {
        bossMonster = GetComponent<BossMonster>();
        _groundLayer = 1 << LayerMask.NameToLayer("Ground");
        bossMonster.OnHealthChanged += UpdateBossHp;


        Init();
    }

    void Update()
    {
        if(attackTarget != null)
        {
            Vector3 dir = Vector3.Scale((attackTarget.position - transform.position), new Vector3(1, 0, 1)).normalized;
            //transform.rotation = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 50f);
        }
    }

    public void ChangeState(StateName state)
    {
        bossMonster.stateMachine.ChangeState(state);
    }

    public IEnumerator ThinkAction()
    {
        yield return new WaitForSeconds(1f);

        int action = UnityEngine.Random.Range(4, 7);
        switch (action)
        {
            case 0:
            case 1:
                _attackManager.TryAttack(AttackName.MISSILE); break;
            case 2:
            case 3:
                _attackManager.TryAttack(AttackName.LASER); break;
            case 4:
            case 5:
            case 6:
                _attackManager.TryAttack(AttackName.CHASING); break;
        }
    }

    private void Init()
    {
        _missileObjects = new GameObject[_missileCount];
        for (int i = 0; i < _missileCount; i++)
        {
            _missileObjects[i] = Instantiate(_missile);
            _missileObjects[i].transform.parent = transform;
        }
        _missile.SetActive(false);

        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = false;
        laserObject.SetActive(false);
        effectObject.SetActive(false);

        _attackManager = GetComponent<BossMonsterAttackManager>();
    }



    public Vector3 CalcRunDir(Transform target)
    {
        if (target == null) return Vector3.zero; //플레이어를 감지 영역에서 발견하지 못한 경우 

        Vector3 targetPos = target.position;

        Vector3 dir = Vector3.Scale((targetPos - transform.position), new Vector3(1, 0, 1)).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        return GetDirection(dir);
    }


    Vector3 GetDirection(Vector3 dir)
    {
        _isOnSlope = IsOnSlope();
        _isGrounded = IsGrounded();

        Vector3 calculatedDirection = (_isOnSlope && _isGrounded) ? AdjustDirectionToSlope(dir) : dir;
        return calculatedDirection;
    }

    Monster GetPal()
    {
        if (Player.Instance.PlayerHasPal) return Player.Instance.currentPal;
        else return null;
    }

    CharacterController CalculateWeight() //공격 대상 확률 계산
    {
        CharacterController[] characterControllers = new CharacterController[_playerWeight + _palWeight];
        for (int i = 0; i < _playerWeight; i++)
        {
            characterControllers[i] = Player.Instance.GetComponent<PlayerController>();
        }

        Monster pal = GetPal();
        for (int i = _playerWeight; i < characterControllers.Length; i++)
        {
            characterControllers[i] = pal.gameObject.GetComponent<MonsterController>();
        }

        int index = UnityEngine.Random.Range(0, characterControllers.Length);
        return characterControllers[index];
    }

    public void WhoToAttack() //공격 대상 지정
    {
        Monster pal = GetPal();
        if (pal == null) attackTarget = Player.Instance.transform;
        else
        {
            if (pal.dangerState)
            {
                attackTarget =  pal.transform;
            }
            else
            {
                CharacterController controller = CalculateWeight();
                attackTarget = controller.transform;
            }
        }
    }

    public Vector3 GetDirToTarget(Vector3 target)
    {
        if (target == null) return Vector3.zero;
        return (target - transform.position).normalized;
    }

    public Vector3[] GetMissileDirs(int cnt)
    {
        Vector3 direction = ((attackTarget.position + Vector3.up) - missileStartPos.transform.position).normalized;
        Vector3[] dirs = new Vector3[cnt];

        float term = _missileAngle / (cnt - 1);
        float halfAngle = _missileAngle / 2;
        for (int i = 0; i < cnt; i++)
        {
            float angle = -halfAngle + term * i;

            // 로컬 좌표계를 고려한 회전 적용
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            dirs[i] = (rotation * direction);
        }

        return dirs;
    }

    public void MissileAttack(Vector3[] dirs)
    {
        for (int i = 0; i < dirs.Length; i++)
        {
            _missileObjects[i].transform.Translate(_missileSpeed * Time.deltaTime * dirs[i], Space.World);
        }
    }

    public bool allMissileOff()
    {
        foreach (var obj in _missileObjects)
        {
            if (obj.activeSelf) return false;
        }
        return true;
    }

    public void resetMissile()
    {
        foreach (var obj in _missileObjects)
        {
            obj.SetActive(true);
            obj.transform.position = missileStartPos.position;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(this.transform.position, this.transform.localRotation * right45 * 50.0f);
        //Gizmos.DrawRay(this.transform.position, this.transform.localRotation * left45 * 50.0f);

        //foreach (Vector3 ang in GetMissileDirs(10))
        //{
        //    Gizmos.DrawRay(missileStartPos.position, this.transform.localRotation * ang * 50.0f);
        //}
        //int cnt = 0;
        //foreach (Vector3 ang in GetMissileDirs(10))
        //{
        //    Gizmos.DrawRay(missileStartPos.position, ang * 50f);
        //    cnt++;
        //}
    }

    bool IsTargetInSight(Vector3 targetPos)    // 계산된 각도를 통해 플레이어가 시야각 범위 내에 있는지 확인
    {
        Vector3 enemyToPlayer = targetPos - transform.position;
        enemyToPlayer.Normalize();

        float angle = Vector3.Angle(transform.forward, enemyToPlayer);

        if (angle < _fieldOfView * 0.5f) return true;
        else return false;
    }

    public Transform CheckTarget(float size = 1) //감지 영역 내에 플레이어 존재 여부 체크 
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _checkDistance * size);
        foreach (Collider coll in colls)
        {
            if (coll.gameObject.CompareTag("Player") ||
                coll.gameObject.CompareTag("Enemy") && (Player.Instance.PlayerHasPal && coll.transform.gameObject == Player.Instance.currentPal.gameObject))
            {
                if (IsTargetInSight(coll.transform.position)) return coll.transform;
            }
        }
        return null;
    }

    public override void Attacking(CharacterController characterController)
    {
        characterController.Hit(bossMonster.attackPower);
        if (characterController is MonsterController mon) mon.monsterInfo.stateMachine.ChangeState(StateName.MHIT);
    }

    public override void Hit(float damage)
    {
        if (bossMonster.CurrentHP > 0)
        {
            float hp = bossMonster.CurrentHP - damage > 0 ? bossMonster.CurrentHP - damage : 0;
            bossMonster.SetHP(hp);

            if (hp == 0)
            {
                Dead = true;
                bossMonster.stateMachine.ChangeState(StateName.BMDEAD);
            }
        }
    }
}
