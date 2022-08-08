using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesDB
{
    private static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();

        var moveArray = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveArray)
        {
            if (moves.ContainsKey(move.Name))
            {
                Debug.LogError($"There are two moves with the name {move.Name}");
            }

            moves[move.Name] = move;
            Debug.Log(move.Name + "-" + moves[move.Name]);
        }

    }

    public static MoveBase GetMoveWithName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.LogError($"Move with name {name} not found in database.");
            return null;
        }

        return moves[name];
    }
}
