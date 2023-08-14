using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour
{
    public Button restart;
    public Button menu;

    private void OnEnable()
    {
        restart.onClick.AddListener(RestartButtonClick);
        menu.onClick.AddListener(MenuButtonClick);
    }

    private void OnDisable()
    {
        menu.onClick.RemoveAllListeners();
        restart.onClick.RemoveAllListeners();
    }

    private void RestartButtonClick()
    {
        GameManager.instance.RestartLevel();
        gameObject.SetActive(false);
    }

    private void MenuButtonClick()
    {
        GameManager.instance.ReturnToMenu();
        gameObject.SetActive(false);
    }
}
