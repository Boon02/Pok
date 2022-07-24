using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

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
}
