using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public Button start;
    public Button sound;
    public GameObject soundDisableIcon;
    public GameObject levelSelectionScreen;

    private bool inManeMenu = true;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    public Button[] levelButtons;


    private void Awake()
    {
        start.onClick.AddListener(StartButtonClick);
        sound.onClick.AddListener(TurnSoundMode);
    }

    private void Update()
    {
        CheckSwipe();
    }

    private void StartButtonClick()
    {
        GameManager.instance.StartLastLevel();
        gameObject.SetActive(false);
    }
    private void TurnSoundMode()
    {
        if(soundDisableIcon.activeInHierarchy) soundDisableIcon.SetActive(false);
        else soundDisableIcon.SetActive(true);
    }
    private void CheckSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;

                // Вычисляем расстояние свайпа
                float swipeDistance = (touchEndPos - touchStartPos).magnitude;

                // Проверяем, был ли свайп достаточно длинным
                if (swipeDistance >= 50f)
                {
                    // Определяем направление свайпа
                    Vector2 swipeDirection = touchEndPos - touchStartPos;

                    if (swipeDirection.x < 0 || inManeMenu)
                    {
                       OpenLevelsScreen();
                    }
                    else if (swipeDirection.x > 0)
                    {
                       // Здесь выполняйте действия для свайпа вниз в меню
                    }
                }
            }
        }
    }

    private void OpenLevelsScreen()
    {
        inManeMenu = false;
        levelSelectionScreen.gameObject.SetActive(true);
        levelSelectionScreen.transform.localPosition = new Vector2(Screen.width, 0);
        levelSelectionScreen.transform.DOLocalMoveX(0, 1f).SetEase(Ease.Linear);

        start.image.DOFade(0, 0.5f);
        sound.image.DOFade(0, 0.5f).OnComplete(() =>
        {
            start.gameObject.SetActive(false);
            sound.gameObject.SetActive(false);
        });
    }
}
