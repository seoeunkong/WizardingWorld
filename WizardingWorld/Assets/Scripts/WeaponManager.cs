using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;


public class WeaponManager
{
    // 현재 무기 스크립트
    public BaseWeapon Weapon { get; private set; }
    public Action<GameObject> unRegisterWeapon { get; set; }
    // 무기를 쥐는 손의 트랜스폼
    private Transform handPosition;
    // 현재 내 무기 오브젝트
    private GameObject weaponObject;
    // 현재 WeaponManager에 등록된 무기 리스트
    private List<BaseWeapon> weapons = new List<BaseWeapon>();
    private List<ObjectData> weaponsData = new List<ObjectData>();

    public bool hasWeapon() => weaponObject != null;

    public WeaponManager(Transform hand)
    {
        handPosition = hand;
    }

    // 무기 등록
    public void RegisterWeapon(BaseWeapon weaponInfo)
    {
        if (weaponInfo == null) return;
       
        if (!weaponsData.Contains(weaponInfo.handleData))
        {
            GameObject weapon = InitWeapon(weaponInfo);
            weapon.transform.SetParent(handPosition);

            weapon.transform.localPosition = weaponInfo.handleData.localPosition;
            weapon.transform.localEulerAngles = weaponInfo.handleData.localRotation;
            weapon.transform.localScale = weaponInfo.handleData.localScale;

            weapons.Add(weaponInfo);
            weaponsData.Add(weaponInfo.handleData);
            weapon.SetActive(false);
        }
    }

    // 무기 삭제
    public void UnRegisterWeapon(GameObject weapon)
    {
        BaseWeapon weaponInfo = weapon.GetComponent<BaseWeapon>();
        if (weaponsData.Contains(weaponInfo.handleData))
        {
            weapons.Remove(weaponInfo);
            weaponsData.Remove(weaponInfo.handleData);
            unRegisterWeapon.Invoke(weapon);
        }
    }

    // 무기 변경
    public void SetWeapon(BaseWeapon weaponInfo)
    {
        if (Weapon == null)
        {
            weaponObject = weaponInfo.gameObject;
            Weapon = weaponInfo;
           
            weaponObject.SetActive(true);
            Player.Instance.animator.runtimeAnimatorController = Weapon.WeaponAnimator;
            return;
        }

        for (int i = 0; i < weapons.Count; i++)
        {
            if (weaponsData[i].Equals(weaponInfo.handleData))
            {
                weaponObject = weapons[i].gameObject;
                weaponObject.SetActive(true);
                Weapon = weaponInfo;
               Player.Instance.animator.runtimeAnimatorController = Weapon.WeaponAnimator;
                continue;
            }
            weapons[i].gameObject.SetActive(false); 
        }
    }

    GameObject InitWeapon(BaseObject baseObject)
    {
        GameObject weapon = baseObject.gameObject;

        Collider collider = weapon.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        DropItem item = weapon.GetComponent<DropItem>();
        if (item != null) item.enabled = false;

        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        return weapon;
    }

    public void UnSetWeapon()
    {
        //Weapon = null;
        weaponObject = null;

        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }
    }

}
