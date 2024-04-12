using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalSphere : CountableObject
{
    public SphereData sphereData { get { return _sphereData; } }

    [Header("팰 스피어 정보"), Tooltip("해당 무기를 쥐었을 때의 local Transform 값 정보 등")]
    [SerializeField] protected SphereData _sphereData;

    // 스피어를 쥐는 손의 트랜스폼
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

    public void Throw(GameObject sphere)
    {
        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        rb.useGravity = true;

        
    }

}
