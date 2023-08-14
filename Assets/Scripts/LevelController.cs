using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Character player;
    public LevelsConstructor levelConstructor;

    private List<Level> levels = new List<Level>(); // Список уровней на сцене
    private List<Arrow> arrows = new List<Arrow>(); // Список указателей на сцене

    private int currentArrowId;
    //private int currentLevelId;

    private Level newestLevel;
                                
    private void Start()
    {
        GameManager.instance.tapEvent += SwitchArrow;
    }

    public void CreateStartLevel(int levelId)
    {
        newestLevel = levelConstructor.ConstructStartLevel(levelId);

        currentArrowId = 0;
        //currentLevelId = 0;


        levels.Add(newestLevel);

        foreach (Arrow arr in newestLevel.arrow)
        {
            arrows.Add(arr);
            player.commands.Add(arr.command);
        }

        arrows[currentArrowId].Activate();

        GameManager.instance.SetDifficulty(levels[0].difficulty);
        player.StartCharacter();

        CreateNextLevel();
    }

    public void CreateNextLevel()
    {
        newestLevel = levelConstructor.ConstructNextLevel(newestLevel);
        newestLevel.transform.Translate(new Vector3(0, 3.95f, 0), Space.Self);

        levels.Add(newestLevel);

        foreach (Arrow arr in newestLevel.arrow)
        {
            arrows.Add(arr);
            player.commands.Add(arr.command);
        }

        if (levels.Count > 3) ClearLastLevel();
    }

    public void RestartLevel() // Рестарт сцены с удаление лишних уровней и перемещением персонажа в в начало текущего уровня
    {
        if (levels.Count > 2)
        {
            ClearLastLevel();
        }
        levels[0].ResetArrows();
        levels[1].ResetArrows();

        currentArrowId = 0;
        arrows[currentArrowId].Activate();
        player.MoveToStart(levels[0].transform.position, levels[0].levelRotation);

        int diff = levels[0].difficulty;
        GameManager.instance.SetDifficulty(diff);
        player.ChangeSpeedDifficulty(diff);
    }

    private void ClearLastLevel() // Удаляет самый старый уровень на сцене (Удаляет команды удалённой сцены из листа комманд персонажа и пройденые стрелки)
    {
        int skippedArrow = levels[0].arrow.Count;

        arrows.RemoveRange(0, skippedArrow);
        currentArrowId -= skippedArrow;
        levels[0].DeleteLevel();
        levels.RemoveAt(0);

        player.ClearOldCommands(skippedArrow);
    }

    public void ClearAllLevels() // Перемещает персонажа в нулевое значение, после чего удаляет все существующие уровни
    {
        player.SetDefaultPos();

        while (levels.Count > 0)
        {
            ClearLastLevel();
        }
    }

    public void SwitchArrow(Commands command)
    {
        arrows[currentArrowId].TurnOff();
        currentArrowId++;

        arrows[currentArrowId].Activate();
    }

}