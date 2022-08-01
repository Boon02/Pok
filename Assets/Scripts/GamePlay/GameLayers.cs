using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{ 
    [SerializeField] LayerMask solidObejctsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;

    public static GameLayers i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidObejctsLayer
    {
        get { return solidObejctsLayer; }
    }
    
    public LayerMask InteractableLayer
    {
        get { return interactableLayer; }
    }
    
    public LayerMask GrassLayer
    {
        get { return grassLayer; }
    }
    
    public LayerMask PlayerLayer
    {
        get { return playerLayer; }
    }
}
