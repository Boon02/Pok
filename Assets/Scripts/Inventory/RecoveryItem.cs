using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Create New Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] private int hpAmount;
    [SerializeField] private bool restoreMaxHP;
    
    [Header("PP")]
    [SerializeField] private int ppAmount;
    [SerializeField] private bool restoreMaxPP;

    [Header("STATUS CONDITIONS")]
    [SerializeField] private ConditionID status;
    [SerializeField] private bool recoverAllStatus;
    
    [Header("REVIVE")]
    [SerializeField] private bool revive;   // hồi sinh
    [SerializeField] private bool maxRevive;   // max hồi sinh

    public override bool Use(Pokemon pokemon)
    {
        // Revive
        if (revive || maxRevive)
        {
            if (pokemon.HP > 0)
            {
                return false;
            }

            if (revive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp / 2);
            }
            else if (revive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp);
            }
            
            pokemon.CureStatus();

            return true;
        }
        
        // Don't use item when pokemon fainted
        if (pokemon.HP <= 0)
        {
            return false;   
        }
        
        // Restore HP
        if (restoreMaxHP || hpAmount > 0)
        {
            if (pokemon.HP == pokemon.MaxHp)
                return false;
            if(restoreMaxHP)
                pokemon.IncreaseHP(pokemon.MaxHp);
            else
                pokemon.IncreaseHP(hpAmount);
        }
        
        // Recover Status
        if (recoverAllStatus || status != ConditionID.none)
        {
            if (pokemon.Status == null && pokemon.VolatileStatus != null)
            {
                return false;
            }

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            }
            else
            {
                if (pokemon.Status.ID == status)
                {
                    pokemon.CureStatus();
                }else if (pokemon.VolatileStatus.ID == status)
                {
                    pokemon.CureVolatileStatus();
                }
            }
        }

        // Restore PP
        if (restoreMaxPP)
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.Pp));
        }
        else
        {
            if (ppAmount > 0)
            {
                pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
            }
        }
        
        return true;
    }
}
