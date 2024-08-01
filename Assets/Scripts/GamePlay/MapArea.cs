using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] private List<Pokemon> WildPokemons;

    // lấy ngẫu nhiên
    public Pokemon GetRandomWildPokemon()
    {
        var wildPokemon = WildPokemons[Random.Range(0, WildPokemons.Count)];
        wildPokemon.Init();
        return wildPokemon;
    }
}
