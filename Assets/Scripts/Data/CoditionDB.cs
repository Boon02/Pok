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
            },
            {
                CoditionID.par,
                new Codition()
                {
                    Name = "Paralyze",
                    StartMessage = "has been Paralyze ",
                    OnBeforMove = (Pokemon pokemon) =>
                    {
                        int value = Random.Range(1, 5);
                        Debug.Log(value);
                        if (value == 1)
                        {
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s Paralyzed and can't move.");
                            return false;
                        }

                        return true;
                    }
                }
            },
            {
                CoditionID.frz,
                new Codition()
                {
                    Name = "Freeze",
                    StartMessage = "has been frozen ",
                    OnBeforMove = (Pokemon pokemon) =>
                    {
                        pokemon.CureStatus();
                        int value = Random.Range(1, 5);
                        Debug.Log(value);
                        if (value == 1)
                        {
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} isn't frozen anymore.");
                            return true;
                        }

                        return false;
                    }
                }
            },
            {
                CoditionID.slp,
                new Codition()
                {
                    Name = "Sleep",
                    StartMessage = "has fallen asleep",
                    OnStart = (Pokemon pokemon) =>
                    {
                        //slp for 1-3 turn
                        pokemon.StatusTime = Random.Range(1, 4);
                        Debug.Log($"Will be asleep for {pokemon.StatusTime} move");
                    },
                    OnBeforMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                            return true;
                        }

                        pokemon.StatusTime--;
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping!");
                        return false;
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
