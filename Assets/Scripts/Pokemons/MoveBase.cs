using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create New Move")]
//https://bulbapedia.bulbagarden.net/wiki/List_of_moves_by_availability_(Generation_VIII)
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int pp;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveEffects effects;
    [SerializeField] private MoveTarget target;
    
    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public PokemonType Type
    {
        get { return type; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public int Pp
    {
        get { return pp; }
    }
    public MoveCategory Category
    {
        get { return category; }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
    
    
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;
    [SerializeField] private CoditionID status;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    
    public CoditionID Status
    {
        get { return status; }
    }
}


[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physic, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}
