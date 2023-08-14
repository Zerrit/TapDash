using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsConstructor : MonoBehaviour
{
    public Level[] levelPrefubs; // Список существующих пирефабов уровней

    public Level ConstructStartLevel(int levelId) // Создание старотового уровня
    {
        return Instantiate(levelPrefubs[levelId], transform);
    }

    public Level ConstructNextLevel(Level newestLevel) // Создание нового уровня
    {
        return Instantiate(levelPrefubs[newestLevel.levelId + 1], newestLevel.finishSegment.position, Quaternion.Euler(newestLevel.finishSegment.eulerAngles), transform);   
    }
}
