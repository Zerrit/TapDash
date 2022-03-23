using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public Button start;
    public Button levels;
    public Button settings;

    private void Awake()
    {
        start.onClick.AddListener(MenuButtonClick);
    }

    private void MenuButtonClick()
    {
        GameManager.instance.StartLevel();
        gameObject.SetActive(false);
    }

    private void LevelsButtonClick()
    {

    }

    private void SettingsButtonClick()
    {

    }
}
