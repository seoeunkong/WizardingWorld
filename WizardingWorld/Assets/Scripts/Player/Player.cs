using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance = null;
    public StateMachine stateMachine { get; private set; }
    public Rigidbody _rigid { get; private set; }
    public Animator _animator {  get; private set; }

    #region #Ä³¸¯ÅÍ ½ºÅÈ
    public float _MaxHP { get { return maxHP; } }
    public float _CurrentHP { get { return currentHP; } }
    public float _MoveSpeed { get { return moveSpeed; } }
    public float _DashSpeed { get { return dashSpeed; } }
    public float _CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }


    [Header("Ä³¸¯ÅÍ ½ºÅÈ")]
    [SerializeField] protected float maxHP;
    [SerializeField] protected float currentHP;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float dashSpeed;
    #endregion

    private float currentSpeed;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            _rigid = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
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
    }

   
    public void OnUpdateState(float maxHP, float currentHP, float moveSpeed, float dashSpeed)
    {
        this.maxHP = maxHP;
        this.currentHP = currentHP;
        this.moveSpeed = moveSpeed;
        this.dashSpeed = dashSpeed;
    }
}
