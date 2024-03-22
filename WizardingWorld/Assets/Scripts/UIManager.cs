using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("대쉬 UI")]
    [SerializeField] private Slider _dashSlider;

    private float _maxDashValue;
    private PlayerController _playerController;

    [Header("인벤토리 UI")]
    [SerializeField]private GameObject _inventory;
    public void ShowInventory() => _inventory.SetActive(true);
    public void HideInventory() => _inventory.SetActive(false);
    private bool _inventoryActive;

    [Header("Z키 PopUp UI")]
    [SerializeField] private GameObject _fPopUpUi;

    void Start()
    {
        _playerController = Player.Instance.GetComponent<PlayerController>();

        InitializeDashSlider();

        _inventory.SetActive(false);

        _fPopUpUi.SetActive(false);
    }

    void Update()
    {
        UpdateDashSlider();
        ClickTab();
        ActivateZPopUp();
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

    void ActivateZPopUp()
    {
        if (_playerController.DropItemPos == null) 
        {
            _fPopUpUi.SetActive(false);
            return;
        }

       if((_fPopUpUi.activeSelf && _fPopUpUi.transform.position != _playerController.DropItemPos.position) || !_fPopUpUi.activeSelf)
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



}
