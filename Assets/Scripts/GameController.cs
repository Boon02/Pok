using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ FreeRoam, Battle}
public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerControl playerControl;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    
    private GameState State;

    private void Start()
    {
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        playerControl.OnEcountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    private void StartBattle()
    {
        State = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        battleSystem.StartBattle();
    }

    private void EndBattle(bool won)
    {
        State = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
    
    private void Update()
    {
        if(State == GameState.FreeRoam)
        {
            playerControl.HandleUpdate();
        }
        else if (State == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
