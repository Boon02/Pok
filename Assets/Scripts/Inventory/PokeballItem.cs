using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Pokeball")]
public class PokeballItem : ItemBase
{
    [SerializeField] private float catchRateModfier = 1;
    
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public override bool CanUsedOutsideBattle => false;

    public float CatchRateModfier => catchRateModfier;
}
