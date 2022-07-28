using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pokemon
{

    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;

    public PokemonBase Base { get { return _base; } }

    public int Level { get { return level; } }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }   // Cấp độ của từng giá trị
    public Codition Status { get; private set; }

    public Queue<string> StatusChanges = new Queue<string>();
    public bool HpChanged { get; set; }
    
    
    public void Init()
    {
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }

            if (Moves.Count >= 4)
                break;
        }

        CalculationStats();
        
        HP = MaxHp;
        
        ResetStatBoost();
    }
    void CalculationStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f )+ 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f )+ 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f )+ 5);
        Stats.Add(Stat.Speed,  Mathf.FloorToInt((Base.Speed * Level) / 100f )+ 5);
        
        MaxHp =  Mathf.FloorToInt((Base.Speed * Level) / 100f )+ 10;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0 },
        };
    }
    
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = StatBoosts[stat] + boost;
            
            if(boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
            
            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}.");
        }
    }
    
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        // Apply stat boost
        int boost = StatBoosts[stat];
        float[] boostsValue = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostsValue[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostsValue[-boost]);
        
        return statVal;
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    
    public int Defense
    {
        get { return GetStat(Stat.Defense);  }
    }
    
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack);  }
    }
    
    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense);  }
    }
    
    public int MaxHp { get; set; }

    public int Speed
    {
        get { return GetStat(Stat.Speed);  }
    }
    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }
        
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) *
                     TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;
        
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * (attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);
        // if (HP <= 0)
        // {
        //     HP = 0;
        //     damageDetails.Fainted = true;
        // }
        return damageDetails;
    }

    //lấy CHIÊU THỨC bất kì
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public void BattleOver()
    {
        ResetStatBoost();
    }

    public void SetStatus(CoditionID coditionID)
    {
        Status = CoditionDB.Codisions[coditionID];
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
    }

    // nhận damage trừ máu
    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }
    
    public void OnAfterTurn()
    {
        Status.OnAfterTurn?.Invoke(this); // toán tử điều kiện - chỉ thực hiện hàm khi "this" khắc null
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
