using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance = null;
    public WeaponManager weaponManager {  get; private set; }
    public StateMachine stateMachine { get; private set; }
    public Rigidbody rigid { get; private set; }
    public Animator animator {  get; private set; }

    #region #Ä³¸¯ÅÍ ½ºÅÈ
    public float MaxHP { get { return _maxHP; } }
    public float CurrentHP { get { return _currentHP; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float DashSpeed { get { return _dashSpeed; } }
    public float AttackPower { get { return _attackPower; } }
    public float CurrentSpeed { get { return _currentSpeed; } set { _currentSpeed = value; } }


    [Header("Ä³¸¯ÅÍ ½ºÅÈ")]
    [SerializeField] protected float _maxHP;
    [SerializeField] protected float _currentHP;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _dashSpeed;
    [SerializeField] protected float _attackPower;
    #endregion

    private float _currentSpeed;

    [SerializeField]
    public Transform rightHand;


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
        stateMachine = new StateMachine(StateName.MOVE,new MoveState(controller));
        stateMachine.AddState(StateName.PUNCHATTACK, new PunchAttackState(controller));
    }

   
    public void OnUpdateState(float maxHP, float currentHP, float moveSpeed, float dashSpeed)
    {
        this._maxHP = maxHP;
        this._currentHP = currentHP;
        this._moveSpeed = moveSpeed;
        this._dashSpeed = dashSpeed;
    }
}
