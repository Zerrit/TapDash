using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevelId { get; private set; }

    public LevelController levelController { get; private set; }
    public CameraController cameraController { get; private set; }
    public UIController uiController { get; private set; }
    public AudioManager audioManager { get; private set; }
    public ScoreController scoreController { get; private set; }


    //События >>
    public delegate void CommandEvent(Commands command);
    public event CommandEvent tapEvent;  // Эвент тапа по экрану при управлении персонажем

    public UnityEvent crystalTaken; // Событие подбора кристалика
    public UnityEvent levelComplete; // Событие прохождения уровня
    public UnityEvent levelLose; // СОбытие проигрыша
    public UnityEvent levelStart; // Событие старта уровня


    //События <<


    private void Awake()
    {
        if (!instance) instance = this;
        else if (instance == this) Destroy(this);

        LoadCurrentLevel();

        //Подписки на события
        levelComplete.AddListener(LevelComplete);
        levelComplete.AddListener(cameraController.ChangeCameraDificulty);
        levelComplete.AddListener(audioManager.ChangeRunSoundSpeed);
        levelComplete.AddListener(scoreController.ChangeMultipliyer);
        levelLose.AddListener(LevelLose);
        uiController.menuPanel.sound.onClick.AddListener(audioManager.SwitchSoundMode);
    }

    public void StartLevel(int levelNumber) // Старт игры и запуск соответствующего события
    {
        levelController.CreateStartLevel(levelNumber);
        levelStart.Invoke();
    }
    public void StartLastLevel() // Старт игры и запуск соответствующего события
    {
        levelController.CreateStartLevel(currentLevelId);
        levelStart.Invoke();
    }
    public void RestartLevel() // Рестарт игры и запуск соответствующего события
    {
        levelController.RestartLevel();
        levelStart.Invoke();
    }
    public void ReturnToMenu() // Открывает панель меню и запускает удаление уровней
    {
        uiController.OpenMenuPanel();
        levelController.ClearAllLevels();
    }
    private void LevelComplete() // Отметка о прохождении уровня и создание следующего уровня
    {
        currentLevelId++;
        levelController.CreateNextLevel();
    }
    private void LevelLose() // Запуск панели проигрыша
    {
        StartCoroutine(uiController.OpenLosePanel());
    }


    public void TapEventActivate(Commands command) // Активация события Тапа
    {
        tapEvent?.Invoke(command);
    }
    public void SetDifficulty(int index)
    {
        cameraController.ChangeCameraDificulty(index);
        audioManager.ChangeRunSoundSpeed(index);
        scoreController.ChangeMultipliyer(index);
    }

    private void LoadCurrentLevel() // Загружает текущий уровень игрока из PlayerPrefs
    {
        if (PlayerPrefs.HasKey("CurrentLvlId"))
        {
            currentLevelId = PlayerPrefs.GetInt("CurrentLvlId");
        }
        else
        {
            currentLevelId = 0;
            PlayerPrefs.SetInt("CurrentLvlId", 0);
        }
    }
    private void SaveCurrentLevel() // Сохранение текущего уровня в PlayerPrefs
    {
        PlayerPrefs.SetInt("CurrentLvlId", currentLevelId);
        PlayerPrefs.Save();
    }
}
