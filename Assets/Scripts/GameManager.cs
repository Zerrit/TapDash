using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevelId;

    public LevelsConstructor levelConstructor;
    public UIController uiController;
    public AudioManager audioManager;

    //События >>
    public delegate void CommandEvent(Commands command);
    public delegate void GameDificulty(int dificultyParameter);

    public event CommandEvent tapEvent;  // Эвента тапа по экрану при управлении персонажем
    public event GameDificulty dificultyEvent; // Эвент задания уровня сложности

    public UnityEvent crystalTaken;
    public UnityEvent levelComplete; 
    public UnityEvent levelLose;
    public UnityEvent levelStart;
    //События <<


    private void Awake()
    {
        if (!instance) instance = this;
        else if (instance == this) Destroy(this);

        LoadCurrentLevel(); 

        //Подписки на события
        levelComplete.AddListener(LevelComplete);
        levelLose.AddListener(LevelLose);
    }

    public void StartLevel() // Старт игры и запуск соответствующего события
    {
        levelConstructor.CreateLevel(currentLevelId);
        levelStart.Invoke();
    }
    public void RestartLevel() // Рестарт игры и запуск соответствующего события
    {
        levelConstructor.RestartLevel();
        levelStart.Invoke();
    }
    public void ReturnToMenu() // Открывает панель меню и запускает удаление уровней
    {
        uiController.OpenMenuPanel();
        levelConstructor.ClearAllLevels();
    }
    private void LevelComplete() // Отметка о прохождении уровня и создание следующего уровня
    {
        currentLevelId++;
        levelConstructor.LoadNextLevel();
    }
    private void LevelLose() // Запуск панели проигрыша
    {
        StartCoroutine(uiController.OpenLosePanel());
    }


    public void TapEventActivate(Commands command) // Активация события Тапа
    {
        tapEvent?.Invoke(command);
    }
    public void DificultyEventActivate(int speedDif)
    {
        dificultyEvent?.Invoke(speedDif);
    }



    private void LoadCurrentLevel() // Загружает текущий уровень игрока из PlayerPrefs
    {
        if (PlayerPrefs.HasKey("MaxLvl"))
        {
            currentLevelId = PlayerPrefs.GetInt("MaxLvl");
        }
        else
        {
            currentLevelId = 0;
        }
    }
    private void SaveCurrentLevel() // Сохранение текущего уровня в PlayerPrefs
    {
        PlayerPrefs.SetInt("MaxLvl", currentLevelId);
        PlayerPrefs.Save();
    }
}
