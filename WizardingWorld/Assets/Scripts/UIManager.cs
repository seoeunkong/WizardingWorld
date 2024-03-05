using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider dashSlider;
    private float _maxDashValue;
    private PlayerController _playerController;

    void Start()
    {
        _playerController = Player.Instance.GetComponent<PlayerController>();
        _maxDashValue = _playerController.setDashTime;

        InitializeDashSlider();
    }

    private void InitializeDashSlider()
    {
        dashSlider.value = _maxDashValue;
        dashSlider.maxValue = _maxDashValue;
        dashSlider.gameObject.SetActive(false);
    }


    void Update()
    {
        UpdateDashSlider();

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
