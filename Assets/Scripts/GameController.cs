using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ FreeRoam, Battle, Dialog}
public class GameController : MonoBehaviour

{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    
    private GameState State;

    private void Awake()
    {
        ConditionDB.Init();
        playerController.OnEcountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        
    }

    private void Start()
    {
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        
        DialogManager.Instance.OnShowDialog += () =>
        {
            State = GameState.Dialog;
        };
        
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (State == GameState.Dialog) 
                State = GameState.FreeRoam;
        };
    }

    private void StartBattle()
    {
        State = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
        battleSystem.StartBattle(playerParty, wildPokemon);
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
            playerController.HandleUpdate();
        }
        else if (State == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (State == GameState.Dialog)
        {
            DialogManager.Instance.HanldeUpdate();
        }
    }
}
