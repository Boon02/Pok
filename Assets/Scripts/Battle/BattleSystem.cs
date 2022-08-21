using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState{ Start, ActionSelection, MoveSelection, RunningTurn, Busy, Bag, PartyScreen, AboutToUse, MoveToForget, BattleOver }
public enum BattleAction{Move, UsingItem, SwitchPokemon, Run}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogBox dialogBox;

    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;
    [SerializeField] private GameObject pokeballSprite;
    [SerializeField] private MoveSelectionUI moveSelectionUI;
    [SerializeField] private InventoryUI inventoryUI;

    private int currentAction = 0;
    private int currentMove = 0;
    private int escapeAttempts;
    private bool aboutToUseChoice = true;
    private PokemonParty playerParty;
    private PokemonParty trainerParty;
    private Pokemon wildPokemon;
    
    private BattleState state;
    
    private TrainerController trainer;
    private PlayerController player;
    private bool isTrainerBattle;
    private MoveBase moveToLearn;
    
    public event Action<bool> OnBattleOver;

    private void Start()
    {
        playerImage.gameObject.SetActive(false);
        trainerImage.gameObject.SetActive(false);
        dialogBox.EnableChoiceBox(false);
        moveSelectionUI.gameObject.SetActive(false);
        partyScreen.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
    }
    
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        isTrainerBattle = false;
        
        player = playerParty.GetComponent<PlayerController>();
        
        StartCoroutine(SetUpBattle());
    }
    
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        if (!isTrainerBattle)
        {
            // wild pokemon battel
            playerUnit.UnClear();
            enemyUnit.UnClear();
            playerUnit.SetUp(playerParty.GetHealthyPokemon());
            enemyUnit.SetUp(wildPokemon);
            
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
        }
        else
        {
            playerUnit.Clear();
            playerUnit.gameObject.SetActive(false);
        
            enemyUnit.Clear();
            enemyUnit.gameObject.SetActive(false);
            
            // trainer battle
            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} wanna battle.");
            
            playerImage.gameObject.SetActive(false);
            trainerImage.gameObject.SetActive(false);
            
            //send out first pokemon of the trainer
            enemyUnit.UnClear();
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.SetUp(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.name} send out {enemyPokemon.Base.name}.");

            //send out first pokemon of the player
            playerUnit.UnClear();
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.SetUp(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.name}.");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        escapeAttempts = 0;
        partyScreen.Init();
        ActionSelection();
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Choose an action!"));
        dialogBox.EnableActionSelection(true);
    }
    
    void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
        dialogBox.gameObject.SetActive(false);
    }

    void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        
        dialogBox.gameObject.SetActive(false);
        partyScreen.SetPartyData();
        partyScreen.gameObject.SetActive(true);
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.BattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won);
    }
    
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelection(false);
        dialogBox.EnableMoveSelection(true);
    }

    IEnumerator ChooseMoveForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog("Choose the move want forget!");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x=>x.Base).ToList(), newMove);
        moveToLearn = newMove;
        
        state = BattleState.MoveToForget;
    }
    
    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} is Fainted!");

        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayer)
        {
            // ++ EXP
            int expYield = enemyUnit.Pokemon.Base.ExpYield;
            int level = enemyUnit.Pokemon.Level;

            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * level * trainerBonus) / 7);
            playerUnit.Pokemon.EXP += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp!");
            yield return playerUnit.Hud.SetExpSmooth();
            
            // check Level Up
            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}");
                playerUnit.Hud.SetLevel();
                var newMove = playerUnit.Pokemon.GetMoveWithCurrentLevel();

                if (newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < 4)
                    {
                        playerUnit.Pokemon.LearnToMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.MoveBase.Name}");
                        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                    }
                    else
                    {
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} trying to learn {newMove.MoveBase.Name}");
                        yield return dialogBox.TypeDialog($"But it can't learn more than {PokemonBase.MaxNumOfMove} moves");
                        yield return ChooseMoveForget(playerUnit.Pokemon, newMove.MoveBase);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }
                
                yield return playerUnit.Hud.SetExpSmooth(true);
            }
            yield return new WaitForSeconds(1f);

        }

        CheckForBattleOver(faintedUnit);
    }
    
    IEnumerator AboutToUse(Pokemon pokemon)
    {
        state = BattleState.Busy;

        yield return dialogBox.TypeDialog(
            $"{trainer.Name} is about to use {pokemon.Base.Name}. Do you want change pokemon?");
        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            bool playerGoesFirst = true;
            if (enemyUnit.Pokemon.CurrentMove.Base.Priority > playerUnit.Pokemon.CurrentMove.Base.Priority)
                playerGoesFirst = false;
            else if (enemyUnit.Pokemon.CurrentMove.Base.Priority == playerUnit.Pokemon.CurrentMove.Base.Priority)
            {
                playerGoesFirst = playerUnit.Pokemon.Base.Speed > enemyUnit.Pokemon.Base.Speed;
            }

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;
            
            //first turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            if(state == BattleState.BattleOver) yield break;
            
            //second turn
            var secondPokemon = secondUnit.Pokemon;
            if (secondPokemon.HP > 0)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if(playerAction == BattleAction.SwitchPokemon)
            {
                var currentPokemon = partyScreen.SelectedMember;
                yield return SwitchPokemon(currentPokemon);
            }
            else if (playerAction == BattleAction.UsingItem)
            {
                dialogBox.EnableActionSelection(false);
                //yield return ThrowPokeball();
            }else if (playerAction == BattleAction.Run)
            {
                dialogBox.EnableActionSelection(false);
                yield return TryToEscape();
            }
            
            //enemy turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove); 
            yield return RunAfterTurn(enemyUnit);
            if(state == BattleState.BattleOver) yield break;
        }
        
        // red point
        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }
        
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;

        bool canMove = sourceUnit.Pokemon.OnBeforMove();
        if (!canMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.WaitForUpdateHp();
            yield break;
        }
        
        dialogBox.gameObject.SetActive(true);
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

                yield return targetUnit.Hud.WaitForUpdateHp();
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
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed!");
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
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
        {
            if (isTrainerBattle) 
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon == null)
                    BattleOver(true);
                else
                    StartCoroutine(AboutToUse(nextPokemon));
            }
            else
                BattleOver(true);
            
        }
        
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

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if(state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
        
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForUpdateHp();
        
        //checkSource
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
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
    
    IEnumerator SwitchPokemon(Pokemon newPokemon, bool isTrainerAboutToUse = false)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}.");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        
        playerUnit.SetUp(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        if(isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerPokemon());
        else 
            state = BattleState.RunningTurn;
    }

    IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;

        var nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.SetUp(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.name} send out {nextPokemon.Base.name}.");
        
        state = BattleState.RunningTurn;
        ActionSelection();
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onSelectedMove = (index) =>
            {
                moveSelectionUI.gameObject.SetActive(false);

                if (index == PokemonBase.MaxNumOfMove)
                {
                    // don't learn the new move
                    StartCoroutine(
                        dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} didn't learn {moveToLearn.Name}."));

                }
                else
                {
                    // forget the move selected
                    var selectedMove = playerUnit.Pokemon.Moves[index];
                    StartCoroutine(dialogBox.TypeDialog(
                        $"{playerUnit.Pokemon.Base.Name} forgot {selectedMove.Base.Name} and learn {moveToLearn.Name}."));
                    playerUnit.Pokemon.Moves[index] = new Move(moveToLearn);

                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };

            moveSelectionUI.HandleMoveSelection(onSelectedMove);
        }
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
                dialogBox.gameObject.SetActive(true);
            };
            
            Action onItemUse = () =>
            {
                state = BattleState.Busy;
                inventoryUI.gameObject.SetActive(false);
                StartCoroutine(RunTurns(BattleAction.UsingItem));
            };

            inventoryUI.HandleUpdate(onBack, onItemUse);
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
                //StartCoroutine(RunTurns(BattleAction.UsingItem));
                OpenBag();
            }
            else if(currentAction == 2)
            {
                //Pokemon
                OpenPartyScreen();
            }
            else if(currentAction == 3)
            {
                //Run
                StartCoroutine(RunTurns(BattleAction.Run));
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
            var move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0) return;
            
            dialogBox.EnableDialogText(true);
            dialogBox.EnableMoveSelection(false);
            StartCoroutine(RunTurns(BattleAction.Move));
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
        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
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
            dialogBox.gameObject.SetActive(true);
            if (partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = (partyScreen.CalledFrom == BattleState.AboutToUse);
                StartCoroutine(SwitchPokemon(selectedMember, isTrainerAboutToUse));
            }
            
            partyScreen.CalledFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessage("You have to choice a pokemon to continue");
                return;
            }
            
            partyScreen.gameObject.SetActive(false);
            dialogBox.gameObject.SetActive(true);

            if (partyScreen.CalledFrom == BattleState.AboutToUse)
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
                ActionSelection();
            
            partyScreen.CalledFrom = null;
        };

        partyScreen.HandleUpdate(onSelected, onBack);

    }
    private void HandleAboutToUse()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) )
        {
            aboutToUseChoice = !aboutToUseChoice;
        }
        dialogBox.UpdateChoiceSelection(aboutToUseChoice);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                // Yes option
                dialogBox.EnableChoiceBox(false);
                OpenPartyScreen();
            }
            else
            {
                // No Option
                dialogBox.EnableChoiceBox(false);
                StartCoroutine(SendNextTrainerPokemon());
            }
        }else if(Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
        
    }

    void voidThrowPokeball()
    {
        StartCoroutine(ThrowPokeball());
    }

    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal the trainers pokemon");
            state = BattleState.RunningTurn;
            yield break;
        }
        
        yield return dialogBox.TypeDialog($"{player.Name} using POKEBALL.");
        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(6f, 0f), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        // Animation
        // - 1: ném pokeball lên trên đầu của pokemon
        yield return pokeball.transform.DOJump
        (
            enemyUnit.transform.position + new Vector3(0f, 2f, 0f),
            2f,
            1,
            1f
        ).WaitForCompletion();

        // - 2: pokemon bị nhốt
        yield return enemyUnit.PlayCaptureAnimation();

        // - 3: pokeball rơi
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        // - 4: pokeball rung lắc
        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon);
        
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokemon is caught
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} is caught.");
            yield return pokeball.DOFade(0f, 1.5f).WaitForCompletion();
            
            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been add to your party.");
            
            Destroy(pokeballObj);
            BattleOver(true);
        }
        else
        {
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBrokeOutAnimation();
            if (shakeCount <= 2) 
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke out.");
            else
            {
                yield return dialogBox.TypeDialog($"Almost caught it.");
            }
            Destroy(pokeballObj);

            state = BattleState.RunningTurn;
        }
    }
    
    // 2 for sleep and freeze, 1.5f for paralyze, poison, or burn and 1 otherwise 
    int TryToCatchPokemon(Pokemon pokemon)
    {
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * ConditionDB.GetStatusBonus(pokemon.Status) /
                  (3 * pokemon.MaxHp);
        if (a >= 255)
            return 4;
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (Random.Range(0, 65535) > b)
                break;
            ++shakeCount;
        }

        return shakeCount;
    }
    // run from battle
    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
    
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("You can't run from trainer battles!");
            state = BattleState.RunningTurn;
            yield break;
        }
    
        ++escapeAttempts;
                
        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;
    
        if (playerSpeed > enemySpeed)
        {
            yield return dialogBox.TypeDialog($"Run away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
    
            if (Random.Range(0, 256) <= f)
            {
                yield return dialogBox.TypeDialog($"Run away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog("Can't Escape!");
                state = BattleState.RunningTurn;
            }
        }
    }
}
