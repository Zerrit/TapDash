using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreController : MonoBehaviour
{
    public int amountOfPoints;
    public int scoreMiltiplier;

    public int currentScore;
    public int currentLevel;

    public int bestScore;

    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI currentScoreText;



    public void Start()
    {
        GameManager.instance.dificultyEvent += (SetMultipliyer);
        GameManager.instance.crystalTaken.AddListener(IncreaseScore);
        GameManager.instance.tapEvent += IncreaseScore;
        GameManager.instance.levelStart.AddListener(SetLevel);
        GameManager.instance.levelComplete.AddListener(LevelUp);

        GameManager.instance.levelComplete.AddListener(CompareScores);
        GameManager.instance.levelLose.AddListener(CompareScores);
    }


    private void IncreaseScore()
    {
        ShakeText();
        currentScore += amountOfPoints * scoreMiltiplier;
        currentScoreText.text = currentScore.ToString();
    }

    private void IncreaseScore(Commands command)
    {
        ShakeText();
        if (command == Commands.Jump)
        {
            currentScore += amountOfPoints * 2 * scoreMiltiplier;
            currentScoreText.text = currentScore.ToString();
        }
        else
        {
            currentScore += amountOfPoints * scoreMiltiplier;
            currentScoreText.text = currentScore.ToString();
        }   
    }

    private void SetLevel()
    {
        currentScore = 0;
        currentScoreText.text = currentScore.ToString();
        currentLevel = GameManager.instance.currentLevelId + 1;
        currentLevelText.text = currentLevel.ToString();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentLevelText.text = currentLevel.ToString();
    }

    private void SetMultipliyer(int multipliyer)
    {
        scoreMiltiplier = multipliyer;
    }

    private void ShakeText()
    {
        currentScoreText.transform.DOShakeScale(.1f);
    }

    public void LoadBestScore()
    {
        if (PlayerPrefs.HasKey("BestScore"))
        {
            bestScore = PlayerPrefs.GetInt("BestScore");
        }
        else
        {
            bestScore = 0;
        }
    }

    public void SaveBestScore()
    {
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();
    }

    private void CompareScores()
    {
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            SaveBestScore();
        }   
    }
}
