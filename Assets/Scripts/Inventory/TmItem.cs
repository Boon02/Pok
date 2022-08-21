using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New TM or HM Item")]
public class TmItem : ItemBase
{
    [SerializeField] private MoveBase move;

    public override bool Use(Pokemon pokemon)
    {
        // Learning move is handle from Inventory UI, If it was learned then return true 
        return pokemon.HasMove(move);
    }


    public MoveBase Move => move;
    
}
