using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider dashSlider;
    private float _maxDashValue;


    void Start()
    {
        _maxDashValue = Player.Instance.GetComponent<PlayerController>().setDashTime;

        dashSlider.value = _maxDashValue;
        dashSlider.maxValue = _maxDashValue;
        dashSlider.gameObject.SetActive(false);
    }


    void Update()
    {
        dashActivate(Player.Instance.GetComponent<PlayerController>());

    }

    void dashActivateFalse()
    {
        dashSlider.gameObject.SetActive(false);
    }

    void dashActivateTrue()
    {
        dashSlider.gameObject.SetActive(true);
    }

    void dashActivate(PlayerController controller)
    {
        float currentDashValue = controller._currentdashTime;
        dashSlider.value = currentDashValue;

        if(controller.player.DashSpeed != controller.IsDash() && dashSlider.value == _maxDashValue && dashSlider.IsActive())
        {
            Invoke("dashActivateFalse", 0.5f);
        }
        if (controller.player.DashSpeed == controller.IsDash() && !dashSlider.IsActive())
        {
            dashSlider.gameObject.SetActive(true);
        }
    }


}
