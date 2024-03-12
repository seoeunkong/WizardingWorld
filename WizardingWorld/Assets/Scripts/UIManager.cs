using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("´ë½¬ UI")]
    public Slider dashSlider;

    private float _maxDashValue;
    private PlayerController _playerController;

    [Header("Bag UI")]
    public GameObject Bag;
    public bool bagActive {  get; private set; }

    private void Awake()
    {
        Bag.SetActive(false);
    }


    void Start()
    {
        _playerController = Player.Instance.GetComponent<PlayerController>();

        InitializeDashSlider();
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
        bagActive = !Bag.activeSelf;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Bag.SetActive(bagActive);
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
