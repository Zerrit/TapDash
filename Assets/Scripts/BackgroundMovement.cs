using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMovement : MonoBehaviour
{
    private Material mat;
    private float offset;
    public float offsetSpeed;

    private void Start()
    {
        mat = GetComponent<Image>().material;
    }
    private void Update()
    {
        offset += offsetSpeed * Time.deltaTime;
        mat.SetTextureOffset("_MainTex", new Vector2(offset, offset));

    }
}
