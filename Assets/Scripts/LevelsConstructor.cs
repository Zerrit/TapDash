using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsConstructor : MonoBehaviour
{
    public Character player;

    public Level[] levelPrefubs; // Список существующих пирефабов уровней
    public List<Level> levelList = new List<Level>(); // Список уровней на сцене

    public Level newestLevel; // Ссылка на новейший созданный уровень

    public void CreateLevel(int levelId) // Создание старотового уровня
    {
        if(levelId + 2 > levelPrefubs.Length)
        {
            newestLevel = Instantiate(levelPrefubs[1], transform);
        }
        else
        {
            newestLevel = Instantiate(levelPrefubs[levelId], transform);
        }
        
        levelList.Add(newestLevel);

        foreach (Arrow arr in newestLevel.arrow)
        {
            player.arrows.Add(arr);
        }

        LoadNextLevel();

        GameManager.instance.DificultyEventActivate(levelList[0].speedDificulty);
        player.SetStartCommand();
    }

    public void LoadNextLevel() // Создание нового уровня
    {
        if (newestLevel.levelId + 1 == levelPrefubs.Length)
        {
            newestLevel = Instantiate(levelPrefubs[newestLevel.levelId - 1], newestLevel.endLevelPoint.position + new Vector3(0, 2, 0), Quaternion.identity, transform);
        }
        else
        {
            newestLevel = Instantiate(levelPrefubs[newestLevel.levelId + 1], newestLevel.endLevelPoint.position + new Vector3(0, 2, 0), Quaternion.identity, transform);
        }
        

        foreach (Arrow arr in newestLevel.arrow)
        {
            player.arrows.Add(arr);
        }

        levelList.Add(newestLevel);
        if (levelList.Count > 3)
        {
            ClearLevel();
        }     
    }

    public void RestartLevel() // Рестарт сцены с удаление лишних уровней и перемещением персонажа в в начало текущего уровня
    {
        if(levelList.Count > 2)
        {
            ClearLevel();
            levelList[0].ResetArrows();
            levelList[1].ResetArrows();

            player.MoveToStart(levelList[0].transform);
        }
        else
        {
            levelList[0].ResetArrows();
            levelList[1].ResetArrows();

            player.MoveToStart(levelList[0].transform);
        }

        GameManager.instance.DificultyEventActivate(levelList[0].speedDificulty);
    }

    private void ClearLevel() // Удаляет самый старый уровень на сцене (Удаляет команды удалённой сцены из листа комманд персонажа)
    {
        foreach (Arrow arr in levelList[0].arrow)
        {
            player.arrows.RemoveAt(0);
            player.currentCommandNumber--;
        }

        levelList[0].DeleteLevel();
        levelList.RemoveAt(0);
    }

    public void ClearAllLevels() // Перемещает персонажа в нулевое значение, после чего удаляет все существующие уровни
    {
        player.SetDefaultPos();

        while(levelList.Count > 0)
        {
            ClearLevel();
        }
    }
}
