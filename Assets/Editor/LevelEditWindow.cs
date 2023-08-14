using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelEditWindow : EditorWindow
{
    private static LevelEditWindow instance;

    private float fadePanel;
    private int segmentsCount;
    private int difficulty;
    private int leftWayRotation;
    private int rightWayRotation;
    private string levelName;
    private Vector2 lastSegmentEndPos;

    private Level creatingLevel;
    private Transform lastSegment;

    private GameObject startSegment;
    private GameObject roadSegment;
    private GameObject turnSegment;
    private GameObject singleJumpSegment;
    private GameObject jumpSegment;
    private GameObject jumpEndSegment;
    private GameObject finishSegment;

    [MenuItem("My Tool/Show LevelConstructor")]
    public static void ShowWindow()
    {
        instance = GetWindow<LevelEditWindow>(true, "Level Constructor");
    }
    private void OnEnable()
    {
        startSegment = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Segments/Start.prefab") as GameObject;
        roadSegment = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Segments/Road.prefab") as GameObject;
        turnSegment = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Segments/Turn.prefab") as GameObject;
        jumpSegment = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Segments/Jump.prefab") as GameObject;
        singleJumpSegment = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Segments/SingleJump.prefab") as GameObject;
        jumpEndSegment = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Segments/JumpEnd.prefab") as GameObject;
        finishSegment = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Segments/Finish.prefab") as GameObject;

        fadePanel = 0f;
    }
    private void OnDestroy()
    {
        if(creatingLevel) DestroyImmediate(creatingLevel.gameObject, false);
    }
    private void BuildSegment(GameObject segment, LastSegmentType type, Quaternion rotation)
    {
        lastSegment = Instantiate(segment, lastSegmentEndPos, rotation, creatingLevel.transform).transform;
        lastSegmentEndPos = lastSegment.GetChild(0).position;
        creatingLevel.segmentList.Add(new Segment(lastSegment, type));
        segmentsCount++;

        if(type == LastSegmentType.TurnLeft)
        {
            creatingLevel.arrow.Add(lastSegment.GetComponentInChildren<Arrow>());
            creatingLevel.arrow[creatingLevel.arrow.Count - 1].command = Commands.TurnLeft;
            leftWayRotation += 90;
            rightWayRotation -= 90;
        }
        else if(type == LastSegmentType.TurnRight)
        {
            creatingLevel.arrow.Add(lastSegment.GetComponentInChildren<Arrow>());
            creatingLevel.arrow[creatingLevel.arrow.Count - 1].command = Commands.TurnRight;
            leftWayRotation -= 90;
            rightWayRotation += 90;
        }
        else if (type == LastSegmentType.Jump)
        {
            creatingLevel.arrow.Add(lastSegment.GetComponentInChildren<Arrow>());
            //creatingLevel.arrow[creatingLevel.arrow.Count - 1].command = Commands.Jump;

        }
    }
    private void BuildSegment(GameObject segment, LastSegmentType type, Quaternion rotation, int range)
    {
        lastSegment = Instantiate(segment, lastSegmentEndPos, rotation, creatingLevel.transform).transform;
        lastSegment.localScale = new Vector3(1, range, 1);
        lastSegmentEndPos = lastSegment.GetChild(0).position;
        creatingLevel.segmentList.Add(new Segment(lastSegment, type));
        segmentsCount++;
    }

    private void CreateLevel()
    {
        creatingLevel = new GameObject("New Level").AddComponent<Level>();
        if (PlayerPrefs.HasKey("MaxLvl")) creatingLevel.levelId = PlayerPrefs.GetInt("MaxLvl") + 1;
        else creatingLevel.levelId = 0;
        BuildSegment(startSegment, LastSegmentType.Simple, Quaternion.identity);
        creatingLevel.levelNumberText = lastSegment.GetComponentInChildren<TextMeshProUGUI>();
    }
    private void DeleteSegment()
    {
        if (creatingLevel.segmentList.Count == 1) return;

        if (creatingLevel.segmentList[segmentsCount - 1].type == LastSegmentType.TurnLeft) 
        {
            creatingLevel.arrow.RemoveAt(creatingLevel.arrow.Count - 1);
            rightWayRotation += 90; 
            leftWayRotation -= 90;
        }
        else if(creatingLevel.segmentList[segmentsCount - 1].type == LastSegmentType.TurnRight)
        {
            creatingLevel.arrow.RemoveAt(creatingLevel.arrow.Count - 1);
            leftWayRotation += 90; 
            rightWayRotation -= 90; 
        }
        else if (creatingLevel.segmentList[creatingLevel.segmentList.Count - 1].type == LastSegmentType.Jump)
        {
            creatingLevel.arrow.RemoveAt(creatingLevel.arrow.Count - 1);
        }
        creatingLevel.segmentList.RemoveAt(segmentsCount - 1);
        DestroyImmediate(lastSegment.gameObject, false);
        segmentsCount--;
        lastSegmentEndPos = creatingLevel.segmentList[segmentsCount - 1].segment.GetChild(0).position;
        lastSegment = creatingLevel.segmentList[segmentsCount - 1].segment;

    }
    private void SaveLevelToPrefab()
    {
        string savePath = "Assets/Prefabs/Levels/" + levelName + ".prefab";
        savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);
        PrefabUtility.SaveAsPrefabAssetAndConnect(creatingLevel.gameObject, savePath, InteractionMode.AutomatedAction);
        DestroyImmediate(creatingLevel.gameObject ,false);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Конструктор уровней TapTapDash");
        //---------------------------------------------------------------------------//  СТАРТ КОНСТРУКТОРА

        if (GUILayout.Button("Создать уровень"))
        {
            fadePanel = 1f;
            lastSegmentEndPos = Vector2.zero;
            segmentsCount = 0;
            leftWayRotation = 0;
            rightWayRotation = 0;

            CreateLevel();
        }

        EditorGUILayout.Space(20);

        //---------------------------------------------------------------------------//  НАЧАЛО МЕНЮ КОНСТРУКТОРА
        if (EditorGUILayout.BeginFadeGroup(fadePanel))
        {
            //-----------------------------------------------------------------------//  СОЗДАНИЕ ДОРОЖЕК
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Неболшая дорожка", GUILayout.Height(30)))
            {
                BuildSegment(roadSegment, LastSegmentType.Simple, Quaternion.Euler(0,0,leftWayRotation), 1);
            }
            if (GUILayout.Button("Средняя дорожка", GUILayout.Height(30)))
            {
                BuildSegment(roadSegment, LastSegmentType.Simple, Quaternion.Euler(0, 0, leftWayRotation), 2);
            }
            if (GUILayout.Button("Длинная дорожка", GUILayout.Height(30)))
            {
                BuildSegment(roadSegment, LastSegmentType.Simple, Quaternion.Euler(0, 0, leftWayRotation), 3);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);

            //---------------------------------------------------------------------------// СОЗДАНИЕ ПОВОРОТОВ
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Поворот налево", GUILayout.Height(50)))
            {
                BuildSegment(turnSegment, LastSegmentType.TurnLeft, Quaternion.Euler(0, 0, leftWayRotation));
            }
            if (GUILayout.Button("Поворот направо", GUILayout.Height(50)))
            {
                BuildSegment(turnSegment, LastSegmentType.TurnRight, Quaternion.Euler(0, 180, rightWayRotation));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);

            //---------------------------------------------------------------------------// СОЗДАНИЕ СЕГМЕНТОВ ДЛЯ ПРЫЖКОВ
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Прыжок", GUILayout.Height(30)))
            {
                BuildSegment(jumpSegment, LastSegmentType.Jump, Quaternion.Euler(0,0,leftWayRotation));
            }

            if (GUILayout.Button("Одиночный прыжок", GUILayout.Height(30)))
            {
                BuildSegment(singleJumpSegment, LastSegmentType.Jump, Quaternion.Euler(0, 0, leftWayRotation));
            }

            if (GUILayout.Button("Зона приземления", GUILayout.Height(30)))
            {
                BuildSegment(jumpEndSegment, LastSegmentType.Simple, Quaternion.Euler(0, 0, leftWayRotation));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(30);
            //---------------------------------------------------------------------------// УДАЛЕНИЕ СЕГМЕНТА

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Удалить последний сегмент", GUILayout.Height(30)))
            {
                DeleteSegment();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(30);
            //---------------------------------------------------------------------------// НАСТРОЙКИ УРОВНЯ

            EditorGUILayout.BeginHorizontal();

            difficulty = EditorGUILayout.IntSlider(difficulty, 1, 3);
            EditorGUILayout.LabelField("Уровень сложности");

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);
            //---------------------------------------------------------------------------// ЗАВЕРШЕНИЕ СОЗДАНИЯ УРОВНЯ

            EditorGUILayout.BeginHorizontal();
            levelName = EditorGUILayout.TextField(levelName);
            EditorGUILayout.LabelField("Введите название уровня");



            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);
            //---------------------------------------------------------------------------// ЗАВЕРШЕНИЕ СОЗДАНИЯ УРОВНЯ


            if (GUILayout.Button("Закончить создание уровня"))
            {
                BuildSegment(finishSegment, LastSegmentType.Simple, Quaternion.Euler(0,0,leftWayRotation));

                creatingLevel.finishSegment = lastSegment;
                creatingLevel.difficulty = difficulty;

                SaveLevelToPrefab();

                fadePanel = 0;
            }
            //---------------------------------------------------------------------------//
        }
        EditorGUILayout.EndFadeGroup();
    }
}
