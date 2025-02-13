using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator FaderIn(float time)
    {
        yield return image.DOFade(1f, time).WaitForCompletion();
    }
    
    public IEnumerator FaderOut(float time)
    {
        yield return image.DOFade(0f, time).WaitForCompletion();
    }
}
