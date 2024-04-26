using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalSphere : CountableObject
{
    public SphereData sphereData { get { return _sphereData; } }
    public bool isToCaptureMonster() => toCaptureMonster;
    public void ChangeSphereMode() => toCaptureMonster = !toCaptureMonster; 

    [Header("�� ���Ǿ� ����"), Tooltip("�ش� ���⸦ ����� ���� local Transform �� ���� ��")]
    [SerializeField] protected SphereData _sphereData;

    private Transform handPosition;     // ���Ǿ ��� ���� Ʈ������
    private bool toCaptureMonster = true; //�� ��ô�� or ���Ǿ� ��ô�� 

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

    //�÷��̾� �տ� ���Ǿ� ���� 
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

    //���Ǿ� �ν��Ͻ�ȭ 
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

    //�÷��̾ ���Ǿ ������ ���� ���Ǿ� �Ӽ� ������Ʈ 
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
