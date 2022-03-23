using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour
{
    public Button restart;
    public Button levels;
    public Button menu;
    public Button settings;

    private void OnEnable()
    {
        restart.onClick.AddListener(RestartButtonClick);
        menu.onClick.AddListener(MenuButtonClick);
    }

    private void OnDisable()
    {
        menu.onClick.RemoveAllListeners();
        restart.onClick.RemoveAllListeners();
        //levels.onClick.RemoveAllListeners();
        //settings.onClick.RemoveAllListeners();
    }

    private void RestartButtonClick()
    {
        GameManager.instance.RestartLevel();
        gameObject.SetActive(false);
    }

    private void LevelsButtonClick()
    {

    }

    private void MenuButtonClick()
    {
        GameManager.instance.ReturnToMenu();
        gameObject.SetActive(false);
    }

    private void SettingsButtonClick()
    {

    }
}
