using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public int levelNumber;
    public Button button;
    public TextMeshProUGUI levelText;

    private void Awake()
    {
        levelText.text = levelNumber.ToString();
        button.onClick.AddListener(StartLevel);
    }

    private void StartLevel()
    {
        GameManager.instance.StartLevel(levelNumber);
    }

}
