using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New TM or HM Item")]
public class TmItem : ItemBase
{
    [SerializeField] private MoveBase move;
    [SerializeField] private bool isHM;

    public override string Name => base.Name + $": {move.Name}";

    public override bool Use(Pokemon pokemon)
    {
        // Learning move is handle from Inventory UI, If it was learned then return true 
        return pokemon.HasMove(move);
    }


    public bool CanBeTaught(Pokemon pokemon)
    {
        return pokemon.Base.LearnableByItems.Contains(move);
    }
    public override bool IsReusable => isHM;
    public override bool CanUseInBattle => false;
    public MoveBase Move => move;
    public bool IsHM => isHM;
        
}
