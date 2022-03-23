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

    public int levelDificulty = 1;
    public bool isCanTapping = false; // Флак доступности управления персонажем
    public bool isImmortal;
    public float moveSpeed;
    public int currentCommandNumber; // Номер текущей команды

    public List<Arrow> arrows = new List<Arrow>(); //Список стрелок на уровне, хранящих действие
    public Commands nextCommand;
    public Transform roadChecker; // Объект для проверки обрыва

    private Transform player;
    private Animator anim;

    private void Start()
    {
        player = GetComponent<Transform>();
        anim = GetComponent<Animator>();

        GameManager.instance.dificultyEvent += ChangeSpeedDif;

        StopPlayer();
    }

    private void Update()
    {
        Move();
        CheckLose();
        CheckTap();
    }
    
    public void SetNextCommand() // Переключение на следующую команду, выключение пройденной стрелки и подсветка следующей
    {
        arrows[currentCommandNumber].TurnOff();
        currentCommandNumber++;
        nextCommand = arrows[currentCommandNumber].command;
        arrows[currentCommandNumber].Activate();
    }
    public void Tap() // Управление персонажа "Тапом". Запускает соответствующий эвент в скрипте GameManager
    {
        isImmortal = false;
        GameManager.instance.TapEventActivate(nextCommand);

        switch (nextCommand)
        {
            case Commands.Jump:
                Jump();
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
    public void Jump() // Прыжок
    {
        isImmortal = true;
        anim.SetTrigger("Jump");
    } 
    public void EndJump() // Завершение прыжка
    {
        isImmortal = false;
        anim.SetTrigger("Run");
    }

    public IEnumerator MoveToCentre(float origin, float centre)  // Центрирование персонажа
    {
        isCanTapping = false;

        float t = 0;
        while (t < 1)
        {
            yield return null;
            t += 5f * Time.deltaTime;
            player.position = new Vector3(Mathf.Lerp(origin, centre, t), player.position.y, -1);
        }

        isCanTapping = true;
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
            levelDificulty = 1;

            GameManager.instance.levelLose.Invoke();
        }
    }
    public void CompleteLevel() // Запуск события о прохождении уровня
    {
        GameManager.instance.DificultyEventActivate(Mathf.Clamp(levelDificulty + 1, 1, 3));
        SetMoveSpeed();
        GameManager.instance.levelComplete.Invoke();
    }
    public void MoveToStart(Transform point) // Перемещение персонажа на точку старта текущего уровня
    {
        player.rotation = Quaternion.identity;
        player.Translate(point.position - player.position);
        StopPlayer();
        SetStartCommand();
    }
    public void SetDefaultPos() // Перемещение персонажа в нулевые координаты (Необходимо при перезапуске уровня из меню)
    {
        player.rotation = Quaternion.identity;
        player.position = new Vector3(0, 0, -1);
        StopPlayer();
    }
    public void SetStartCommand() // Устанавливаие первую команду на выполнение, активирует следующую стрелку и стартует персонажа
    {
        currentCommandNumber = 0;
        nextCommand = arrows[currentCommandNumber].command;
        arrows[currentCommandNumber].Activate();

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
        moveSpeed = Mathf.Clamp((defaultSpeed + levelDificulty), 4, 8);
        anim.SetFloat("Speed", .25f * moveSpeed);
    }
    public void ChangeSpeedDif(int speedDif)
    {
        levelDificulty = speedDif;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            isCanTapping = false;
        }

        if (collision.tag == "Start" && !isImmortal)
        {
            CompleteLevel();
            StartCoroutine(MoveToCentre(player.position.x, collision.transform.position.x));
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
        
// Рассмотреть вариант с хранение в массиве arrows только объектов текущего уровня. И обновлять их при входе в триггер 
// стартового сегмента. Этим альтернативным образом можно будет избежать бага при повороте у финиша и упростить скрипт
// посредством уменьшения количества хранимой и обрабатываемой информации скриптом.