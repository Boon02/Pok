using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private Text messageText;

    [SerializeField] PartyMemberUI[] memberSlots;
    private List<Pokemon> pokemons;
    private PokemonParty party;
    private int selection = 0;
    
    public BattleState? CalledFrom { get; set; }
    public Pokemon SelectedMember => pokemons[selection];
    

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        party = PokemonParty.GetPlayerParty(); 
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()  //List<Pokemon> pokemons)
    {
        //this.pokemons = pokemons;
        pokemons = party.Pokemons;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }
        
        UpdateMemberSelection(selection);
        messageText.text = "Choose a pokemon!";
    }
    
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        int prevSelection = selection;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            selection += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            selection -= 2;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            ++selection;
        
        selection = Mathf.Clamp(selection, 0, pokemons.Count - 1);
        
        if(prevSelection != selection)
            UpdateMemberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        } 
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
        
    }
    
    public void UpdateMemberSelection(int selectedMove)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            if (i == selectedMove)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
