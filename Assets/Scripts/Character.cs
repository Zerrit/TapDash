using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Commands
{
    TurnRight,
    TurnLeft,
    Jump
}

public class Character : MonoBehaviour
{
    private Vector2 moveDirection = new Vector2(0, 1);
    private int defaultSpeed = 3;

    [SerializeField] private int speedDificulty = 1;
    [SerializeField] private bool isCanTapping = false; // Флак доступности управления персонажем
    [SerializeField] private bool isImmortal;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int currentCommandId = 0; // Номер текущей команды
    [SerializeField] public List<Commands> commands = new List<Commands>();
    [SerializeField] private Commands nextCommand;

    public Transform roadChecker; // Объект для проверки обрыва
    private Transform player;
    private Animator anim;

    private void Start()
    {
        player = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        StopPlayer();
    }

    private void Update()
    {
        Move();
        CheckLose();
        CheckTap();
    }
    
    private void SetNextCommand() // Установка следующей команды
    {
        currentCommandId++;
        nextCommand = commands[currentCommandId];
    }
    private void SetStartCommand() // Установка стартовой команды уровня
    {
        currentCommandId = 0;
        nextCommand = commands[currentCommandId];
    }
    public void Tap() // Управление персонажа "Тапом". Запускает соответствующий эвент в скрипте GameManager
    {
        isImmortal = false;
        GameManager.instance.TapEventActivate(nextCommand);

        switch (nextCommand)
        {
            case Commands.Jump:
                StartCoroutine(Jump());
                //Jump();
                SetNextCommand();
                break;

            case Commands.TurnLeft:
                StartCoroutine(Turn(player.eulerAngles.z, 90f));
                SetNextCommand();
                break;

            case Commands.TurnRight:
                StartCoroutine(Turn(player.eulerAngles.z,  -90f));
                SetNextCommand();
                break;
        } 
    }

    private IEnumerator Jump()
    {
        isImmortal = true;
        isCanTapping = false;
        anim.SetTrigger("Jump");
        float t = Time.time;
        while ((t+0.6f - (0.1f * speedDificulty)) > Time.time)
        {
            yield return null;
        }
        isImmortal = false;
        anim.SetTrigger("Run");
        isCanTapping = true;
    }
    public IEnumerator MoveToCentre(Vector3 origin, Vector3 centre)  // Центрирование персонажа
    {
        isCanTapping = false;
        float t = 0;
        while (t < 1)
        {
            yield return null;
            t += 5f * Time.deltaTime;
            if (isPayerDirectionVecrtical(player.eulerAngles.z))
            {
                player.position = new Vector3(Mathf.Lerp(origin.x, centre.x, t), player.position.y, -1);
            }
            else
            {
                player.position = new Vector3(player.position.x, Mathf.Lerp(origin.y, centre.y, t), -1);
            }
        }
        //isCanTapping = true;
    } 
    public IEnumerator Turn(float origin, float angle)  // Поворот персонажа
    {
        float t = 0;
        while (t < 1)
        {
            yield return null;
            t += 12 * Time.deltaTime;
            player.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(origin, origin + angle, t));
        }
    } 
    public IEnumerator StartPlayer()    // Запуск персонажа
    {
        yield return new WaitForFixedUpdate();
        //yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("Run");
        SetMoveSpeed();
        isImmortal = false;
    } 

    private void CheckTap()
    {
        if (Input.GetMouseButtonDown(0) && isCanTapping)
        {
            Tap();
        }
    }
    private void CheckLose() // Проверка на выход с дорожки. Запуск события проигрыша
    {
        if (!Physics2D.OverlapCircle(roadChecker.position, 0.1f, 1 << 8) && !isImmortal)
        {
            anim.SetTrigger("Fall");
            StopPlayer();
            speedDificulty = 1;

            GameManager.instance.levelLose.Invoke();
        }
    }
    public void CompleteLevel() // Запуск события о прохождении уровня
    {
        ChangeSpeedDifficulty();
        SetMoveSpeed();
        GameManager.instance.levelComplete.Invoke();
    }
    public void MoveToStart(Vector3 point, Quaternion playerRotation) // Перемещение персонажа на точку старта текущего уровня
    {
        currentCommandId = 0;
        player.rotation = playerRotation;
        player.Translate(point - player.position, Space.World);
        StopPlayer();
        StartCharacter();
    }
    public void SetDefaultPos() // Перемещение персонажа в нулевые координаты (Необходимо при перезапуске уровня из меню)
    {
        player.rotation = Quaternion.identity;
        player.position = new Vector3(0, 0, -1);
        anim.SetTrigger("OnStart");
        StopPlayer();
    }
    public void StartCharacter() // Устанавливаие первую команду на выполнение, активирует следующую стрелку и стартует персонажа
    {
        SetStartCommand();
        StartCoroutine(StartPlayer());
    }
    public void StopPlayer()
    {
        moveSpeed = 0;
        anim.SetFloat("Speed", 0);
        isImmortal = true;
        isCanTapping = false;
    }
    private void Move()
    {
        player.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
    public void SetMoveSpeed()
    {
        moveSpeed = defaultSpeed + speedDificulty;
        anim.SetFloat("Speed", .25f * moveSpeed);
    }
    public void ChangeSpeedDifficulty(int index)
    {
        speedDificulty = index;
    }
    public void ChangeSpeedDifficulty()
    {
        speedDificulty = Mathf.Clamp(++speedDificulty, 1, 3);
    }

    public void ClearOldCommands(int countCommands)
    {
        commands.RemoveRange(0, countCommands);
        currentCommandId -= countCommands;
    }
    private bool isPayerDirectionVecrtical(float rotation)
    {
        if ((Mathf.Abs(rotation) < 5f) || (Mathf.Abs(rotation) > 355f) || (Mathf.Abs(rotation) > 175f && Mathf.Abs(rotation) < 185f)) return true;
        else return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            isCanTapping = false;
        }

        if (collision.tag == "Start" && !isImmortal)
        {
            StartCoroutine(MoveToCentre(player.position, collision.transform.position));
            CompleteLevel();          
        }

        if (collision.tag == "Crystal")
        {
            GameManager.instance.crystalTaken.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Start")
        {
            isCanTapping = true;
        }
    }
}