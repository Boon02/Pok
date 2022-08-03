using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create New Pokemon")]
//https://bulbapedia.bulbagarden.net/wiki/List_of_Pokémon_by_base_stats_(Generation_VIII-present)
public class PokemonBase : ScriptableObject
{
    [SerializeField] private string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private Sprite fontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;
    
    // Base Stats
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
    
    //https://bulbapedia.bulbagarden.net/wiki/List_of_Pok%C3%A9mon_by_effort_value_yield
    [SerializeField] private int expYield;

    //https://bulbapedia.bulbagarden.net/wiki/List_of_Pok%C3%A9mon_by_catch_rate
    // catchRate càng bé càng hiếm
    [SerializeField] private int catchRate = 255;
    [SerializeField] private GrowthRate growthRate;
    
    [SerializeField] private List<LearnableMove> learnableMoves;

    public int GetExpForlevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return Mathf.FloorToInt(4 * (level * level * level) / 5);
        }else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }

        return -1;
    }
    
    public string Name
    {
        get { return name; }
    }
    
    public string Description
    {
        get { return description; }
    }
    
    public Sprite FontSprite
    {
        get { return fontSprite; }
    }
    
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    
    public PokemonType Type1
    {
        get { return type1; }
    }
    
    public PokemonType Type2
    {
        get { return type2; }
    }
    
    public int MaxHp
    {
        get { return maxHp; }
    }
    
    public int Attack
    {
        get { return attack; }
    }
    
    public int Defense
    {
        get { return defense; }
    }
    
    public int SpAttack
    {
        get { return spAttack; }
    }
    
    public int SpDefense
    {
        get { return spDefense; }
    }
    
    public int Speed
    {
        get { return speed; }
    }
    
    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
    
    public int CatchRate => catchRate; 
    public int ExpYield => expYield;
    public GrowthRate GrowthRate => growthRate;
    
    
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private int level;

    public MoveBase MoveBase
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

public enum GrowthRate
{
    Fast,
    MediumFast
}

public enum Stat
{
    Attack,
    SpAttack,
    Defense,
    SpDefense,
    Speed,
    
    // These 2 are not actual stats, they're used to boost the Accuracy
    Accuracy,
    Evasion
}

public enum PokemonType
{
    None,
    Normal,
    Fight,
    Flying,      
    Poison,    
    Ground, 
    Rock,   
    Bug,    
    Ghost,    
    Steel,   
    Fire,       
    Water,       
    Grass,     
    Electric,   
    Psychic,  
    Ice,      
    Dragon,  
    Dark,     
    Fairy,
}

public class TypeChart
{
    private static float[][] Chart =
    {
        //                         NOR    FIG  FLY   POI GROUND ROCK   BUG   GOS  STEEL  FIRE   WAT   GRS  ELE   PSY   ICE   DRA    DARK  FAI
        /*NORMAL*/    new float[] { 1f,   1f,   1f,   1f, 0.5f,   1f,   0f, 0.5f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f},
        /*FIGHT*/     new float[] { 2f,   1f, 0.5f, 0.5f,   1f,   2f, 0.5f,   0f,   2f,   1f,   1f,   1f,   1f, 0.5f,   2f,   1f,   2f, 0.5f},
        /*FLYING*/    new float[] { 1f,   2f,   1f,   1f,   1f, 0.5f,   2f,   1f, 0.5f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f,   1f,   1f},  
        /*POISON*/    new float[] { 1f,   1f,   1f, 0.5f, 0.5f, 0.5f,   1f, 0.5f,   0f,   1f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f},
        /*GROUND*/    new float[] { 1f,   1f,   0f,   2f,   1f,   2f, 0.5f,   1f,   2f,   2f,   1f, 0.5f,   2f,   1f,   1f,   1f,   1f,   1f},
        /*ROCK*/      new float[] { 1f, 0.5f,   2f,   1f, 0.5f,   1f,   2f,   1f, 0.5f,   2f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f},
        /*BUG*/       new float[] { 1f, 0.5f, 0.5f, 0.5f,   1f,   1f,   1f, 0.5f, 0.5f, 0.5f,   1f,   2f,   1f,   2f,   1f,   1f,   2f, 0.5f},
        /*GHOST*/     new float[] { 0f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f, 0.5f,   1f},
        /*STEEL*/     new float[] { 1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f, 0.5f, 0.5f, 0.5f,   1f, 0.5f,   1f,   2f,   1f,   1f,   2f},
        /*FIRE*/      new float[] { 1f,   1f,   1f,   1f,   1f, 0.5f,   2f,   1f,   2f, 0.5f, 0.5f,   2f,   1f,   1f,   2f, 0.5f,   1f,   1f}, 
        /*WATER*/     new float[] { 1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   1f,   2f, 0.5f, 0.5f,   1f,   1f,   1f, 0.5f,   1f,   1f},  
        /*GRASS*/     new float[] { 1f,   1f, 0.5f, 0.5f,   2f,   2f, 0.5f,   1f, 0.5f, 0.5f,   2f, 0.5f,   1f,   1f,   1f, 0.5f,   1f,   1f}, 
        /*ELECTRIC*/  new float[] { 1f,   1f,   2f,   1f,   0f,   1f,   1f,   1f,   1f,   1f,   2f, 0.5f, 0.5f,   1f,   1f, 0.5f,   1f,   1f},  
        /*PSYCHIC*/   new float[] { 1f,   2f,   1f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f},
        /*ICE*/       new float[] { 1f,   1f,   2f,   1f,   2f,   1f,   1f,   1f, 0.5f, 0.5f, 0.5f,   2f,   1f,   1f, 0.5f,   2f,   1f,   1f},
        /*DRAGON*/    new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   0f},
        /*DARK*/      new float[] { 1f, 0.5f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f, 0.5f, 0.5f},
        /*FAIRY*/     new float[] { 1f,   2f,   1f, 0.5f,   1f,   1f,   1f,   1f, 0.5f, 0.5f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f},
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
        {
            return 1f;
        }

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return Chart[row][col];
    }
}














