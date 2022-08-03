using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ FreeRoam, Battle, Dialog, Cutscene}
public class GameController : MonoBehaviour

{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    
    public static GameController Instance { get; private set; } 
    
    private GameState State;
    private TrainerController trainerController;
    private void Awake()
    {
        ConditionDB.Init();
        playerController.OnEcountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        Instance = this;
    }

    private void Start()
    {
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        
        playerController.OnEnterTrainersView += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                State = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
            
        };

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

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
        battleSystem.StartBattle(playerParty, wildPokemonCopy);
    }
    
    public void StartTrainertBattle(TrainerController trainer)
    {
        State = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        trainerController = trainer;
        
        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    private void EndBattle(bool won)
    {
        if (trainerController != null && won)
        {
            trainerController.BattleLost();
            trainerController = null;
        }
        
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
