using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalSphere : CountableObject
{
    public SphereData sphereData { get { return _sphereData; } }
    public bool isToCaptureMonster() => toCaptureMonster;
    public void ChangeSphereMode() => toCaptureMonster = !toCaptureMonster; 

    [Header("팰 스피어 정보"), Tooltip("해당 무기를 쥐었을 때의 local Transform 값 정보 등")]
    [SerializeField] protected SphereData _sphereData;

    private Transform handPosition;     // 스피어를 쥐는 손의 트랜스폼
    private bool toCaptureMonster = true; //팰 투척용 or 스피어 투척용 

    private void Awake()
    {
        InitializeData(sphereData);
    }

    private void Start()
    {
        handPosition = Player.Instance.rightHand;
    }

    public override void InitializeData(ObjectData objData)
    {
        base.InitializeData(objData);
        _countableData = sphereData;
    }

    //플레이어 손에 스피어 장착 
    public void SetPalSphere()
    {
        GameObject sphere = IniSphere(this);
        if (sphere == null) return;

        PalSphere sphereInfo = sphere.GetComponent<PalSphere>();
        sphere.transform.SetParent(handPosition);

        sphere.transform.localPosition = sphereInfo.sphereData.localPosition;
        sphere.transform.localEulerAngles = sphereInfo.sphereData.localRotation;
        sphere.transform.localScale = sphereInfo.sphereData.localScale;

        sphere.SetActive(true);
    }

    //스피어 인스턴스화 
    GameObject IniSphere(BaseObject baseObject)
    {
        GameObject sphere = baseObject.gameObject;

        Collider collider = sphere.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        DropItem item = sphere.GetComponent<DropItem>();
        if (item != null) item.enabled = false;

        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        return sphere;
    }

    //플레이어가 스피어를 던지기 전에 스피어 속성 업데이트 
    public void InitThrowSphere()
    {
        this.transform.SetParent(null);
        this.GetComponent<Collider>().enabled = true;

        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!toCaptureMonster)
            {
                BaseObject pal = Inventory.Instance.GetCurrentPal();
                if(pal != null)
                {
                    pal.transform.position = transform.position + new Vector3(0,-0.1f,0f);
                    pal.gameObject.SetActive(true);
                    transform.gameObject.SetActive(false);
                }
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(toCaptureMonster) transform.gameObject.SetActive(false);
        }
    }

}
