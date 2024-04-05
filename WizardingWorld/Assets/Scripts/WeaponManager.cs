using System.Collections.Generic;
using System;
using UnityEngine;


public class WeaponManager
{
    // ���� ���� ��ũ��Ʈ
    public BaseWeapon Weapon { get; private set; }
    public Action<GameObject> unRegisterWeapon { get; set; }
    // ���⸦ ��� ���� Ʈ������
    private Transform handPosition;
    // ���� �� ���� ������Ʈ
    private GameObject weaponObject;
    // ���� WeaponManager�� ��ϵ� ���� ����Ʈ
    private List<GameObject> weapons = new List<GameObject>();

    public WeaponManager(Transform hand)
    {
        handPosition = hand;
    }

    // ���� ���
    public void RegisterWeapon(GameObject weapon)
    {
        if (!weapons.Contains(weapon))
        {
            BaseWeapon weaponInfo = weapon.GetComponent<BaseWeapon>();
            weapon.transform.SetParent(handPosition);

            weapon.transform.localPosition = weaponInfo.handleData.localPosition;
            weapon.transform.localEulerAngles = weaponInfo.handleData.localRotation;
            weapon.transform.localScale = weaponInfo.handleData.localScale;
            weapons.Add(weapon);
            weapon.SetActive(false);
        }
    }

    // ���� ����
    public void UnRegisterWeapon(GameObject weapon)
    {
        if (weapons.Contains(weapon))
        {
            weapons.Remove(weapon);
            unRegisterWeapon.Invoke(weapon);
        }
    }

    // ���� ����
    public void SetWeapon(GameObject weapon)
    {
        if (Weapon == null)
        {
            weaponObject = weapon;
            Weapon = weapon.GetComponent<BaseWeapon>();
           
            weaponObject.SetActive(true);
            Player.Instance.animator.runtimeAnimatorController = Weapon.WeaponAnimator;
            return;
        }

        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].Equals(weapon))
            {
                weaponObject = weapon;
                weaponObject.SetActive(true);
                Weapon = weapon.GetComponent<BaseWeapon>();
               Player.Instance.animator.runtimeAnimatorController = Weapon.WeaponAnimator;
                continue;
            }
            weapons[i].SetActive(false);
        }
    }

    public void UnSetWeapon()
    {
        Weapon = null;

        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(false);
        }
    }

}
