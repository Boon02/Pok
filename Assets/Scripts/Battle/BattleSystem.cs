using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState{ Start, PlayerAction, PlayerMove, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHud playerHud;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHud enemyHud;
    [SerializeField] private BattleDialogBox dialogBox;

    private int currentAction = 0;
    private int currentMove = 0;
    
    private BattleState State;
    
    private void Start()
    {
        StartCoroutine( SetUpBattle());
    }
    

    IEnumerator SetUpBattle()
    {
        playerUnit.SetUp();
        enemyUnit.SetUp();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
        yield return new WaitForSeconds(1f);
        PlayerAction();
    }

    void PlayerAction()
    {
        State = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action!"));
        dialogBox.EnableActionSelection(true);
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
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}!");
        yield return new WaitForSeconds(1f);
        
        bool isFainted =  enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHp();
        
        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} is Fainted!");
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
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}!");
        yield return new WaitForSeconds(1f);
        
        bool isFainted =  playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        yield return playerHud.UpdateHp();
        
        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} is Fainted!");
        }
        else
        {
            PlayerAction();
        }
    }
    private void Update()
    {
        if (State == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if(State == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                ++currentAction;
            }
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }
        }
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
                //Run
            }
        }
    }
    
    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
        }
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableMoveSelection(false);
            StartCoroutine(PerformPlayerMove());
        }
    }
    
}
