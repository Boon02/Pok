using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ FreeRoam, Battle, Menu, Dialog, PartyScreen, Bag, Cutscene, Paused}
public class GameController : MonoBehaviour

{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private InventoryUI inventoryUI;
    
    public SceneDetails CurrentScene { get; private set; } 
    public SceneDetails PrevScene { get; private set; } 
    
    public static GameController Instance { get; private set; } 
    
    private GameState state;
    private GameState stateBeforPause;
    private TrainerController trainerController;
    private MenuController menuController;
    private void Awake()
    {
        ConditionDB.Init();
        PokemonsDB.Init();
        MovesDB.Init();
        battleSystem.OnBattleOver += EndBattle;
        Instance = this;

        menuController = GetComponent<MenuController>();
        
        // Lock mouse
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        
    }

    private void Start()
    {
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        inventoryUI.gameObject.SetActive(false);
        
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };
        
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog) 
                state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += (x) =>
        {
            if (x == 0)
            {
                //Debug.LogError("TODO: Pokemon");
                partyScreen.gameObject.SetActive(true);
                partyScreen.SetPartyData(playerController.GetComponent<PokemonParty>().Pokemons);
                state = GameState.PartyScreen;
            }else if (x == 1)
            {
                //Debug.LogError("TODO: Bag");
                inventoryUI.gameObject.SetActive(true);
                state = GameState.Bag;

            }else if (x == 2)
            {
                //Debug.LogError("TODO: Save");
                SavingSystem.i.Save("saveSlot_1");
                state = GameState.FreeRoam;
            }else if (x == 3)
            {
                //Debug.LogError("TODO: Load");
                SavingSystem.i.Load("saveSlot_1");
                state = GameState.FreeRoam;
            }
        };

        menuController.onBack += () =>
        {
            partyScreen.gameObject.SetActive(false);
            state = GameState.FreeRoam;
        };
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon();
        
        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
        battleSystem.StartBattle(playerParty, wildPokemonCopy);
    }
    
    public void StartTrainertBattle(TrainerController trainer)
    {
        state = GameState.Battle;
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
        
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
    
    private void Update()
    {
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }

        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HanldeUpdate();
        }
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                Debug.LogError("TODO: go to Summary Screen ");
            };

            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            
            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onSelected = () =>
            {
                Debug.LogError("TODO: onSelected Bag");
            };
            
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            
            inventoryUI.HandleUpdate(onSelected, onBack);
        }
    }
    
    public void OnEnterTrainersView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforPause = state;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforPause;
        }
    }

    public void SetCurrentScene(SceneDetails currentScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currentScene;
    }
}
