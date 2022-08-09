using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// True : được đánh
// False : ko được đánh
public class ConditionDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionID = kvp.Key;
            var conditionValue = kvp.Value;

            conditionValue.ID = conditionID;
        }
    }
    
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } 
        = new Dictionary<ConditionID, Condition>()
        {
            {
                ConditionID.psn,
                new Condition()
                {
                    Name = "psn",
                    StartMessage = "has been poison",
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison.");
                    }
                }
            },
            {
                ConditionID.brn,
                new Condition()
                {
                    Name = "Burn",
                    StartMessage = "has been poison",
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to burn.");
                    }
                }
            },
            {
                ConditionID.par,
                new Condition()
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
                ConditionID.frz,
                new Condition()
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
                ConditionID.slp,
                new Condition()
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
            },
            
            // Volatile Status Conditions
            {
                ConditionID.confusion,
                new Condition()
                {
                    Name = "Confusion",
                    StartMessage = "has fallen confused",
                    OnStart = (Pokemon pokemon) =>
                    {
                        //slp for 1-4 turn
                        pokemon.VolatileStatusTime = Random.Range(1, 5);
                        Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} move");
                    },
                    OnBeforMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.VolatileStatusTime <= 0)
                        {
                            pokemon.CureVolatileStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} kicked out of confusion!");
                            return true;
                        }

                        pokemon.VolatileStatusTime--;

                        if (Random.Range(1, 3) == 1)
                        {
                            return true;
                        }

                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused!");
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"It hurt itself due to confusion!");
                        return false;
                    }
                }
            }
        };
    
    // 2 for sleep and freeze, 1.5f for paralyze, poison, or burn and 1 otherwise 
    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.ID == ConditionID.slp || condition.ID == ConditionID.frz)
            return 2f;
        else if (condition.ID == ConditionID.par || condition.ID == ConditionID.psn || condition.ID == ConditionID.brn)
            return 1.5f;
        return 1f;
    }
}

public enum ConditionID
{
    none,
    psn,    // độc tố - poison
    brn,    // đốt cháy
    slp,    // ngủ
    par,    // tê liệt
    frz,    // đóng băng
    confusion
    
}
