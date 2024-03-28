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

    private Button _btn;

    //������ �ε���
    public int Index { get; private set; }

    //������ �������� �����ϰ� �ִ��� ����

    public bool HasItem => _iconImage.sprite != null;

    public RectTransform SlotRect => _slotRect;
    public RectTransform IconRect => _iconRect;

    private RectTransform _slotRect;
    private RectTransform _iconRect;
    private RectTransform _highlightRect;


    void Start()
    {
        SetIndex(); 
        HideIcon();
        HideText();

        InitRect();
        
    }

    private void ShowIcon() => _iconImage.gameObject.SetActive(true);
    private void HideIcon() => _iconImage.gameObject.SetActive(false);
    private void ShowText() => _amountText.gameObject.SetActive(true);
    private void HideText() => _amountText.gameObject.SetActive(false);

    void InitRect()
    {
        _slotRect = GetComponent<RectTransform>();
        _iconRect = _iconImage.GetComponent<RectTransform>();

    }

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
    public void SetItemImg(Sprite itemSprite)
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

    public void SetItemAmountTxt(int amount)
    {
        if (HasItem && amount > 1)
            ShowText();
        else
            HideText();

        _amountText.text = amount.ToString();
    }

    public void HideItemAmountText()
    {
        HideText();
    }

    /// <summary> �ٸ� ���԰� ������ ������ ��ȯ </summary>
    public void SwapOrMoveIcon(ItemSlotUI other)
    {
        if (other == null) return;
        if (other == this) return; // �ڱ� �ڽŰ� ��ȯ �Ұ�

        var temp = _iconImage.sprite;

        // 1. ��� �������� �ִ� ��� : ��ȯ
        if (other.HasItem) SetItemImg(other._iconImage.sprite);

        // 2. ���� ��� : �̵�
        else RemoveItem();

        other.SetItemImg(temp);
    }
}
