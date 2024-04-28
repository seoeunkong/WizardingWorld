using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.SceneView;

public class BossMonsterController : CharacterController
{
    public BossMonster bossMonster { get; private set; }

    #region #플레이어의 팰 정보 
    public bool playerHasPal => Player.Instance.currentPal != null && Player.Instance.currentPal.gameObject.activeSelf;
    #endregion

    #region #공격 
    public Transform attackeTarget { get; private set; }
    [Header("공격 확률")]
    [SerializeField][Range(0, 10)] private int _playerWeight;
    [SerializeField][Range(0, 10)] private int _palWeight;

    [Header("원거리 미사일 공격")]
    [SerializeField] private int _missileCount;
    [SerializeField] private float _missileAngle;
    [SerializeField] private GameObject _missile;
    [SerializeField] private float _missileSpeed;
    public Transform missileStartPos;
    public GameObject[] _missileObjects { get; private set; }

    public int missileCount { get { return _missileCount; } }
    #endregion

    #region #쿨타임
    public const float missileCoolTime = 5f;
    private bool _availableMissile = true;
    #endregion


    void Start()
    {
        bossMonster = GetComponent<BossMonster>();
        Init();
    }

    void Update()
    {
        if (bossMonster.stateMachine.CurrentState == bossMonster.stateMachine.GetState(StateName.BMIDLE))
        {
            StartCoroutine(ThinkAction());
        }
    }

    IEnumerator ThinkAction()
    {
        yield return new WaitForSeconds(0.1f);

        int action = Random.Range(0, 2);
        switch (action)
        {
            case 0:
            case 1:
                if (!_availableMissile) break;
                bossMonster.stateMachine.ChangeState(StateName.BMMISSILE);
                StartCoroutine(MissileAttackCoolTime());
                break;
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
    }

    Monster GetPal()
    {
        if (playerHasPal) return Player.Instance.currentPal;
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

        int index = Random.Range(0, characterControllers.Length);
        return characterControllers[index];
    }

    Transform WhoToAttack() //공격 대상 지정
    {
        Monster pal = GetPal();
        if (pal == null) return Player.Instance.transform;
        else
        {
            if (pal.dangerState)
            {
                return pal.transform;
            }
            else
            {
                CharacterController controller = CalculateWeight();
                return controller.transform;
            }
        }
    }

    Vector3 GetDirToTarget(Vector3 target)
    {
        if (target == null) return Vector3.zero;
        return (target - transform.position).normalized;
    }

    public Vector3[] GetMissileDirs(int cnt)
    {
        Vector3 forward = missileStartPos.forward;
        Vector3[] dirs = new Vector3[cnt];

        float term = _missileAngle / cnt;
        float angle = _missileAngle / 2;
        for (int i = 0; i < cnt; i++)
        {
            angle -= term;
            if (angle < 0) angle += 360;

            Quaternion rot = Quaternion.Euler(0, angle, 0);
            dirs[i] = (rot * forward);
        }

        return dirs;
    }

    public void MissileAttack(Vector3[] dirs)
    {
        for (int i = 0; i < dirs.Length; i++)
        {
            _missileObjects[i].transform.Translate(_missileSpeed * Time.deltaTime * dirs[i]);
        }
       
    }

    IEnumerator MissileAttackCoolTime()
    {
        _availableMissile = false;

        float timer = missileCoolTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
        _availableMissile = true;
    }

    public bool allMissileOff()
    {
        foreach (var obj in _missileObjects)
        {
            if(obj.activeSelf) return false;
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


    }

    public override void Attacking(CharacterController characterController)
    {
        throw new System.NotImplementedException();
    }

    public override void Hit(float damage)
    {
        throw new System.NotImplementedException();
    }
}
