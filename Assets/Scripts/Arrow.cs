using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour
{
    public Commands command;
    public SpriteRenderer sprite;

    public void Activate()
    {
        sprite.DOFade(1, 0);
    }

    public void TurnOff()
    {
        sprite.DOFade(0, .2f);
    }

    public void SetDefault()
    {
        sprite.DOFade(.3f, 0);
    }
}
