using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    private PokemonBase _base;
    private int level;

    public int HP;
    public List<Move> Moves;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        HP = _base.MaxHp;
        Moves = new List<Move>();
        foreach (var move in _base.LearnableMoves)
        {
            if (move.Level <= level)
            {
                Moves.Add(new Move(move.MoveBase));
            }

            if (Moves.Count >= 4)
                break;
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((_base.Attack * level) / 100f )+ 5;  }
    }
    
    public int Defense
    {
        get { return Mathf.FloorToInt((_base.Defense * level) / 100f )+ 5;  }
    }
    
    public int SpAttack
    {
        get { return Mathf.FloorToInt((_base.SpAttack * level) / 100f )+ 5;  }
    }
    
    public int SpDefense
    {
        get { return Mathf.FloorToInt((_base.SpDefense * level) / 100f )+ 5;  }
    }
    
    public int MaxHp
    {
        get { return Mathf.FloorToInt((_base.MaxHp * level) / 100f )+ 5;  }
    }
    
    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed * level) / 100f )+ 10;  }
    }
    
}
