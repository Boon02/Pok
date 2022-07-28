using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoditionDB
{
    public static Dictionary<CoditionID, Codition> Codisions { get; set; } 
        = new Dictionary<CoditionID, Codition>()
        {
            {
                CoditionID.psn,
                new Codition()
                {
                    Name = "psn",
                    StartMessage = "has been poison",
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.UpdateHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison.");
                    }
                }
            },
            {
                CoditionID.brn,
                new Codition()
                {
                    Name = "Burn",
                    StartMessage = "has been poison",
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.UpdateHP(pokemon.MaxHp / 16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to burn.");
                    }
                }
            }
        };
}

public enum CoditionID
{
    none,
    psn,    // độc tố - poison
    brn,    // đốt cháy
    slp,    // ngủ
    par,    // tê liệt
    frz     // đóng băng
    
}
