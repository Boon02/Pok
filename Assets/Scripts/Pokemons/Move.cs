using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private MoveBase _base { get; set; }
    private int PP { get; set; }

    public Move(MoveBase mBase)
    {
        _base = mBase;
        PP = mBase.Pp;
    }

}
