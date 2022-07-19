using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

}
