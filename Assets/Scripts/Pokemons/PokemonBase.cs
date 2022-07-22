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

    [SerializeField] private List<LearnableMove> learnableMoves;

    
    
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

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon
}

public class TypeChart
{
    private static float[][] Chart =
    {
        //                    NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI 
        /*NOR*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*FIR*/ new float[] { 1f, 0.5f,0.5f, 1f,  2f,  2f,  1f,  1f},
        /*WAT*/ new float[] { 1f, 2f,  0.5f, 2f, 0.5f, 1f,  1f,  1f},
        /*ELE*/ new float[] { 1f, 1f,   2f,0.5f, 0.5f, 2f,  1f,  1f},
        /*GRS*/ new float[] { 1f, 0.5f, 2f,  2f, 0.5f, 1f,  1f,  0.5f},
        /*POI*/ new float[] { 1f, 1f,   1f,  1f,   2f, 1f,  1f,  1f},
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














