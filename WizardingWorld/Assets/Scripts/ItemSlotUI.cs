using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Tooltip("������ ������ �̹���")]
    [SerializeField] private Image _iconImage;

    [Tooltip("������ ���� �ؽ�Ʈ")]
    [SerializeField] private Text _amountText;

    [Space]
    [Tooltip("���̶���Ʈ �̹��� ���� ��")]
    [SerializeField] private float _highlightAlpha = 0.5f;

    [Tooltip("���̶���Ʈ �ҿ� �ð�")]
    [SerializeField] private float _highlightFadeDuration = 0.2f;

    //������ �ε���
    public int Index { get; private set; }

    //������ �������� �����ϰ� �ִ��� ����

    public bool HasItem => _iconImage.sprite != null;


    void Start()
    {
        SetIndex(); 
    }
    private void ShowIcon() => _iconImage.gameObject.SetActive(true);
    private void HideIcon() => _iconImage.gameObject.SetActive(false);
    private void ShowText() => _amountText.gameObject.SetActive(true);
    private void HideText() => _amountText.gameObject.SetActive(false);

    private void SetIndex()
    {
       GameObject inventory = transform.parent.gameObject;
       foreach(ItemSlotUI item in inventory.GetComponentsInChildren<ItemSlotUI>())
        {
            if (item.name == this.name) break;
            Index++;
        }
    }

    //���Կ� ������ ���
    public void SetItem(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            _iconImage.sprite = itemSprite;
            ShowIcon();
        }
        else
        {
            RemoveItem();
        }
    }

    //���Կ��� ������ ���� 
    public void RemoveItem()
    {
        _iconImage.sprite = null;
        HideIcon();
        HideText();
    }

    public void SetItemAmount(int amount)
    {
        if (HasItem && amount > 1)
            ShowText();
        else
            HideText();

        _amountText.text = amount.ToString();
    }

}
