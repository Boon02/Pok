using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public Move(MoveBase mBase)
    {
        Base = mBase;
        PP = mBase.Pp;
    }

    public Move(MoveSaveData saveData)
    {
        Base = MovesDB.GetMoveWithName(saveData.name);
        PP = saveData.pp;
    }

    public MoveSaveData GetSaveMove()
    {
        var saveData = new MoveSaveData()
        {
           name = Base.Name,
           pp = PP
        };

        return saveData;
    }

    public void IncreasePP(int amount)
    {
        PP = Mathf.Clamp(PP + amount, 0, Base.Pp);
    } 

}
[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}
