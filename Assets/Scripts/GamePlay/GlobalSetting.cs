using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GlobalSetting : MonoBehaviour
{
    [SerializeField] private Color highLightedColor;
    public Color HighLightedColor => highLightedColor;

    public static GlobalSetting i { get; private set; }

    private void Awake()
    {
        i = this;
    }
}
