using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance = null;
    public WeaponManager weaponManager {  get; private set; }
    public StateMachine<PlayerController> stateMachine { get; private set; }
    public Rigidbody rigid { get; private set; }
    public Animator animator {  get; private set; }

    #region #캐릭터 스탯
    private float _currentAttackPower;
    private float _currentSpeed;
    public float MaxHP { get { return _maxHP; } }
    public float CurrentHP { get { return _currentHP; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float DashSpeed { get { return _dashSpeed; } }
    public float AttackPower { get { return _attackPower; } }
    public float ThrowPower { get { return _throwPower; } }
    public float CurrentAttackPower { get { return _currentAttackPower; } }
    public float CurrentSpeed { get { return _currentSpeed; } set { _currentSpeed = value; } }


    [Header("캐릭터 스탯")]
    [SerializeField] protected float _maxHP;
    [SerializeField] protected float _currentHP;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _dashSpeed;
    [SerializeField] protected float _attackPower;
    [SerializeField] protected float _throwPower;
    #endregion

    public delegate void HealthChanged(float newHealth);
    public event HealthChanged OnHealthChanged;

    //현재 팰 정보 
    private Monster _pal;
    public Monster currentPal { get { return _pal; } set { _pal = value; } }
    public bool PlayerHasPal => _pal != null;
    private bool _unSetPal = false;
    public bool UnSetPal {  get { return _unSetPal; } set { _unSetPal = value; } }


    [SerializeField]
    public Transform rightHand;
    public Transform NotUsingWeapons;

    public void SetHPValue(float hp)
    {
        _currentHP = hp;
        OnHealthChanged?.Invoke(_currentHP); 

    }
    public void SetAttackPower(float attackPower) { _currentAttackPower = attackPower; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            weaponManager = new WeaponManager(rightHand);
            weaponManager.unRegisterWeapon = (weapon) => { Destroy(weapon); };
            rigid = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

    }

    void Start()
    {
        InitStateMachine();
        SetHPValue(MaxHP);
    }

    void Update()
    {
        stateMachine?.UpdateState();
    }

    void FixedUpdate()
    {
        stateMachine?.FixedUpdateState();
    }


    void InitStateMachine()
    {
        PlayerController controller = GetComponent<PlayerController>();
        stateMachine = new StateMachine<PlayerController>(StateName.MOVE,new MoveState(controller));
        stateMachine.AddState(StateName.PUNCHATTACK, new PunchAttackState(controller));
        stateMachine.AddState(StateName.ATTACK, new AttackState(controller));
        stateMachine.AddState(StateName.THROW, new ThrowState(controller));
    }

   
    public void OnUpdateState(float maxHP, float currentHP, float moveSpeed, float dashSpeed)
    {
        this._maxHP = maxHP;
        this._currentHP = currentHP;
        this._moveSpeed = moveSpeed;
        this._dashSpeed = dashSpeed;
    }

    public PalSphere hasSphere()
    {
        //손에 무기가 있는지 체크 
        foreach (Transform child in rightHand)
        {
            if (child.gameObject.activeSelf)
            {
                BaseObject bo = child.GetComponent<BaseObject>();
                if (bo is PalSphere ps) return ps;
            }
        }
        return null;
    }

    public void UnSetSphere()
    {
        if(!hasSphere()) return;

        foreach (Transform child in rightHand)
        {
            if (child.gameObject.activeSelf) child.gameObject.SetActive(false);
        }
    }

}
