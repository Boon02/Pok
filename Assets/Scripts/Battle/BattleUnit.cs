using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;
    [SerializeField] private bool isPlayer;

    public Pokemon Pokemon { get; set; }

    public void SetUp()
    {
        Pokemon = new Pokemon(_base, level);
        if (isPlayer)
        {
            GetComponent<Image>().sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            GetComponent<Image>().sprite = Pokemon.Base.FontSprite;
        }
    }

}
