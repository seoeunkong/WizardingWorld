using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Tooltip("아이템 아이콘 이미지")]
    [SerializeField] private Image _iconImage;

    [Tooltip("아이템 개수 텍스트")]
    [SerializeField] private Text _amountText;

    [Space]
    [Tooltip("하이라이트 이미지 알파 값")]
    [SerializeField] private float _highlightAlpha = 0.5f;

    [Tooltip("하이라이트 소요 시간")]
    [SerializeField] private float _highlightFadeDuration = 0.2f;

    //슬롯의 인덱스
    public int Index { get; private set; }

    //슬롯이 아이템을 보유하고 있는지 여부

    public bool HasItem => _iconImage.sprite != null;


    void Start()
    {
        SetIndex(); 
    }

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

    public void SetItem(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            _iconImage.sprite = itemSprite;
            float amount = float.Parse(_amountText.text) + 1;
            _amountText.text = amount.ToString();
            ShowText();
        }
    }

}
