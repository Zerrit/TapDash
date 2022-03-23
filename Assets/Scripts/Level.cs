using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level : MonoBehaviour
{
    public int levelId;

    public int speedDificulty;
    public int cameraDificulty;

    public TextMeshProUGUI levelNumber;
    public Transform endLevelPoint;

    public List<Arrow> arrow = new List<Arrow>();
    public Crystal[] crystal;

    private void Awake()
    {
        levelNumber.text = (levelId + 1).ToString();
    }

    public void ResetArrows()
    {
        foreach (Arrow arr in arrow)
        {
            arr.SetDefault();
        }

        foreach (Crystal crys in crystal)
        {
            crys.Reset();
        }
    }

    public void DeleteLevel()
    {
        Destroy(gameObject);
    }
}
