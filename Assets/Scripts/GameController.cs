using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameState{ FreeRoam, Battle}
public class GameController : MonoBehaviour
{
    [FormerlySerializedAs("playerControl")] [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    
    private GameState State;

    private void Start()
    {
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        playerController.OnEcountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
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
    }
}
