using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Transform camTransform;
    private Camera cam;

    private int cameraDificulty;

    private void Start()
    {
        camTransform = GetComponent<Transform>();
        cam = GetComponent<Camera>();
        camTransform.position = new Vector3(player.position.x, player.position.y, -2);

        GameManager.instance.tapEvent += TapRotation;
        GameManager.instance.levelStart.AddListener(SetDefaultRotation);
        GameManager.instance.levelLose.AddListener(SetLoseCamera);
    }

    private void LateUpdate()
    {
        camTransform.position = new Vector3(player.position.x, player.position.y, -2);
    }

    private void TapRotation(Commands command)
    {
        switch (cameraDificulty)
        {
            case 1:
                if (command == Commands.TurnLeft) StartCoroutine(TurnCamera(camTransform.eulerAngles.z, 90f)); 
                if (command == Commands.TurnRight) StartCoroutine(TurnCamera(camTransform.eulerAngles.z, -90f));   
                break;

            case 2:
                if (command == Commands.TurnLeft) StartCoroutine(TurnCamera(camTransform.eulerAngles.z, 40f));
                if (command == Commands.TurnRight) StartCoroutine(TurnCamera(camTransform.eulerAngles.z, -40f));
                break;

            case 3:
                if (command == Commands.TurnLeft) StartCoroutine(TurnCamera(camTransform.eulerAngles.z, Random.Range(-100, 100)));
                if (command == Commands.TurnRight) StartCoroutine(TurnCamera(camTransform.eulerAngles.z, Random.Range(-100, 100)));
                if (command == Commands.Jump) StartCoroutine(TurnCamera(camTransform.eulerAngles.z, Random.Range(-200, 200)));
                break;
        }
    }

    private void SetCameraDistance(int value)
    {
        StartCoroutine(ResizeCamera(cam.orthographicSize, value * .5f));
    }

    public void ChangeCameraDificulty()
    {
        cameraDificulty = Mathf.Clamp(++cameraDificulty, 1, 3);
        SetCameraDistance(cameraDificulty);
    }
    public void ChangeCameraDificulty(int value)
    {
        cameraDificulty = value;
        SetCameraDistance(value);
    }

    public IEnumerator TurnCamera(float origin, float angle)
    { 
        float t = 0;
        while (t < 1)
        {
            yield return null;
            t += 10 * Time.deltaTime;
            camTransform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(origin, origin + angle, t));
        }
    }

    public IEnumerator ResizeCamera(float origin, float size)
    {
        float t = 0;
        while (t < 1)
        {
            yield return null;
            t += 3 * Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(origin, 3.75f + size, t);
        }
    }

    private void SetDefaultRotation()
    {
        camTransform.rotation = player.rotation;
    }

    private void SetLoseCamera()
    {
        SetCameraDistance(5);
    }

    private void SetDefaultSize()
    {
        cam.orthographicSize = 4;
    }
}
