using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("�뽬 UI")]
    [SerializeField] private Slider _dashSlider;
    private float _maxDashValue;
    private PlayerController _playerController;

    [Header("�κ��丮 UI")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _inventory;
    private ItemSlotUI[] _itemSlotUIs; //�κ��丮 ���� UI 

    [Header("�� UI")]
    [SerializeField] private GameObject _palPanel;
    [SerializeField] private Image _palImg;

    #region #�巡�׾ص��
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    private ItemSlotUI _beginDragSlot; // ���� �巡�׸� ������ ����
    private Transform _beginDragIconTransform; // �ش� ������ ������ Ʈ������

    private Vector3 _beginDragIconPoint;   // �巡�� ���� �� ������ ��ġ
    private Vector3 _beginDragCursorPoint; // �巡�� ���� �� Ŀ���� ��ġ
    private int _beginDragSlotSiblingIndex;
    #endregion

    public void ShowInventory() => _inventory.SetActive(true);
    public void HideInventory() => _inventory.SetActive(false);
    private bool _inventoryActive;

    [Header("fŰ PopUp UI")]
    [SerializeField] private GameObject _fPopUpUi;
    private void ShowIcon() => _palImg.gameObject.SetActive(true);
    private void HideIcon() => _palImg.gameObject.SetActive(false);

    private void Awake()
    {
        _gr = _canvas.GetComponent<GraphicRaycaster>();
    }

    void Start()
    {

        InitUI();

    }

    void Update()
    {
        UpdateDashSlider();

        ClickTab();

        ActivateFPopUp();

        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();

        ShowPalPanel();
    }

    void ShowPalPanel()
    {
        if (_inventory.activeSelf) _palPanel.SetActive(false);
        else _palPanel.SetActive(true);
    }

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();

        _ped.position = Input.mousePosition;

        _gr.Raycast(_ped, _rrList);

        if (_rrList.Count == 0)
            return null;

        if (_rrList[0].gameObject.CompareTag("UI/Slot")) return _rrList[0].gameObject.GetComponent<T>();

        return _rrList[1].gameObject.GetComponent<T>();
    }

    private void InitUI()
    {
        _playerController = Player.Instance.GetComponent<PlayerController>();

        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>();

        _inventory.SetActive(false);
        _fPopUpUi.SetActive(false);
        _itemSlotUIs = GetComponentsInChildren<ItemSlotUI>();

        InitializeDashSlider();
        HideIcon();

    }

    void InitializeDashSlider()
    {
        _maxDashValue = _playerController.setDashTime;

        _dashSlider.value = _maxDashValue;
        _dashSlider.maxValue = _maxDashValue;
        _dashSlider.gameObject.SetActive(false);
    }

    void ClickTab()
    {
        _inventoryActive = !_inventory.activeSelf;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _inventory.SetActive(_inventoryActive);
        }
    }

    void UpdateDashSlider()
    {
        float currentDashValue = _playerController._currentdashTime;
        _dashSlider.value = currentDashValue;

        if (_playerController.player.DashSpeed != _playerController.IsDash() && _dashSlider.value == _maxDashValue && _dashSlider.IsActive())
        {
            Invoke("dashActivateFalse", 0.5f);
        }
        if (_playerController.player.DashSpeed == _playerController.IsDash() && !_dashSlider.IsActive())
        {
            _dashSlider.gameObject.SetActive(true);
        }
    }

    void dashActivateFalse()
    {
        _dashSlider.gameObject.SetActive(false);
    }

    void ActivateFPopUp()
    {
        if (_playerController.DropItemPos == null)
        {
            _fPopUpUi.SetActive(false);
            return;
        }
        //Debug.Log(_playerController.DropItemPos);

        if ((_fPopUpUi.activeSelf && _fPopUpUi.transform.position != _playerController.DropItemPos.position) || !_fPopUpUi.activeSelf)
        {
            Vector3 getDropItemPos = _playerController.DropItemPos.position;
            _fPopUpUi.transform.position = getDropItemPos + Vector3.up;
            _fPopUpUi.transform.rotation = Camera.main.transform.rotation;

            List<GameObject> PopUi = FindWithTag(_fPopUpUi, "UI/Text");
            PopUi[0].GetComponent<Text>().text = _playerController.DropItemPos.GetComponent<BaseObject>()._objData.Name;

            _fPopUpUi.SetActive(true);
        }
    }

    List<GameObject> FindWithTag(GameObject parent, string tag)
    {
        List<GameObject> taggedChildren = new List<GameObject>();

        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                taggedChildren.Add(child.gameObject);
            }
        }

        return taggedChildren;
    }

    private void OnPointerDown()
    {
        // Left Click : Begin Drag
        if (Input.GetMouseButtonDown(0))
        {
            _beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            // �������� ���� �ִ� ���Ը� �ش�
            if (_beginDragSlot != null && _beginDragSlot.HasItem)
            {
                // ��ġ ���, ���� ���
                _beginDragIconTransform = _beginDragSlot.IconRect.transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;
            }
            else
            {
                _beginDragSlot = null;
            }
        }
    }
    /// <summary> �巡���ϴ� ���� </summary>
    private void OnPointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            // ��ġ �̵�
            _beginDragIconTransform.position =
                _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
        }
    }
    /// <summary> Ŭ���� �� ��� </summary>
    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            // End Drag
            if (_beginDragSlot != null)
            {
                // ��ġ ����
                _beginDragIconTransform.position = _beginDragIconPoint;

                // �巡�� �Ϸ� ó��
                EndDrag();

                // ���� ����
                _beginDragSlot = null;
                _beginDragIconTransform = null;
            }
        }
    }

    private void EndDrag()
    {
        ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if (endDragSlot != null)
        {
            TrySwapItems(_beginDragSlot, endDragSlot);
        }
    }

    //�� ������ ������ ��ȯ 
    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        Inventory inv = GetComponent<Inventory>();

        if (from == to || !inv.isValidSwap(from.Index, to.Index))
        {
            return;
        }

        from.SwapOrMoveIcon(to);

        inv.Swap(from.Index, to.Index);
    }

    //�� ���
    public void SetPalImg(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            _palImg.sprite = itemSprite;
            ShowIcon();
        }
        else
        {
            HideIcon();
        }
    }

}
