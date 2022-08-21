using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> slots;
    [SerializeField] private List<ItemSlot> pokeballSlots;
    [SerializeField] private List<ItemSlot> tmSlots;

    private List<List<ItemSlot>> allSlots; 

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlots};
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS", "POKEBALLS", "TMs & HMs"
    };

    public List<ItemSlot> GetSlotsByCategory(int index)
    {
        var item = allSlots[index];
        if (item == null)
            return null;
        
        return item;
    }

    public event Action OnUpdated;

    public ItemBase UseItem(int index, Pokemon selectedPokemon, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        
        var item = currentSlots[index].Item;
        bool itemUsed = item.Use(selectedPokemon);
        
        if(itemUsed)
        {
            RemoveItem(item, selectedCategory);
            return item;
        }

        return null;
    }

    public void RemoveItem(ItemBase item, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        
        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count <= 0)
        {
            slots.Remove(itemSlot);
        }
        
        OnUpdated?.Invoke();
    }
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
    
}

[System.Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count;

    public ItemBase Item => item;
    public int Count {
        get => count;
        set => count = value;
    }
    
}
