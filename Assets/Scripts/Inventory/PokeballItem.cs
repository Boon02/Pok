using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create New Pokeball")]
public class PokeballItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
    
}
