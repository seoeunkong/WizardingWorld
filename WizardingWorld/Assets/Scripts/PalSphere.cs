using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalSphere : CountableObject
{
    public SphereData sphereData { get { return _sphereData; } }

    [Header("�� ���Ǿ� ����"), Tooltip("�ش� ���⸦ ����� ���� local Transform �� ���� ��")]
    [SerializeField] protected SphereData _sphereData;

    // ���Ǿ ��� ���� Ʈ������
    private Transform handPosition;

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

}