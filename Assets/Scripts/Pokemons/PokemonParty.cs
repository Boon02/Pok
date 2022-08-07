using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;
        }
        set
        {
            pokemons = value;
        }
    }
    private void Start()
    {
        foreach (var item in pokemons)
        {
            item.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        // lấy ra pokemon đầu tiên trong nhóm nếu nó có HP > 0 hoặc trả về null
        return pokemons.Where(x => x.HP > 0 ).FirstOrDefault();
    }

    public void AddPokemon(Pokemon pokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(pokemon);
        }
        else
        {
            //TODO: Add to the PC once that's implemented
        }
    }
}
