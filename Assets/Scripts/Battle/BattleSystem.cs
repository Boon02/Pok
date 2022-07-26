using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState{ Start, PlayerAction, PlayerMove, EnemyMove, Busy, PartyScreen}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHud playerHud;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHud enemyHud;
    [SerializeField] private BattleDialogBox dialogBox;

    [SerializeField] private PartyScreen partyScreen;

    private int currentAction = 0;
    private int currentMove = 0;
    private int currentMember = 0;
    private PokemonParty playerParty;
    private Pokemon wildPokemon;
    
    private BattleState State;
    
    
    public event Action<bool> OnBattleOver;
    
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        // thay đổi image (back sprite - font sprite)
        playerUnit.SetUp(playerParty.GetHealthyPokemon());
        enemyUnit.SetUp(wildPokemon);
        // set hud
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
        PlayerAction();
    }

    void PlayerAction()
    {
        State = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action!"));
        dialogBox.EnableActionSelection(true);
    }

    void OpenPartyScreen()
    {
        State = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void PlayerMove()
    {
        State = BattleState.PlayerMove;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelection(false);
        dialogBox.EnableMoveSelection(true);
    }

    IEnumerator PerformPlayerMove()
    {
        State = BattleState.Busy;
        
        var move = playerUnit.Pokemon.Moves[currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}!");
        
        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        
        enemyUnit.PlayHitAnimation();
        
        var damageDetails =  enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHp();
        yield return ShowDamageDetails(damageDetails);
        
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} is Fainted!");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
        
    }
    
    IEnumerator EnemyMove()
    {
        State = BattleState.EnemyMove;
        
        var move = enemyUnit.Pokemon.GetRandomMove();
        move.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}!");
        
        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        
        playerUnit.PlayHitAnimation();
        
        var damageDetails =  playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        yield return playerHud.UpdateHp();
        yield return ShowDamageDetails(damageDetails);
        
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} is Fainted!");
            playerUnit.PlayFaintAnimation();
            
            yield return new WaitForSeconds(2f);
            
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else
            {
                OnBattleOver(false);
            }
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A critical hit!");
        }

        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("This is super effective!");
        }
        else if(damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("This isn't very effective!");
        }
        else if(damageDetails.TypeEffectiveness == 1f)
        {
            yield return dialogBox.TypeDialog("This is normal effective!");
        }
    }
    
    public void HandleUpdate()
    {
        if (State == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if(State == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }

        if (State == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;

        currentAction = Mathf.Clamp(currentAction, 0, 3);
        
        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                PlayerMove();
            }
            else if(currentAction == 1)
            {
                //Bag
            }
            else if(currentAction == 2)
            {
                //Pokemon
                OpenPartyScreen();
            }
            else if(currentAction == 3)
            {
                //Run
            }
        }
    }
    
    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableMoveSelection(false);
            StartCoroutine(PerformPlayerMove());
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableMoveSelection(false);
            PlayerAction();
        }
    }
    
    private void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);
        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.K))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessage("You can't choose faint pokemon!");
                return;
            }

            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessage("You can't switch with a same pokemon!");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            State = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));

        } 
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            PlayerAction();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}.");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        
        playerUnit.SetUp(newPokemon);
        playerHud.SetData(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        StartCoroutine(EnemyMove());

    }
    
}
