using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BattleState{ Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
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
        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
        
        ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.Pokemon.Base.Speed > enemyUnit.Pokemon.Base.Speed)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    
    void ActionSelection()
    {
        State = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Choose an action!"));
        dialogBox.EnableActionSelection(true);
    }

    void OpenPartyScreen()
    {
        State = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void BattleOver(bool won)
    {
        State = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.BattleOver());
        OnBattleOver(won);
    }
    
    void MoveSelection()
    {
        State = BattleState.MoveSelection;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelection(false);
        dialogBox.EnableMoveSelection(true);
    }

    IEnumerator PlayerMove()
    {
        State = BattleState.PerformMove;
        
        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);
        
        // If the battle state wasn't changed by RunMove, then go to next step
        if(State == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }
    
    IEnumerator EnemyMove()
    {
        State = BattleState.PerformMove;
        
        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);
        
        // If the battle state wasn't changed by RunMove, then go to next step
        if(State == BattleState.PerformMove)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;

        bool canMove = sourceUnit.Pokemon.OnBeforMove();
        if (!canMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHp();
            yield break;
        }

        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}!");
        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);

            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffect(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);

                yield return targetUnit.Hud.UpdateHp();
                yield return ShowDamageDetails(damageDetails);
            }
            
            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    if (Random.Range(1, 101) <= secondary.Chance)
                    {
                        yield return RunMoveEffect(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                    }
                }
            }

            // checkTarget
            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} is Fainted!");

                targetUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(2f);

                CheckForBattleFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed!");
        }

        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHp();
        
        //checkSource
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} is Fainted!");
            
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            
            CheckForBattleFainted(sourceUnit);
        }
    }

    void CheckForBattleFainted(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
            BattleOver(true);
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            string message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = source.StatBoosts[Stat.Evasion];

        var boostValues = new float[] {1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f };
        
        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];
        
        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator RunMoveEffect(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        
        //Stat Boosting
        if(effects.Boosts != null )
        {
            if(moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        // Status Codition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetStatus(effects.VolatileStatus);
        }
        
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
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
        if (State == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if(State == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (State == BattleState.PartyScreen)
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
                MoveSelection();
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
            StartCoroutine(PlayerMove());
        } 
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableMoveSelection(false);
            ActionSelection();
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
            ActionSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonFaint = true;
        if (playerUnit.Pokemon.HP > 0)
        {
            currentPokemonFaint = false;
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}.");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        
        playerUnit.SetUp(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        if (currentPokemonFaint)
        {
            ChooseFirstTurn();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }

    }
    
    
}
