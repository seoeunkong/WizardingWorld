using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("대쉬 UI")]
    public Slider dashSlider;

    private float _maxDashValue;
    private PlayerController _playerController;

    [Header("인벤토리 UI")]
    public GameObject inventory;
    public bool inventoryActive {  get; private set; }

    void Start()
    {
        _playerController = Player.Instance.GetComponent<PlayerController>();

        InitializeDashSlider();
        inventory.SetActive(false);
    }

    void Update()
    {
        UpdateDashSlider();
        ClickTab();

    }

    void InitializeDashSlider()
    {
        _maxDashValue = _playerController.setDashTime;

        dashSlider.value = _maxDashValue;
        dashSlider.maxValue = _maxDashValue;
        dashSlider.gameObject.SetActive(false);
    }

    void ClickTab()
    {
        inventoryActive = !inventory.activeSelf;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventory.SetActive(inventoryActive);
        }
    }

    void UpdateDashSlider()
    {
        float currentDashValue = _playerController._currentdashTime;
        dashSlider.value = currentDashValue;

        if (_playerController.player.DashSpeed != _playerController.IsDash() && dashSlider.value == _maxDashValue && dashSlider.IsActive())
        {
            Invoke("dashActivateFalse", 0.5f);
        }
        if (_playerController.player.DashSpeed == _playerController.IsDash() && !dashSlider.IsActive())
        {
            dashSlider.gameObject.SetActive(true);
        }
    }

    void dashActivateFalse()
    {
        dashSlider.gameObject.SetActive(false);
    }


}
