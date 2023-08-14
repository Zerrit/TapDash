using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum LastSegmentType
{
    Simple,
    TurnRight,
    TurnLeft,
    Jump
}
public class Segment
{
    public Transform segment;
    public LastSegmentType type;

    [SerializeField]
    public Segment(Transform segment, LastSegmentType type)
    {
        this.segment = segment;
        this.type = type;
    }
}

public class Level : MonoBehaviour
{
    public int levelId;

    public int difficulty;

    public TextMeshProUGUI levelNumberText;

    public Transform finishSegment;
    [SerializeField]public Quaternion levelRotation;


    public List<Segment> segmentList = new List<Segment>();     
    public List<Arrow> arrow = new List<Arrow>();
    public Crystal[] crystal;

    private void Awake()
    {
        levelNumberText.text = (levelId + 1).ToString();
        levelRotation = transform.rotation;
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

    public void GetCommands(List<Commands> commands)
    {
        foreach (Arrow arr in arrow)
        {
            commands.Add(arr.command);
        }
    }
}
