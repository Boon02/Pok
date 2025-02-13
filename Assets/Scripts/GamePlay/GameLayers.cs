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
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;

    public static GameLayers i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidObejctsLayer { get => solidObejctsLayer; }
    
    public LayerMask InteractableLayer { get => interactableLayer; }
    
    public LayerMask GrassLayer { get => grassLayer; }
    
    public LayerMask PlayerLayer { get => playerLayer; }
    
    public LayerMask FovLayer { get => fovLayer; }
    
    public LayerMask PortalLayer => portalLayer; 
    
    public LayerMask TriggerableLayer => grassLayer | fovLayer | portalLayer  ; 
}
