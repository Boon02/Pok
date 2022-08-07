using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonsDB : MonoBehaviour
{
    static Dictionary<string, PokemonBase> pokemons;

    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();
        
        var pokemonArray = Resources.LoadAll<PokemonBase>("");
        foreach (var pokemon in pokemonArray)
        {
            if (pokemons.ContainsKey(pokemon.Name))
            {
                Debug.LogError($"There are two pokemons with the name {pokemon.Name}");
                continue;
            }
            
            pokemons[pokemon.Name] = pokemon;
        }
    }

    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemons.ContainsKey(name))
        {
            Debug.Log($"Pokemon with {name} not found in the database");
            return null;
        }

        return pokemons[name];
    }
}
